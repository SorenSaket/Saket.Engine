using System;
using System.Numerics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ImGuiNET;
using WebGpuSharp;
using WebGpuSharp.FFI;

using ImDrawIdx = ushort; // figure out what size index buffer imgui uses

namespace Saket.Engine.IMGUI;

delegate void UserCallbackDelegate(ImDrawListPtr parent_list, ImDrawCmdPtr cmd);

/// <summary>
///  https://github.com/ocornut/imgui/blob/master/backends/imgui_impl_wgpu.cpp
/// A translation of the webgpu backend for imgui
/// </summary>
public class ImGui_Impl_WebGPUSharp : IDisposable
{
    #region Structures

    /// <summary>
    /// Initialization data, for ImGui_ImplWGPU_Init()
    /// </summary>
    public struct ImGui_ImplWGPU_InitInfo
    {
        public required Device device;
        public uint num_frames_in_flight;
        public TextureFormat rt_format;
        public TextureFormat depth_format;
        public MultisampleState PipelineMultisampleState;

        public ImGui_ImplWGPU_InitInfo()
        {
            num_frames_in_flight = 3;
            rt_format = TextureFormat.Undefined;
            depth_format = TextureFormat.Undefined;
            PipelineMultisampleState.Count = 1;
            PipelineMultisampleState.Mask = uint.MaxValue;
            PipelineMultisampleState.AlphaToCoverageEnabled = false;
        }
    };

    /// <summary>
    /// A structure containing all data used internally for the renderer/WebGPU implementation. 
    /// These are mostly WebGPU Resources.
    /// </summary>
    internal struct RenderResources
    {
        // Font texture
        public Texture FontTexture;
        // Texture view for font texture
        public TextureView FontTextureView;
        // Sampler for the font texture
        public Sampler FontTextureSampler;
        // Shader uniforms
        public WebGpuSharp.Buffer Uniforms;
        // Resources bind-group to bind the common resources to pipeline
        public BindGroup CommonBindGroup;
        // Resources bind-group to bind the font/image resources to pipeline (this is a key->value map)
        public Dictionary<nint, BindGroup> ImageBindGroups;
        // Default fonImGuiStoraget-resource of Dear ImGui
        public BindGroup ImageBindGroup;
        // Cache layout used for the image bind group. Avoids allocating unnecessary JS objects when working with WebASM
        public BindGroupLayout ImageBindGroupLayout;
    };

    /// <summary>
    /// A structure contraining frame specific data.
    /// </summary>
    public struct FrameResources
    {
        public WebGpuSharp.Buffer IndexBuffer;
        public WebGpuSharp.Buffer VertexBuffer;
        public ImDrawIdx[] IndexBufferHost;
        public ImDrawVert[] VertexBufferHost;
        public int IndexBufferSize;
        public int VertexBufferSize;
    };

    /// <summary>
    /// A structure containing all data for the Implementation per IMGUI context
    /// </summary>
    internal struct ImGui_ImplWGPU_Data
    {
        public ImGui_ImplWGPU_InitInfo InitInfo;
        public Device device;
        public Queue defaultQueue;
        public TextureFormat renderTargetFormat;
        public TextureFormat depthStencilFormat;

        public RenderPipeline pipeline;

        public RenderResources renderResources;
        public FrameResources[] pFrameResources;
        public uint numFramesInFlight;
        public uint frameIndex;

    };

    /// <summary>
    /// Uniform structure
    /// </summary>
    internal struct Uniforms
    {
        public Matrix4x4 MVP;
        public float Gamma;
    }

    #endregion

    #region Shaders
    private const string __shader_vert_wgsl = @"
struct VertexInput
    {
        @location(0) position: vec2<f32>,
    @location(1) uv: vec2<f32>,
    @location(2) color: vec4<f32>,
};

    struct VertexOutput
    {
        @builtin(position) position: vec4<f32>,
    @location(0) color: vec4<f32>,
    @location(1) uv: vec2<f32>,
};

