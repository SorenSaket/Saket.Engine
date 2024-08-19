using System.Numerics;
using System.Runtime.InteropServices;

using WebGpuSharp;
using Saket.Engine.Graphics;
using StbImageSharp;
using Saket.ECS;
using Saket.Engine;

namespace TestApp;

/// <summary>
/// 
/// </summary>
public class RendererMesh
{

    #region Structures

    internal class InternalMesh
    {
        public int IndexBufferSize;
        public int VertexBufferSize;

        public WebGpuSharp.Buffer buffer_vertex;
        public WebGpuSharp.Buffer buffer_index;

        public uint material;
    }
    internal class InternalMaterial
    {
        public InternalTexture texture_base;
    }
    // TODO unify with saket.engine.image
    internal class InternalTexture
    {
        // Font texture
        public Texture Texture;
        // Texture view for font texture
        public TextureView TextureView;
        // Sampler for the font texture
        public Sampler Sampler;
        // 
        public BindGroup BindGroup;
    }


    public struct Settings
    {
        public ulong bufferSizeInBytes;
        public ulong instanceCount;
        public Settings()
        {
            bufferSizeInBytes = 268435456;
            instanceCount = 1024;
        }
    }

    internal static T MEMALIGN<T>(T size, T align) where T : IBinaryInteger<T>
    {
        return (size + (align - T.One)) & ~(align - T.One);
    }
    public struct Vertex
    {
        public Vector3 pos;

        public Vector2 uv;

        public uint col;
    }

    public struct Instance
    {
        public System.Numerics. Matrix4x4 model;
    }
    /// <summary>
    /// Uniform structure
    /// </summary>
    internal struct Uniforms
    {
        public Matrix4x4 ViewProjectionMatrix;
    }
    #endregion

    #region Shaders
    private const string __shader_vert_wgsl = @"
        struct VertexInput
        {
            @location(0) position: vec3<f32>,
            @location(1) uv: vec2<f32>,
            @location(2) color: vec4<f32>,
        };
        struct InstanceInput {
            @location(3) model_matrix_0 : vec4<f32>,
            @location(4) model_matrix_1 : vec4<f32>,
            @location(5) model_matrix_2 : vec4<f32>,
            @location(6) model_matrix_3 : vec4<f32>,
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
        };

        @group(0) @binding(0) var<uniform> uniforms: Uniforms;

        @vertex
        fn main(in: VertexInput,  instance: InstanceInput) -> VertexOutput {
            let model_matrix = mat4x4<f32>(
                instance.model_matrix_0,
                instance.model_matrix_1,
                instance.model_matrix_2,
                instance.model_matrix_3,
            );
            var out: VertexOutput;
            out.position = uniforms.mvp * vec4<f32>(in.position, 1.0);
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
        };

        @group(0) @binding(0) var<uniform> uniforms: Uniforms;
        @group(0) @binding(1) var s: sampler;
        @group(1) @binding(0) var t: texture_2d<f32>;

        @fragment
        fn main(in: VertexOutput) -> @location(0) vec4<f32> {
            let color = in.color * textureSample(t, s, in.uv);
            return color;
        }
        ";

    #endregion

    #region Variables

    // Render
    RenderPipeline renderpipeline;

    Instance[] instanceHost;
    WebGpuSharp.Buffer buffer_instance;

    WebGpuSharp.Buffer buffer_uniforms;

    BindGroupLayout bindgroupLayout_image;
    BindGroup bindgroup;
    InternalTexture nullTex;

    Settings settings;

    GraphicsContext cxt;


    uint counter_meshID;
    Dictionary<uint, InternalMesh> meshes;

    uint counter_matID = 0;
    Dictionary<uint, InternalMaterial> materials;

    Dictionary<string, InternalTexture> textures;
    #endregion

