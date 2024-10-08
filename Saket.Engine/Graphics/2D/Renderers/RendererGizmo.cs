
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using WebGpuSharp;

namespace Saket.Engine.Graphics.D2.Renderers;

public class RendererGizmo
{
    #region Variables

    List<Vertex2D> verticies;
    protected WebGpuSharp.Buffer buffer_vertex;
    protected ulong sizeInBytes_bufferVertex;

    protected GraphicsContext graphics;

    public RenderPipeline pipeline;

    public static RenderPipeline pipeline_default;
    public static BindGroupLayout[] bindGroupLayouts_default;
    #endregion

    #region Constructors

    /// <param name="graphics"></param>
    /// <param name="maximumBatchCount"> The maximum number of sprites to allocate for a single batch. Increasing this number will increase memory usage but can lower drawcalls. Set according to your games largest batch. </param>
    public RendererGizmo(GraphicsContext graphics, RenderPipeline pipeline = null)
    {
        this.graphics = graphics;
        this.pipeline = pipeline;
        this.verticies = new List<Vertex2D>();
        if (pipeline != null)
            return;

        if (pipeline_default != null)
        {
            this.pipeline = pipeline_default;
            return;
        }

        // Create render pipeline
        {
            // Pipeline
            PipelineLayout layout;
            {
                var descriptor = new PipelineLayoutDescriptor()
                {
                    BindGroupLayouts = bindGroupLayouts_default =
                    [
                        graphics.systemBindGroupLayout
                    ]
                };

                layout = graphics.device.CreatePipelineLayout(descriptor)!;
            }

            ShaderModule shader = null;
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "Saket.Engine.Resources.shader_renderer_gizmo.wgsl";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();

                    ShaderModuleWGSLDescriptor descriptor = new() { Code = result };
                    shader = graphics.device.CreateShaderModule(new ShaderModuleDescriptor(ref descriptor))!;
                }
            }

            VertexState vertexState;
            {
                vertexState = new VertexState()
                {
                    Module = shader,
                    Buffers = [
                        new()
                        {
                            ArrayStride = (ulong)Marshal.SizeOf<Vertex2D>(),
                            StepMode = VertexStepMode.Vertex,
                            Attributes =
                            [
                                new()
                                {
                                    Format = VertexFormat.Float32x2,
                                    Offset = (ulong)Marshal.OffsetOf<Vertex2D>("pos"),
                                    ShaderLocation = 0,
                                },
                                new()
                                {
                                    Format = VertexFormat.Float32x2,
                                    Offset = (ulong)Marshal.OffsetOf<Vertex2D>("uv"),
                                    ShaderLocation = 1,
                                },
                                new()
                                {
                                    Format = VertexFormat.Unorm8x4,
                                    Offset = (ulong)Marshal.OffsetOf<Vertex2D>("col"),
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
                fragmentState = new FragmentState()
                {
                    Module = shader,
                    Targets =
                    [
                        new ColorTargetState()
                        {
                            Format = graphics.applicationpreferredFormat,
                            Blend = new BlendState()
                            {
                                Alpha = new(){
                                    Operation = BlendOperation.Add,
                                    SrcFactor = BlendFactor.One,
                                    DstFactor = BlendFactor.OneMinusSrcAlpha
                                },
                                Color = new(){
                                    Operation = BlendOperation.Add,
                                    SrcFactor = BlendFactor.SrcAlpha,
                                    DstFactor = BlendFactor.OneMinusSrcAlpha
                                },
                            },
                            WriteMask = ColorWriteMask.All
                        }
                    ],
                };
            }


            MultisampleState multisampleState = new();

            var descriptor_pipeline = new RenderPipelineDescriptor()
            {
                Layout = layout,
                Primitive = new()
                {
                    Topology = PrimitiveTopology.TriangleList,
                    StripIndexFormat = IndexFormat.Undefined,
                    FrontFace = FrontFace.CW,
                    CullMode = CullMode.None
                },
                Vertex = ref vertexState,
                Fragment = fragmentState,
                DepthStencil = null,
                Multisample = ref multisampleState,
            };
            this.pipeline = pipeline_default = graphics.device.CreateRenderPipeline(descriptor_pipeline)!;
        }

    }

    static RendererGizmo()
    {

    }


    #endregion

    #region Immediate mode API

    // TODO(Soren): create draw overloads that doesn't use propriatary structs
    public void Draw(IEnumerable<Vertex2D> verticies)
    {
        this.verticies.AddRange(verticies);
    }

    public void RenderPass(CommandEncoder commandEncoder, BindGroup systemBindGroup, RenderPassDescriptor renderPassDescriptor)
    {
        if (verticies.Count <= 0)
            return;

        // Vertex buffer 
        {
            var newsize = (ulong)(verticies.Count * Marshal.SizeOf<Vertex2D>());

            if (sizeInBytes_bufferVertex < newsize)
            {
                sizeInBytes_bufferVertex = newsize;

                BufferDescriptor bufferDescriptor = new()
                {
                    Usage = BufferUsage.CopyDst | BufferUsage.Vertex,
                    Size = newsize,
                    Label = "buffer_renderer_gizmo"
                };

                buffer_vertex = graphics.device.CreateBuffer(bufferDescriptor)!;
            }

            graphics.queue.WriteBuffer(buffer_vertex, 0, verticies);
        }

        var RenderPassEncoder = commandEncoder.BeginRenderPass(renderPassDescriptor);

        // Set the pipline for the renderpass
        RenderPassEncoder.SetPipeline(pipeline);
        
        // Set system bind group
        RenderPassEncoder.SetBindGroup(0, systemBindGroup);

        // set vertex buffers and Submit actual draw comand
        RenderPassEncoder.SetVertexBuffer(0, buffer_vertex, 0, sizeInBytes_bufferVertex);
        RenderPassEncoder.Draw((uint)verticies.Count, 1, 0, 0);

        // Finish Rendering
        RenderPassEncoder.End();

        verticies.Clear();
    }

    public void ClearBatch()
    {
        verticies.Clear();
    }

    #endregion
}