    struct Uniforms
    {
        mvp: mat4x4<f32>,
    gamma: f32,
};

    @group(0) @binding(0) var<uniform> uniforms: Uniforms;

@vertex
fn main(in: VertexInput) -> VertexOutput {
    var out: VertexOutput;
    out.position = uniforms.mvp* vec4<f32>(in.position, 0.0, 1.0);
    out.color = in.color;
    out.uv = in.uv;
    return out;
}
";

    private const string __shader_frag_wgsl = @"
    struct VertexOutput
    {
        @builtin(position) position: vec4<f32>,
        @location(0) color: vec4<f32>,
        @location(1) uv: vec2<f32>,
    };

    struct Uniforms
    {
        mvp: mat4x4<f32>,
        gamma: f32,
    };

    @group(0) @binding(0) var<uniform> uniforms: Uniforms;
    @group(0) @binding(1) var s: sampler;
    @group(1) @binding(0) var t: texture_2d<f32>;

    @fragment
    fn main(in: VertexOutput) -> @location(0) vec4<f32> {
        let color = in.color* textureSample(t, s, in.uv);
    let corrected_color = pow(color.rgb, vec3<f32>(uniforms.gamma));
        return vec4<f32>(corrected_color, color.a);
    }
    ";

    #endregion

    #region Helper functions

    // Backend data stored in io.BackendRendererUserData to allow support for multiple Dear ImGui contexts
    // It is STRONGLY preferred that you use docking branch with multi-viewports (== single Dear ImGui context + multiple windows) instead of multiple Dear ImGui contexts.
    unsafe static ref ImGui_ImplWGPU_Data GetBackendData()
    {   // TODO  implement this to support multi viewport. Current problem is suspected GC collection
        return ref data; // Unsafe.AsRef<ImGui_ImplWGPU_Data>((void*)ImGui.GetIO().BackendRendererUserData);
    }
    internal static T MEMALIGN<T>(T size, T align) where T : IBinaryInteger<T>
    {
        return (size + (align - T.One)) & ~(align - T.One);
    }

    public static nint GetTextureViewID(TextureView view)
    {
        return unchecked((nint)WebGPUMarshal.GetBorrowHandle(view).GetAddress());
    }

    #endregion

    #region Variables
    private bool disposedValue;

    private static ImGui_ImplWGPU_Data data; // Workaround
    #endregion

    /// <summary>
    /// Constructs a new ImGuiController.
    /// https://github.com/ocornut/imgui/blob/aa5a6098ee24ca30b3e0a180282619777e95fc67/backends/imgui_impl_wgpu.cpp#L724
    /// </summary>
    public ImGui_Impl_WebGPUSharp(in ImGui_ImplWGPU_InitInfo initInfo)
    {
        //
        var io = ImGui.GetIO();

        //
        if (io.BackendRendererUserData != 0)
        {
            Console.WriteLine("Already initialized backend");
            return;
        }


        // Setup backend capabilities flags
        nint bdPtr = Marshal.AllocHGlobal(Marshal.SizeOf<ImGui_ImplWGPU_Data>());
        io.BackendRendererUserData = bdPtr;
        //io.BackendRendererName ="imgui_impl_webgpu";
        // We can honor the ImDrawCmd::VtxOffset field, allowing for large meshes.
        io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;


        ref ImGui_ImplWGPU_Data bd = ref GetBackendData();
        bd.InitInfo = initInfo;
        bd.device = initInfo.device;
        bd.defaultQueue = bd.device.GetQueue()!;
        bd.renderTargetFormat = initInfo.rt_format;
        bd.depthStencilFormat = initInfo.depth_format;
        bd.numFramesInFlight = initInfo.num_frames_in_flight;
        bd.frameIndex = uint.MaxValue;


        // Create buffers with a default size (they will later be grown as needed)
        bd.pFrameResources = new FrameResources[bd.numFramesInFlight];
        for (int i = 0; i < bd.numFramesInFlight; i++)
        {
            ref FrameResources fr = ref bd.pFrameResources[i];
            fr.IndexBufferSize = 10000;
            fr.VertexBufferSize = 5000;
        }

    }

