using WebGpuSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Graphics.Shaders
{
    public static class SpriteSimple
    {
        public unsafe static Shader CreateShader(GraphicsContext graphics, ReadOnlySpan<byte> ShaderSource)
        {
            var blendState = new BlendState()
            {
                Color = new BlendComponent()
                {
                    SrcFactor = BlendFactor.SrcAlpha,
                    DstFactor = BlendFactor.OneMinusSrcAlpha,
                    Operation = BlendOperation.Add
                },
                Alpha = new BlendComponent()
                {
                    SrcFactor = BlendFactor.Zero,
                    DstFactor = BlendFactor.One,
                    Operation = BlendOperation.Add
                },
            };


            // Vertex Layout

            VertexAttributeList attributes_Transform = new()
            {
                new()
                {
                    Format = VertexFormat.Float32x3,
                    Offset = 4 * 0,
                    ShaderLocation = 0,
                },
                new()
                {
                    Format = VertexFormat.Float32,
                    Offset = 4 * 3,
                    ShaderLocation = 1,
                },
                new()
                {
                    Format = VertexFormat.Float32x2,
                    Offset = 4 * 4,
                    ShaderLocation = 2,
                }
            };

            VertexAttributeList attributes_sprite = new()
            {
                new()
                {
                    Format = VertexFormat.Sint32,
                    Offset = 4 * 1,
                    ShaderLocation = 3,
                },
                new()
                {
                    Format = VertexFormat.Unorm8x4,
                    Offset = 4 * 2,
                    ShaderLocation = 4,
                },
            };

            VertexBufferLayoutList vertexBufferLayouts = new ()
           {
                new()
                {
                    ArrayStride = (ulong)sizeof(Transform2D),
                    StepMode = VertexStepMode.Instance,

                    Attributes = attributes_Transform
                },
                new()
                {
                    ArrayStride = (ulong)sizeof(Sprite),
                    StepMode = VertexStepMode.Instance,
                    Attributes = attributes_sprite
                }
           };


            // Bingroup layout
            var layouts = new BindGroupLayout[]
             {
                graphics.systemBindGroupLayout,
                TextureAtlas.GetBindGroupLayout(graphics),
            };
            var pipelineLayout = graphics.device.CreatePipelineLayout(new PipelineLayoutDescriptor()
            {
                BindGroupLayouts = layouts.AsSpan()
            }) ;

            SaketShaderDescriptor shaderDescriptor = new()
            {
                label = "spriteShader",
                shaderSource = ShaderSource,
                //
                pipelineLayout = pipelineLayout,
                vertex_entryPoint = "vtx_main",
                vertex_VertexBufferLayouts = vertexBufferLayouts,
                fragment_entryPoint = "frag_main",
                fragment_colorTargets = new() {
                    new()
                    {
                        Format = graphics.applicationpreferredFormat,
                        Blend = blendState,
                        WriteMask = ColorWriteMask.All
                    }
                },
                multisampleState = new()
                {
                    Count = 1,
                    Mask = ~0u,
                    AlphaToCoverageEnabled = false
                },
                primitiveState = new()
                {
                    Topology = PrimitiveTopology.TriangleList,
                    StripIndexFormat = IndexFormat.Undefined,
                    FrontFace = FrontFace.CCW,
                    CullMode = CullMode.None,
                }
            };

            // Create the shader w device
            return new Shader(graphics.device, shaderDescriptor);
        }
    }
}