
using Saket.ECS;
using Saket.Engine;
using Saket.Engine.Graphics;
using Saket.Engine.Platform;
using Saket.Engine.Resources.Loaders;
using Saket.WebGPU;
// TODO remove depency on native
using Saket.WebGPU.Native;
using Saket.WebGPU.Objects;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HackAttack
{
    internal class Application : Saket.Engine.Application
    {
        private World world;
        private Pipeline pipeline_update;
        private Pipeline pipeline_render;

        private RendererSpriteSimple spriteRenderer;

        private Window window;
        private Graphics graphics;

        Shader shader_sprite;

        BindGroup bindgroup_atlas;

        CameraOrthographic camera;

        public unsafe Application()
        {
            world = new();
            pipeline_update = new();
            pipeline_render = new();

            graphics = new Graphics();

            window = Platform.CreateWindow();

            graphics.AddWindow(window);
            
            spriteRenderer = new RendererSpriteSimple(1000, graphics.device);

            shader_sprite = Saket.Engine.Graphics.Shaders.SpriteSimple.CreateShader(graphics, MemoryMarshal.Cast<char, byte>( File.ReadAllText(@".\Assets\Shaders\Sprite\shader_sprite.wgsl", System.Text.Encoding.UTF8).AsSpan()));

            bindgroup_atlas = LoaderPyxel.Load(File.OpenRead(@".\Assets\tileset.pyxel")).GetBindGroup(graphics);

            camera = new CameraOrthographic(5f, 0.1f, 100f);
            graphics.SetSystemUniform(new SystemUniform()
            {
                projectionMatrix = camera.projectionMatrix,
                viewMatrix = camera.viewMatrix,
                frame = 0
            });
        }

        public override void Update(float deltaTime)
        {
            // Perform Update
            {
                pipeline_update.Update(world, deltaTime);
            }

            // Perform rendering
            // TODO make safe
             {
                // Non-standardized behavior: submit empty queue to flush callbacks
                // (wgpu-native also has a device.poll but its API is more complex)
                wgpu.QueueSubmit(graphics.queue.Handle, 0, 0);

                // TODO, For each Camera
                unsafe {
                    GraphicsWindow w = graphics.windows[window];
                    // Get the texture where to draw the next frame
                    nint textureView = w.GetCurretTextureView().Handle;
                    
                    // RenderPass
                    WGPURenderPassColorAttachment renderPassColorAttachment = new()
                    {
                        view = textureView,
                        resolveTarget = 0,
                        loadOp = WGPULoadOp.Clear,
                        storeOp = WGPUStoreOp.Store,
                        clearValue = new WGPUColor(0.9, 0.1, 0.2, 1.0)
                    };
                    WGPURenderPassDescriptor renderPassDesc = new()
                    {
                        colorAttachmentCount = 1,
                        colorAttachments = &renderPassColorAttachment,
                        depthStencilAttachment = null,
                        timestampWriteCount = 0,
                        timestampWrites = null,
                        nextInChain = null,
                    };


                    // This should hopefully only iterate once
                    spriteRenderer.IterateForRender(world, (c) => {  // 
                        // Command Encoder
                        nint commandEncoder = wgpu.DeviceCreateCommandEncoder(graphics.device.Handle, new() { });

                        nint RenderPassEncoder = wgpu.CommandEncoderBeginRenderPass(commandEncoder, renderPassDesc);

                        // Set the pipline for the renderpass
                        wgpu.RenderPassEncoderSetPipeline(RenderPassEncoder, shader_sprite.pipeline);

                        // Set system bind group
                        // TODO
                        wgpu.RenderPassEncoderSetBindGroup(RenderPassEncoder, 0, graphics.systemBindGroup.Handle, 0, (uint*)0);
                        // Set atlas/sampler bindgroup
                        wgpu.RenderPassEncoderSetBindGroup(RenderPassEncoder, 1, bindgroup_atlas.Handle, 0, (uint*)0);

                        // set vertex buffers and Submit actual draw comand
                        spriteRenderer.SetbuffersAndDraw(RenderPassEncoder, c);
                        
                        // Finish Rendering
                        wgpu.RenderPassEncoderEnd(RenderPassEncoder);

                        nint commandBuffer = wgpu.CommandEncoderFinish(commandEncoder, new() { });
                        wgpu.QueueSubmit(graphics.queue.Handle, 1, commandBuffer);

                        wgpu.RenderPassEncoderRelease(RenderPassEncoder);
                        wgpu.CommandEncoderRelease(commandEncoder);
                    });

                    // We can now release the textureview
                    wgpu.TextureViewRelease(textureView);
                    // Preset swapchain
                    w.swapchain.Present();
                }
            }
           

        }
    }
}