    #region Setup

    /// <summary>
    /// 
    /// </summary>
    public bool CreateDeviceObjects()
    {
        ref ImGui_ImplWGPU_Data bd = ref GetBackendData();
        if (bd.device == null)
            return false;

        BindGroupLayout[] bindGroupLayouts;

        // Create render pipeline
        {
            // Pipeline
            PipelineLayout layout;
            {
                var descriptor = new PipelineLayoutDescriptor()
                {
                    BindGroupLayouts = bindGroupLayouts =
                    [
                        bd.device.CreateBindGroupLayout(new ()
                        {
                            Label = "Bind Group Layout Common",
                            Entries = [
                                  new ()
                                  {
                                      Binding = 0,
                                      Visibility = ShaderStage.Vertex | ShaderStage.Fragment,
                                      Buffer = new(BufferBindingType.Uniform),
                                  },
                                new ()
                                {
                                    Binding = 1,
                                    Visibility = ShaderStage.Fragment,
                                    Sampler = new(SamplerBindingType.Filtering)
                                }
                            ]
                        })!,
                        bd.device.CreateBindGroupLayout(new ()
                        {
                            Label = "Bind Group Layout Image",
                            Entries = [
                                new ()
                                {
                                    Binding = 0,
                                    Visibility = ShaderStage.Fragment,
                                    Texture = new TextureBindingLayout(TextureSampleType.Float, TextureViewDimension.D2)
                                },
                            ]
                        })!
                    ]

                };

                layout = bd.device.CreatePipelineLayout(descriptor)!;
            }

            // Vertex shader
            VertexState vertexState;
            {
                ShaderModuleWGSLDescriptor vertexDescriptor = new() { Code = __shader_vert_wgsl };

                vertexState = new VertexState()
                {
                    Module = bd.device.CreateShaderModule(new ShaderModuleDescriptor(ref vertexDescriptor))!,
                    Buffers = [
                         new()
                         {
                             ArrayStride = (ulong)Marshal.SizeOf<ImGuiNET.ImDrawVert>(),
                             StepMode = VertexStepMode.Vertex,
                             Attributes =
                            [
                                new()
                                {
                                    Format = VertexFormat.Float32x2,
                                    Offset = (ulong)Marshal.OffsetOf<ImGuiNET.ImDrawVert>("pos"), // TODO fix: Causes GC
                                    ShaderLocation = 0,
                                },
                                new()
                                {
                                    Format = VertexFormat.Float32x2,
                                    Offset = (ulong)Marshal.OffsetOf<ImGuiNET.ImDrawVert>("uv"),
                                    ShaderLocation = 1,
                                },
                                new()
                                {
                                    Format = VertexFormat.Unorm8x4,
                                    Offset = (ulong)Marshal.OffsetOf<ImGuiNET.ImDrawVert>("col"),
                                    ShaderLocation = 2,
                                }
                            ]
                         },
                    ],
                };
            }

            // Fragment Shader
            FragmentState fragmentState;
            {
                ShaderModuleWGSLDescriptor fragmentDescriptor = new() { Code = __shader_frag_wgsl };

                fragmentState = new FragmentState()
                {
                    Module = bd.device.CreateShaderModule(new ShaderModuleDescriptor(ref fragmentDescriptor))!,
                    Targets =
                    [
                        new ColorTargetState()
                        {
                            Format = bd.renderTargetFormat,
                            Blend = new BlendState()
                            {
                                Alpha = new(BlendOperation.Add, BlendFactor.One, BlendFactor.OneMinusSrcAlpha),
                                Color = new(BlendOperation.Add, BlendFactor.SrcAlpha, BlendFactor.OneMinusSrcAlpha)
                            },
                            WriteMask = ColorWriteMask.All
                        }
                    ],
                };
            }

            // Depth Stencil. Default values. TODO set default so this is not needed
            DepthStencilState depthStencilState;
            {
                depthStencilState = new DepthStencilState()
                {
                    Format = bd.depthStencilFormat,
                    DepthWriteEnabled = false,
                    DepthCompare = CompareFunction.Always,

                    StencilFront = new StencilFaceState(
                        CompareFunction.Always,
                        StencilOperation.Keep,
                        StencilOperation.Keep,
                        StencilOperation.Keep),

                    StencilBack = new StencilFaceState(
                        CompareFunction.Always,
                        StencilOperation.Keep,
                        StencilOperation.Keep,
                        StencilOperation.Keep),
                };
            }


            var descriptor_pipeline = new RenderPipelineDescriptor()
            {
                Layout = layout,
                Primitive = new(
                    PrimitiveTopology.TriangleList,
                    IndexFormat.Undefined,
                    FrontFace.CW,
                    CullMode.None),
                Vertex = ref vertexState,
                Fragment = fragmentState,
                DepthStencil = bd.depthStencilFormat == TextureFormat.Undefined ? null : depthStencilState, // TODO set defaults not this is not needed
                Multisample = ref bd.InitInfo.PipelineMultisampleState
            };
            bd.pipeline = bd.device.CreateRenderPipeline(descriptor_pipeline)!;
        }


        // Create resources
        {
            RecreateFontDeviceTexture();
            CreateUniformBuffer();

            {
                bd.renderResources.CommonBindGroup = bd.device.CreateBindGroup(new BindGroupDescriptor()
                {
                    Label = "",
                    Entries = [
                        new BindGroupEntry()
                        {
                            Binding = 0,
                            Buffer = bd.renderResources.Uniforms,
                            Offset = 0,
                            Size = (ulong)MEMALIGN(Marshal.SizeOf<Uniforms>(),16),
                            Sampler = null,
                            TextureView = null
                        },
                        new BindGroupEntry()
                        {
                            Binding = 1,
                            Buffer = null,
                            Offset = 0,
                            Size = 0,
                            Sampler = bd.renderResources.FontTextureSampler,
                            TextureView = null
                        }
                    ],
                    Layout = bindGroupLayouts[0],
                })!;

                bd.renderResources.ImageBindGroup = ImGui_ImplWGPU_CreateImageBindGroup(bindGroupLayouts[1], bd.renderResources.FontTextureView);
                bd.renderResources.ImageBindGroupLayout = bindGroupLayouts[1];

                bd.renderResources.ImageBindGroups = new()
                {
                    {GetTextureViewID(bd.renderResources.FontTextureView), bd.renderResources.ImageBindGroup }
                };
            }
        }

        return true;
    }

