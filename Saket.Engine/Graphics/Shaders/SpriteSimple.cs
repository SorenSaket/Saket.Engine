using Saket.WebGPU;
using Saket.WebGPU.Native;
using Saket.WebGPU.Objects;
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
            var blendState = new WGPUBlendState()
            {
                color = new WGPUBlendComponent()
                {
                    srcFactor = WGPUBlendFactor.SrcAlpha,
                    dstFactor = WGPUBlendFactor.OneMinusSrcAlpha,
                    operation = WGPUBlendOperation.Add
                },
                alpha = new WGPUBlendComponent()
                {
                    srcFactor = WGPUBlendFactor.Zero,
                    dstFactor = WGPUBlendFactor.One,
                    operation = WGPUBlendOperation.Add
                },
            };

            var attributes_Transform = stackalloc WGPUVertexAttribute[]
                {
                // Position
                new()
                {
                    format = WGPUVertexFormat.Float32x3,
                    offset = 4 * 0,
                    shaderLocation = 0,
                },
                // Rotation
                new()
                {
                    format = WGPUVertexFormat.Float32,
                    offset = 4 * 3,
                    shaderLocation = 1,
                },
                // Size
                new()
                {
                    format = WGPUVertexFormat.Float32x2,
                    offset = 4 * 4,
                    shaderLocation = 2,
                }
            };

            var attributes_sprite = stackalloc WGPUVertexAttribute[]
            {
                new()
                {
                    format = WGPUVertexFormat.Sint32,
                    offset = 4 * 7,
                    shaderLocation = 3,
                },
                new()
                {
                    format = WGPUVertexFormat.Float32x4,
                    offset = 4 * 8,
                    shaderLocation = 4,
                },
            };

            var layouts = stackalloc nint[]
             {
                graphics.systemBindGroup.Handle,
                TextureAtlas.GetBindGroupLayout(graphics).Handle,
            };
            var pipelineLayout = wgpu.DeviceCreatePipelineLayout(graphics.device.Handle, new WGPUPipelineLayoutDescriptor()
            {
                bindGroupLayouts = layouts,
                bindGroupLayoutCount = 2,
            });

            SaketShaderDescriptor shaderDescriptor = new()
            {
                label = "spriteShader"u8,
                shaderSource = ShaderSource,
                //
                pipelineLayout = pipelineLayout,
                vertex_entryPoint = "vtx_main"u8,
                vertex_VertexBufferLayouts = stackalloc WGPUVertexBufferLayout[]
                {
                    new()
                    {
                        arrayStride = (ulong)sizeof(Transform2D),
                        stepMode = WGPUVertexStepMode.Instance,
                        attributeCount = 3,
                        attributes = attributes_Transform
                    },
                    new()
                    {
                        arrayStride = (ulong)sizeof(Sprite),
                        stepMode = WGPUVertexStepMode.Instance,
                        attributeCount = 2,
                        attributes = attributes_sprite
                    }
                },
                fragment_entryPoint = "frag_main"u8,
                fragment_colorTargets = stackalloc WGPUColorTargetState[] {
                    new()
                    {
                        format = graphics.applicationpreferredFormat,
                        blend = &blendState,
                        writeMask = WGPUColorWriteMask.All
                    }
                },
                multisampleState = new()
                {
                    count = 1,
                    mask = ~0u,
                    alphaToCoverageEnabled = false
                },
                primitiveState = new()
                {
                    topology = WGPUPrimitiveTopology.TriangleList,
                    stripIndexFormat = WGPUIndexFormat.Undefined,
                    frontFace = WGPUFrontFace.CCW,
                    cullMode = WGPUCullMode.None,
                },
                
            };

            // Create the shader w device
            return new Shader(graphics.device, shaderDescriptor);
        }
    }
}