    public RendererMesh(GraphicsContext cxt, Settings settings)
    {
        this.cxt = cxt;
        this.settings = settings;
        
        meshes = new();
        materials = new();
        textures = new();
        instanceHost = new Instance[settings.instanceCount];
        BindGroupLayout[] bindGroupLayouts;
      
        // Create Render Pipeline
        {
            // pipeline Layout
            PipelineLayout layout = cxt.device.CreatePipelineLayout(new()
            {
                BindGroupLayouts = bindGroupLayouts = [
                    cxt.device.CreateBindGroupLayout(new()
                    {
                        Label = "Bind Group Layout Common",
                        Entries = [
                            new()
                            {
                                Binding = 0,
                                Visibility = ShaderStage.Vertex | ShaderStage.Fragment,
                                Buffer = new(BufferBindingType.Uniform),
                            },
                            new BindGroupLayoutEntry()
                            {
                                Binding = 1,
                                Visibility = ShaderStage.Fragment,
                                Sampler = new(SamplerBindingType.Filtering)
                            }
                        ]
                    })!,
                    cxt.device.CreateBindGroupLayout(new()
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
                    })!,
                ]
            })!;

            VertexState vertexState;
            {
                var vertexDescriptor = new ShaderModuleWGSLDescriptor() { Code = __shader_vert_wgsl };

                vertexState = new VertexState()
                {
                    Module = cxt.device.CreateShaderModule(new ShaderModuleDescriptor(ref vertexDescriptor))!,
                    Buffers = [
                            new()
                            {
                                ArrayStride = (ulong)Marshal.SizeOf<Vertex>(),
                                StepMode = VertexStepMode.Vertex,
                                Attributes =
                                [
                                    new()
                                    {
                                        Format = VertexFormat.Float32x3,
                                        Offset = (ulong)Marshal.OffsetOf<Vertex>("pos"), // TODO fix: Causes GC
                                        ShaderLocation = 0,
                                    },
                                    new()
                                    {
                                        Format = VertexFormat.Float32x2,
                                        Offset = (ulong)Marshal.OffsetOf<Vertex>("uv"),
                                        ShaderLocation = 1,
                                    },
                                    new()
                                    {
                                        Format = VertexFormat.Unorm8x4,
                                        Offset = (ulong)Marshal.OffsetOf<Vertex>("col"),
                                        ShaderLocation = 2,
                                    }
                                ]
                            },
                        new()
                        {
                            ArrayStride = (ulong)Marshal.SizeOf<Instance>(),
                            StepMode = VertexStepMode.Instance,
                            Attributes =
                                [
                                    new()
                                    {
                                        Format = VertexFormat.Float32x4,
                                        Offset = 0, 
                                        ShaderLocation = 3,
                                    },
                                    new()
                                    {
                                        Format = VertexFormat.Float32x4,
                                        Offset = 16,
                                        ShaderLocation = 4,
                                    },
                                    new()
                                    {
                                        Format = VertexFormat.Float32x4,
                                        Offset = 32,
                                        ShaderLocation = 5,
                                    },
                                    new()
                                    {
                                        Format = VertexFormat.Float32x4,
                                        Offset = 48,
                                        ShaderLocation = 6,
                                    }
                                ]
                        },
                    ],
                };
            }

            FragmentState fragmentState;
            {
                var fragmentDescriptor = new ShaderModuleWGSLDescriptor() { Code = __shader_frag_wgsl };

                fragmentState = new FragmentState()
                {
                    Module = cxt.device.CreateShaderModule(new ShaderModuleDescriptor(ref fragmentDescriptor))!,
                    Targets =
                    [
                        new ColorTargetState()
                        {
                            Format = cxt.applicationpreferredFormat,
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
                    Format = TextureFormat.Depth32Float,
                    DepthWriteEnabled = true,
                    DepthCompare = CompareFunction.Less,

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

            MultisampleState multisampleState = new()
            {
                Count = 1,
                Mask = uint.MaxValue,
                AlphaToCoverageEnabled = false
            };


            var descriptor_pipeline = new RenderPipelineDescriptor()
            {
                Layout = layout,
                Primitive = new(
                        PrimitiveTopology.TriangleList,
                        IndexFormat.Undefined,
                        FrontFace.CCW,
                        CullMode.None),
                Vertex = ref vertexState,
                Fragment = fragmentState,
                DepthStencil = depthStencilState, // TODO set defaults not this is not needed
                Multisample = ref multisampleState
            };
            renderpipeline = cxt.device.CreateRenderPipeline(descriptor_pipeline)!;
        }

        // Upload buffers
        { 
            // Uniform Buffer
            BufferDescriptor bufferDescriptor = new()
            {
                Label = "MeshRenderer Uniform Buffer",
                Size = (ulong)MEMALIGN(Marshal.SizeOf<Uniforms>(), 16),
                Usage = BufferUsage.CopyDst | BufferUsage.Uniform,
                MappedAtCreation = false,
            };
            buffer_uniforms = cxt.device.CreateBuffer(bufferDescriptor)!;

          
            // Instance
            BufferDescriptor instancebuffer_desc = new()
            {
                Label = "MeshRenderer Instance Buffer",
                Size = ((ulong)Marshal.SizeOf<Instance>() * settings.instanceCount),
                Usage = BufferUsage.CopyDst | BufferUsage.Vertex,
                MappedAtCreation = false,
            };
            buffer_instance = cxt.device.CreateBuffer(instancebuffer_desc)!;
        }

        // Create bindgroups
        bindgroup = cxt.device.CreateBindGroup(new ()
        {
            Label = "MeshRenderer BindGroup",
            Entries = [
                new ()
                {
                    Binding = 0,
                    Buffer = buffer_uniforms,
                    Offset = 0,
                    Size = (ulong)MEMALIGN(Marshal.SizeOf<Uniforms>(), 16),
                    Sampler = null,
                    TextureView = null
                },
                new()
                {
                    Binding = 1,
                    Buffer = null,
                    Offset = 0,
                    Size = 0,
                    Sampler = cxt.defaultSampler,
                    TextureView = null
                },
            ],
            Layout = bindGroupLayouts[0],
        })!;
      
        bindgroupLayout_image = bindGroupLayouts[1];

        nullTex = UploadTexture([255,0,128,255, 255, 0, 128, 255, 255, 0, 128, 255, 255, 0, 128, 255] , 2, 2, "nulltex");
    }


    Query query_meshTransform = new Query().With<Transform>().With<Mesh>();

    public void RenderWorld(World world, RenderPassEncoder encoder, System.Numerics.Matrix4x4 vp)
    {
        Dictionary<uint, List<ECSPointer>> counts = new ();

        var entities = world.Query(query_meshTransform);
        foreach (var entity in entities)
        {
            var mesh = entity.Get<Mesh>();
            if (counts.ContainsKey(mesh.id))
                counts[mesh.id].Add(entity.EntityPointer);
            else
                counts.Add(mesh.id, new List<ECSPointer>([entity.EntityPointer]));
        }

        // Iterate different meshes
        foreach (var mesh in counts)
        {
            for (int i = 0; i < mesh.Value.Count; i++)
            {
                var transform = world.GetEntity(mesh.Value[i]).Get<Transform>();
                instanceHost[i].model = transform.TRS();
            }

            Render(encoder, vp, mesh.Key, instanceHost.AsSpan().Slice(0, mesh.Value.Count));
        }

    }


    public void Render(RenderPassEncoder encoder, System.Numerics.Matrix4x4 vp, uint meshID, Span<Instance> instances)
    {
        if (!meshes.ContainsKey(meshID))
            throw new Exception("Mesh doesnt exsist");

        cxt.queue.WriteBuffer(buffer_instance, 0, instances);
        cxt.queue.WriteBuffer(buffer_uniforms, 0, vp);

        InternalMesh m = meshes[meshID];

        // Setup viewport
        encoder.SetViewport(0, 0, 1280,720, 0, 1);

        // Bind shader and vertex buffers
        encoder.SetVertexBuffer(0, m.buffer_vertex, 0,   (ulong)( m.VertexBufferSize * Marshal.SizeOf<Vertex>()));
        encoder.SetVertexBuffer(1, buffer_instance, 0,  (ulong)(instances.Length * Marshal.SizeOf<Instance>()));

        encoder.SetIndexBuffer(m.buffer_index, IndexFormat.Uint16, 0, (ulong)(m.IndexBufferSize *2));

        encoder.SetPipeline(renderpipeline);
        encoder.SetBindGroup(0, bindgroup);
        if(materials[m.material].texture_base != null)
            encoder.SetBindGroup(1, materials[m.material].texture_base.BindGroup);
        else
            encoder.SetBindGroup(1, nullTex.BindGroup);
        // Setup blend factor
        WebGpuSharp.Color blendcolor = new(0, 0, 0, 0);
        encoder.SetBlendConstant(blendcolor);

        // TODO, webgpusharp,  draw indexed optional parameters, https://developer.mozilla.org/en-US/docs/Web/API/GPURenderPassEncoder/drawIndexed
        encoder.DrawIndexed((uint)m.IndexBufferSize, (uint)instances.Length, 0,0,0);
    }

    public uint[] UploadScene(Assimp.Scene scene, string path)
    {
        // Upload all the materials
        uint[] matindexes = new uint[scene.MaterialCount];

        for (int i = 0; i < scene.MaterialCount; i++)
        {
            matindexes[i] = UploadMaterial(scene.Materials[i],  path);
        }

        uint[] meshIDS = new uint[scene.MeshCount];

        // Upload all the meshes
        for (int m = 0; m < scene.Meshes.Count; m++)
        {
            if (!scene.Meshes[m].HasVertices)
                continue;

            // copy verticies to a single buffer
            RendererMesh.Vertex[] verts = new RendererMesh.Vertex[scene.Meshes[m].Vertices.Count];
            {
                // 
                for (int k = 0; k < scene.Meshes[m].Vertices.Count; k++)
                {
                    verts[k].pos.X = scene.Meshes[m].Vertices[k].X;
                    verts[k].pos.Y = scene.Meshes[m].Vertices[k].Y;
                    verts[k].pos.Z = scene.Meshes[m].Vertices[k].Z;
                }

                // Index colors
                if (scene.Meshes[m].HasVertexColors(0))
                {
                    for (int k = 0; k < scene.Meshes[m].VertexColorChannels[0].Count; k++)
                    {
                        verts[k].col = FromFloatRGBA(
                             scene.Meshes[m].VertexColorChannels[0][k].R,
                              scene.Meshes[m].VertexColorChannels[0][k].G,
                               scene.Meshes[m].VertexColorChannels[0][k].B,
                            scene.Meshes[m].VertexColorChannels[0][k].A
                            );
                    }

                }
                else
                {
                    for (int k = 0; k < verts.Length; k++)
                    {
                        verts[k].col = uint.MaxValue;
                    }
                }
                // UVS
                if (scene.Meshes[m].HasTextureCoords(0))
                {
                    for (int k = 0; k < scene.Meshes[m].TextureCoordinateChannels[0].Count; k++)
                    {
                        verts[k].uv.X = scene.Meshes[m].TextureCoordinateChannels[0][k].X;
                        verts[k].uv.Y = scene.Meshes[m].TextureCoordinateChannels[0][k].Y;
                    }

                }
            }
      
            // Copy indicies to a single buffer
            ushort[] indicies = new ushort[scene.Meshes[m].FaceCount * 3];
            {
                int indiciIndex = 0;
                if (scene.Meshes[m].HasFaces)
                {
                    for (int f = 0; f < scene.Meshes[m].FaceCount; f++)
                    {
                        // This should never run since the model should be triangulated
                        if (scene.Meshes[m].Faces[f].IndexCount != 3)
                        {
                            continue;
                        }

                        for (int i = 0; i < scene.Meshes[m].Faces[f].IndexCount; i++)
                        {
                            indicies[indiciIndex] = ((ushort)scene.Meshes[m].Faces[f].Indices[i]);
                            indiciIndex++;
                        }
                    }
                }

            }

            // Vertex 
            BufferDescriptor vb_descriptor = new()
            {
                Label = "MeshRenderer Vertex Buffer",
                Size = (ulong)(Marshal.SizeOf<Vertex>()* verts.Length),
                Usage = BufferUsage.CopyDst | BufferUsage.Vertex,
                MappedAtCreation = false,
            };
            var buffer_vertex = cxt.device.CreateBuffer(vb_descriptor)!;
            cxt.queue.WriteBuffer(buffer_vertex, 0, verts);
            // Index
            BufferDescriptor ib_desc = new()
            {
                Label = "MeshRenderer Index Buffer",
                Size = (ulong)(2 * indicies.Length),
                Usage = BufferUsage.CopyDst | BufferUsage.Index,
                MappedAtCreation = false,
            };
            var buffer_index = cxt.device.CreateBuffer(ib_desc)!;
            cxt.queue.WriteBuffer(buffer_index, 0, indicies);

            meshes.Add(counter_meshID, new InternalMesh()
            {
                IndexBufferSize = indicies.Length,
                VertexBufferSize = verts.Length,
                buffer_index = buffer_index,
                buffer_vertex = buffer_vertex,
                material = matindexes[scene.Meshes[m].MaterialIndex]
            });
            meshIDS[m] = counter_meshID;
            counter_meshID++;
        }
        return meshIDS;
    }


    static uint FromFloatRGBA(float r, float g, float b, float a)
    {
        // Clamp the float values between 0.0 and 1.0
        r = Math.Clamp(r, 0.0f, 1.0f);
        g = Math.Clamp(g, 0.0f, 1.0f);
        b = Math.Clamp(b, 0.0f, 1.0f);
        a = Math.Clamp(a, 0.0f, 1.0f);

        // Convert to 8-bit (0-255) range
        byte r8 = (byte)(r * 255.0f);
        byte g8 = (byte)(g * 255.0f);
        byte b8 = (byte)(b * 255.0f);
        byte a8 = (byte)(a * 255.0f);

        // Pack the values into a 32-bit integer
        return (uint)(r8 << 24 | g8 << 16 | b8 << 8 | a8);
    }

    internal uint UploadMaterial(Assimp.Material material, string path)
    {
        InternalMaterial mat = new InternalMaterial();
        
        // If has base texture
        if(material.HasTextureDiffuse)
        {
            StbImage.stbi_set_flip_vertically_on_load(1);
            mat.texture_base = UploadTexture(Path.Combine(Path.GetDirectoryName(path)!, material.TextureDiffuse.FilePath));
        }

        materials[counter_matID] = mat;
        counter_matID++;
        return counter_matID -1; 
    }

    internal InternalTexture UploadTexture(string filepath)
    {
        if(textures.ContainsKey(filepath))
            return textures[filepath];
        // Load the image using system.io Filestream and stb image sharp
        ImageResult result;
        using (FileStream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
        {
             result = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        }
        
        // convert from rgba to bgra
        for (int i = 0; i < result.Width * result.Height; ++i)
        {
            byte temp = result.Data[i * 4];
            result.Data[i * 4] = result.Data[i * 4 + 2];
            result.Data[i * 4 + 2] = temp;
        }

        return UploadTexture(result.Data, (uint)result.Width, (uint)result.Height, filepath);
    }
    InternalTexture UploadTexture(Span<byte> bytes, uint width, uint height, string name)
    {

        TextureDescriptor texturedescriptor = new()
        {
            Label = name,
            Dimension = TextureDimension.D2,
            Size = new Extent3D(width, height, 1),
            SampleCount = 1,
            Format = TextureFormat.RGBA8Unorm,
            MipLevelCount = 1,
            Usage = TextureUsage.CopyDst | TextureUsage.TextureBinding,
        };
        Texture gputex = cxt.device.CreateTexture(texturedescriptor)!;

        TextureViewDescriptor textureViewDescriptor = new()
        {
            Label = name + " TextureView",
            Format = TextureFormat.RGBA8Unorm,
            Dimension = TextureViewDimension.D2,
            BaseMipLevel = 0,
            MipLevelCount = 1,
            BaseArrayLayer = 0,
            ArrayLayerCount = 1,
            Aspect = TextureAspect.All
        };
        TextureView gputexview = gputex.CreateView(textureViewDescriptor)!;

        ImageCopyTexture destination = new()
        {
            Texture = gputex,
            MipLevel = 0,
            Origin = default,
            Aspect = TextureAspect.All
        };

        TextureDataLayout layout = new()
        {
            Offset = 0,
            BytesPerRow = width * (uint)4,
            RowsPerImage = height,
        };
        Extent3D size = new(width, height, 1);

        cxt.queue.WriteTexture(destination, bytes, layout, size);


        BindGroupDescriptor image_bg_descriptor = new()
        {
            Layout = bindgroupLayout_image,
            Entries = [
                new BindGroupEntry()
                {
                    TextureView = gputexview,
                }
            ]
        };

        BindGroup bindgroup = cxt.device.CreateBindGroup(image_bg_descriptor)!;

        var nt = new InternalTexture()
        {
            Texture = gputex,
            TextureView = gputexview,
            Sampler = cxt.defaultSampler,
            BindGroup = bindgroup,
        };
        textures.Add(name, nt);
        return nt;
    }


   
}