    /// <summary>
    /// Recreates the device texture used to render text.
    /// </summary>
    public static void RecreateFontDeviceTexture()
    {
        ref ImGui_ImplWGPU_Data bd = ref GetBackendData();
        ImGuiIOPtr io = ImGui.GetIO();


        io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bytesPerPixel);


        // Upload texture to graphics system
        {
            TextureDescriptor texturedescriptor = new()
            {
                Label = "Dear ImGui Font Texture",
                Dimension = TextureDimension.D2,
                Size = new Extent3D((uint)width, (uint)height, 1),
                SampleCount = 1,
                Format = TextureFormat.RGBA8Unorm,
                MipLevelCount = 1,
                Usage = TextureUsage.CopyDst | TextureUsage.TextureBinding,
            };
            bd.renderResources.FontTexture = bd.device.CreateTexture(texturedescriptor)!;

            TextureViewDescriptor textureViewDescriptor = new()
            {
                Label = "",
                Format = TextureFormat.RGBA8Unorm,
                Dimension = TextureViewDimension.D2,
                BaseMipLevel = 0,
                MipLevelCount = 1,
                BaseArrayLayer = 0,
                ArrayLayerCount = 1,
                Aspect = TextureAspect.All
            };
            bd.renderResources.FontTextureView = bd.renderResources.FontTexture.CreateView(textureViewDescriptor)!;


            ImageCopyTexture destination = new()
            {
                Texture = bd.renderResources.FontTexture,
                MipLevel = 0,
                Origin = default,
                Aspect = TextureAspect.All
            };

            TextureDataLayout layout = new()
            {
                Offset = 0,
                BytesPerRow = (uint)width * (uint)bytesPerPixel,
                RowsPerImage = (uint)height,
            };
            Extent3D size = new((uint)width, (uint)height, 1);
            // TODO make safe
            unsafe
            {
                bd.defaultQueue.WriteTexture(destination, new Span<byte>((void*)pixels, width * height * bytesPerPixel), layout, size);
            }
        }

