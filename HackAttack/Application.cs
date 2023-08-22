
using Saket.ECS;
using Saket.Engine;
using Saket.Engine.Formats;
using Saket.Engine.Graphics;
using Saket.Engine.Graphics.Renderers;
using Saket.Engine.Platform;
using Saket.Engine.Resources.Loaders;
using Saket.Graphics;
using Saket.WebGPU;
// TODO remove depency on native
using Saket.WebGPU.Native;
using Saket.WebGPU.Objects;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace HackAttack
{
    internal class Application : Saket.Engine.Application
    {
        private World world;
        private Pipeline pipeline_update;
        private Pipeline pipeline_render;

        private RendererSpriteSimple spriteRenderer;

        private Window window;
        private GraphicsContext graphics;

        Shader shader_sprite;

        BindGroup bindgroup_atlas;

        CameraOrthographic camera;
        IPlatform platform;

        public unsafe Application()
        {
            platform = new Saket.Engine.Platform.Windows.Platform();

            world = new();
            pipeline_update = new();
            pipeline_render = new();

            graphics = new GraphicsContext();
            graphics.applicationpreferredFormat = WGPUTextureFormat.BGRA8UnormSrgb;

            window = platform.CreateWindow();

            graphics.AddWindow(window);

            spriteRenderer = new RendererSpriteSimple(1000, graphics.device);

            shader_sprite = Saket.Engine.Graphics.Shaders.SpriteSimple.CreateShader(graphics, File.ReadAllBytes(@".\Assets\Shaders\Sprite\shader_sprite.wgsl"));

            bindgroup_atlas = Pyxel.Load(File.OpenRead(@".\Assets\tileset.pyxel")).GetBindGroup(graphics);

            camera = new CameraOrthographic(5f, 0.1f, 100f);
            
            {
                world.CreateEntity()
                    .Add(new Transform2D() { })
                    .Add(new Sprite() { color = Color.White});
            }
        }
        
        public override void Update(float deltaTime)
        {
            // Poll all events
            while (window.PollEvent() != 0)
            {

            }

            // Non-standardized behavior: submit empty queue to flush callbacks
            // (wgpu-native also has a device.poll but its API is more complex)
            wgpu.QueueSubmit(graphics.queue.Handle, 0, 0);


            // Perform Update
            {
                pipeline_update.Update(world, deltaTime);
            }
            // Perform rendering
            // TODO make safe
             {
                graphics.SetSystemUniform(new SystemUniform()
                {
                    projectionMatrix = camera.projectionMatrix,
                    viewMatrix = camera.viewMatrix,
                    frame = Frame
                });

                // TODO, For each Camera
                unsafe {
                    // Get the texture where to draw the next frame
                    nint textureView = window.GetCurretTextureView().Handle;
                    if (textureView == 0)
                        throw new Exception("faild to get texture view for next frame");
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
                        spriteRenderer.SetbuffersAndDraw(graphics.queue.Handle, RenderPassEncoder, c);
                        
                        // Finish Rendering
                        wgpu.RenderPassEncoderEnd(RenderPassEncoder);
                        
                        nint commandBuffer = wgpu.CommandEncoderFinish(commandEncoder, new() { });

                        wgpu.QueueSubmit(graphics.queue.Handle, 1, new IntPtr(&commandBuffer));

                        wgpu.RenderPassEncoderRelease(RenderPassEncoder);
                        wgpu.CommandEncoderRelease(commandEncoder);
                    });
                   
                    

                    // We can now release the textureview
                    wgpu.TextureViewRelease(textureView);

                    // Preset swapchain
                    window.swapchain.Present();
                    
                }
            }
        }
    }
}