        // Create the associated sampler
        // (Bilinear sampling is required by default. Set 'io.Fonts->Flags |= ImFontAtlasFlags_NoBakedLines' or 'style.AntiAliasedLinesUseTex = false' to allow point/nearest sampling)
        {
            SamplerDescriptor descriptor = new()
            {
                MinFilter = FilterMode.Linear,
                MagFilter = FilterMode.Linear,
                MipmapFilter = MipmapFilterMode.Linear,
                AddressModeU = AddressMode.Repeat,
                AddressModeV = AddressMode.Repeat,
                AddressModeW = AddressMode.Repeat,
                MaxAnisotropy = 1,
            };

            bd.renderResources.FontTextureSampler = bd.device.CreateSampler(ref descriptor)!;
        }
        io.Fonts.SetTexID(GetTextureViewID(bd.renderResources.FontTextureView));

        io.Fonts.ClearTexData();
    }

    public static void CreateUniformBuffer()
    {
        ref ImGui_ImplWGPU_Data bd = ref GetBackendData();
        BufferDescriptor bufferDescriptor = new()
        {
            Label = "Dear ImGui Uniform Buffer",
            Size = (ulong)MEMALIGN(Marshal.SizeOf<Uniforms>(), 16),
            Usage = BufferUsage.CopyDst | BufferUsage.Uniform,
            MappedAtCreation = false,
        };
        bd.renderResources.Uniforms = bd.device.CreateBuffer(bufferDescriptor)!;
    }

    BindGroup ImGui_ImplWGPU_CreateImageBindGroup(BindGroupLayout layout, TextureView texture)
    {
        ref ImGui_ImplWGPU_Data bd = ref GetBackendData();

        BindGroupDescriptor image_bg_descriptor = new()
        {
            Layout = layout,
            Entries = [
                new BindGroupEntry()
                {
                    TextureView = texture,
                }
            ]
        };

        return bd.device.CreateBindGroup(image_bg_descriptor)!;
    }


    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="drawDataPtr"></param>
    /// <param name="ctx"></param>
    /// <param name="frameResources"></param>
    public static void SetupRenderState(ImDrawDataPtr drawDataPtr, RenderPassEncoder ctx, in FrameResources frameResources)
    {
        ref ImGui_ImplWGPU_Data bd = ref GetBackendData();

        // Setup orthographic projection matrix into our constant buffer
        // Our visible imgui space lies from draw_data->DisplayPos (top left) to draw_data->DisplayPos+data_data->DisplaySize (bottom right).
        {
            // Write Model View project matrix
            float L = drawDataPtr.DisplayPos.X;
            float R = drawDataPtr.DisplayPos.X + drawDataPtr.DisplaySize.X;
            float T = drawDataPtr.DisplayPos.Y;
            float B = drawDataPtr.DisplayPos.Y + drawDataPtr.DisplaySize.Y;
            //TODO validate matrix 
            // use Matrix4x4.CreateOrthographicOffCenter
            Matrix4x4 mvp = new(
                2.0f / (R - L), 0.0f, 0.0f, 0.0f,
                0.0f, 2.0f / (T - B), 0.0f, 0.0f,
                0.0f, 0.0f, 0.5f, 0.0f,
                (R + L) / (L - R), (T + B) / (B - T), 0.5f, 1.0f
            );

            // Upload Gamma
            float gamma;
            switch (bd.renderTargetFormat)
            {
                case TextureFormat.ASTC10x10UnormSrgb:
                case TextureFormat.ASTC10x5UnormSrgb:
                case TextureFormat.ASTC10x6UnormSrgb:
                case TextureFormat.ASTC10x8UnormSrgb:
                case TextureFormat.ASTC12x10UnormSrgb:
                case TextureFormat.ASTC12x12UnormSrgb:
                case TextureFormat.ASTC4x4UnormSrgb:
                case TextureFormat.ASTC5x5UnormSrgb:
                case TextureFormat.ASTC6x5UnormSrgb:
                case TextureFormat.ASTC6x6UnormSrgb:
                case TextureFormat.ASTC8x5UnormSrgb:
                case TextureFormat.ASTC8x6UnormSrgb:
                case TextureFormat.ASTC8x8UnormSrgb:
                case TextureFormat.BC1RGBAUnormSrgb:
                case TextureFormat.BC2RGBAUnormSrgb:
                case TextureFormat.BC3RGBAUnormSrgb:
                case TextureFormat.BC7RGBAUnormSrgb:
                case TextureFormat.BGRA8UnormSrgb:
                case TextureFormat.ETC2RGB8A1UnormSrgb:
                case TextureFormat.ETC2RGB8UnormSrgb:
                case TextureFormat.ETC2RGBA8UnormSrgb:
                case TextureFormat.RGBA8UnormSrgb:
                    gamma = 2.2f;
                    break;
                default:
                    gamma = 1.0f;
                    break;
            }

            bd.defaultQueue.WriteBuffer(bd.renderResources.Uniforms, 0, new Uniforms() { MVP = mvp, Gamma = gamma});
        }

        // Setup viewport
        ctx.SetViewport(0, 0, (uint)(drawDataPtr.FramebufferScale.X * drawDataPtr.DisplaySize.X), (uint)(drawDataPtr.FramebufferScale.Y * drawDataPtr.DisplaySize.Y), 0, 1);

        // Bind shader and vertex buffers
        ctx.SetVertexBuffer(0, frameResources.VertexBuffer, 0, (ulong)(frameResources.VertexBufferSize * Marshal.SizeOf<ImDrawVert>()));
        ctx.SetIndexBuffer(frameResources.IndexBuffer, Marshal.SizeOf<ImDrawIdx>() == 2 ? IndexFormat.Uint16 : IndexFormat.Uint32, 0, (ulong)(frameResources.IndexBufferSize * Marshal.SizeOf<ImDrawIdx>()));
        ctx.SetPipeline(bd.pipeline);
        ctx.SetBindGroup(0, bd.renderResources.CommonBindGroup);

        // Setup blend factor
        WebGpuSharp.Color blendcolor = new(0, 0, 0, 0);
        ctx.SetBlendConstant(blendcolor);
    }

    /// <summary>
    /// Render Function for IMGUI
    /// </summary>
    /// <param name="draw_data"></param>
    /// <param name="renderPassEncoder"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void RenderDrawData(ImDrawDataPtr draw_data, WebGpuSharp.RenderPassEncoder renderPassEncoder)
    {
        // Framebuffer size
        int fb_width = (int)(draw_data.DisplaySize.X * draw_data.FramebufferScale.X);
        int fb_height = (int)(draw_data.DisplaySize.Y * draw_data.FramebufferScale.Y);

        // Avoid rendering when minimized
        if (fb_width <= 0.0f || fb_height <= 0.0f || draw_data.CmdListsCount == 0)
            return;


        ref ImGui_ImplWGPU_Data bd = ref GetBackendData();
        // Advance the frame index
        bd.frameIndex++;
        ref FrameResources fr = ref bd.pFrameResources[bd.frameIndex % bd.numFramesInFlight];


        // Create and grow vertex/index buffers & hosts if needed
        {
            if (fr.VertexBuffer == null || fr.VertexBufferSize < draw_data.TotalVtxCount)
            {
                // The old vertexbuffer will get released automatically thanks to the WGPU implmentation
                // Thanks to EmilSV

                // Grow in 5000 increments
                // Todo set manual start size
                fr.VertexBufferSize = draw_data.TotalVtxCount + 5000;

                BufferDescriptor vb_descriptor = new()
                {
                    Label = "Dear ImGui Vertex Buffer",
                    Size = (ulong)MEMALIGN(Marshal.SizeOf<ImDrawVert>() * fr.VertexBufferSize, 4),
                    Usage = BufferUsage.CopyDst | BufferUsage.Vertex,
                    MappedAtCreation = false,
                };
                fr.VertexBuffer = bd.device.CreateBuffer(vb_descriptor)!;

                fr.VertexBufferHost = new ImDrawVert[fr.VertexBufferSize];


            }
            if (fr.IndexBuffer == null || fr.IndexBufferSize < draw_data.TotalIdxCount)
            {
                fr.IndexBufferSize = draw_data.TotalIdxCount + 10000;

                BufferDescriptor ib_desc = new()
                {
                    Label = "Dear ImGui Index Buffer",
                    Size = (ulong)MEMALIGN(Marshal.SizeOf<ImDrawIdx>() * fr.IndexBufferSize, 4),
                    Usage = BufferUsage.CopyDst | BufferUsage.Index,
                    MappedAtCreation = false,
                };
                fr.IndexBuffer = bd.device.CreateBuffer(ib_desc)!;

                fr.IndexBufferHost = new ImDrawIdx[fr.IndexBufferSize];
            }
        }

        // Upload vertex/index data into a single contiguous GPU buffer
        unsafe
        {
            Span<ImDrawVert> vtx_dst = new(fr.VertexBufferHost);
            Span<ImDrawIdx> idx_dst = new(fr.IndexBufferHost);
            int vtx_count = 0;
            int idx_count = 0;

            for (int n = 0; n < draw_data.CmdListsCount; n++)
            {
                ImDrawListPtr cmd_list = draw_data.CmdLists[n];
                ReadOnlySpan<ImDrawVert> vtx_source = new((void*)cmd_list.VtxBuffer.Data, cmd_list.VtxBuffer.Size);
                ReadOnlySpan<ImDrawIdx> idx_source = new((void*)cmd_list.IdxBuffer.Data, cmd_list.IdxBuffer.Size);

                vtx_source.CopyTo(new Span<ImDrawVert>(fr.VertexBufferHost, vtx_count, cmd_list.VtxBuffer.Size));
                idx_source.CopyTo(new Span<ImDrawIdx>(fr.IndexBufferHost, idx_count, cmd_list.IdxBuffer.Size));

                vtx_count += cmd_list.VtxBuffer.Size;
                idx_count += cmd_list.IdxBuffer.Size;
            }
            bd.defaultQueue.WriteBuffer(fr.VertexBuffer, 0, vtx_dst[..MEMALIGN(vtx_count, 4)]);
            bd.defaultQueue.WriteBuffer(fr.IndexBuffer, 0, idx_dst[..MEMALIGN(idx_count, 4)]);
        }

        // Setup desired render state
        SetupRenderState(draw_data, renderPassEncoder, fr);

        // Render command lists
        // (Because we merged all buffers into a single one, we maintain our own offset into them)
        {
            int global_vtx_offset = 0;
            int global_idx_offset = 0;
            Vector2 clip_scale = draw_data.FramebufferScale;
            Vector2 clip_off = draw_data.DisplayPos;

            for (int n = 0; n < draw_data.CmdListsCount; n++)
            {
                ImDrawListPtr cmd_list = draw_data.CmdLists[n];

                for (int cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++)
                {
                    ImDrawCmdPtr pcmd = cmd_list.CmdBuffer[cmd_i];
                    if (pcmd.UserCallback != 0)
                    {
                        // User callback, registered via ImDrawList::AddCallback()
                        // (ImDrawCallback_ResetRenderState is a special callback value used by the user to request the renderer to reset render state.)
                        if (pcmd.UserCallback == -8) // TODO replace with enum
                            SetupRenderState(draw_data, renderPassEncoder, fr);
                        else
                        {
                            var callback = Marshal.GetDelegateForFunctionPointer<UserCallbackDelegate>(pcmd.UserCallback);
                            callback.Invoke(cmd_list, pcmd);
                        }
                    }
                    else
                    {
                        // Bind custom texture
                        IntPtr tex_id = pcmd.GetTexID();
                        UIntPtr u = unchecked((nuint)tex_id);

                        bd.renderResources.ImageBindGroups.TryGetValue(tex_id, out BindGroup? bind_group);

                        if (bind_group != null)
                        {
                            renderPassEncoder.SetBindGroup(1, bind_group);
                        }
                        else
                        {
                            TextureView tex = new TextureViewHandle(u).ToSafeHandle(false)!;
                            BindGroup image_bind_group = ImGui_ImplWGPU_CreateImageBindGroup(bd.renderResources.ImageBindGroupLayout, tex);

                            bd.renderResources.ImageBindGroups.Add(tex_id, image_bind_group);

                            renderPassEncoder.SetBindGroup(1, image_bind_group);
                        }

                        // Project scissor/clipping rectangles into framebuffer space
                        Vector2 clip_min = new((pcmd.ClipRect.X - clip_off.X) * clip_scale.X, (pcmd.ClipRect.Y - clip_off.Y) * clip_scale.Y);
                        Vector2 clip_max = new((pcmd.ClipRect.Z - clip_off.X) * clip_scale.X, (pcmd.ClipRect.W - clip_off.Y) * clip_scale.Y);

                        // Clamp to viewport as wgpuRenderPassEncoderSetScissorRect() won't accept values that are off bounds
                        if (clip_min.X < 0.0f) { clip_min.X = 0.0f; }
                        if (clip_min.Y < 0.0f) { clip_min.Y = 0.0f; }
                        if (clip_max.X > fb_width) { clip_max.X = (float)fb_width; }
                        if (clip_max.Y > fb_height) { clip_max.Y = (float)fb_height; }
                        if (clip_max.X <= clip_min.X || clip_max.Y <= clip_min.Y)
                            continue;

                        // Apply scissor/clipping rectangle, Draw
                        renderPassEncoder.SetScissorRect((uint)clip_min.X, (uint)clip_min.Y, (uint)(clip_max.X - clip_min.X), (uint)(clip_max.Y - clip_min.Y));
                        renderPassEncoder.DrawIndexed(pcmd.ElemCount, 1, (uint)pcmd.IdxOffset + (uint)global_idx_offset, (int)pcmd.VtxOffset + (int)global_vtx_offset, 0);
                    }
                }
                global_idx_offset += cmd_list.IdxBuffer.Size;
                global_vtx_offset += cmd_list.VtxBuffer.Size;
            }
        }
    }

    /// <summary>
    /// reevaluaate the need for this
    /// </summary>
    public void NewFrame()
    {
        ref ImGui_ImplWGPU_Data bd = ref GetBackendData();
        if (bd.pipeline == null)
            CreateDeviceObjects();
    }

    #region Disposal

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
            Marshal.FreeHGlobal(ImGui.GetIO().BackendRendererUserData);
        }
    }

    // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~ImGui_Impl_WebGPUSharp()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}