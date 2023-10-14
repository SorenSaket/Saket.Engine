
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
using System.Numerics;
using HackAttack.Components;

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
            // Windows only application for now
            platform = new Saket.Engine.Platform.Windows.Platform();

            // Create new graphics Context
            graphics = new GraphicsContext();
            graphics.applicationpreferredFormat = WGPUTextureFormat.BGRA8UnormSrgb;

            window = platform.CreateWindow();

            graphics.AddWindow(window);

            spriteRenderer = new RendererSpriteSimple(graphics, 1000);

            shader_sprite = Saket.Engine.Graphics.Shaders.SpriteSimple.CreateShader(graphics, File.ReadAllBytes(@".\Assets\Shaders\Sprite\shader_sprite.wgsl"));
             
            bindgroup_atlas = Pyxel.Load(File.OpenRead(@".\Assets\tileset.pyxel")).GetBindGroup(graphics);

            // Setup ecs
            world = new();
            pipeline_update = new();
            pipeline_update.AddStage(new Stage().Add(Systems.Move));
            pipeline_render = new();

            camera = new CameraOrthographic(5f, 0.1f, 100f);
            {
                world.CreateEntity()
                    .Add(new Transform2D() { })
                    .Add(new Sprite() { color = Color.White, spr = 0 })
                    .Add(new PathFindingAgent());

                world.CreateEntity()
                   .Add(new Transform2D() {Position = new Vector2(2,0) })
                   .Add(new Sprite() { color = Color.White, spr = 1 })
                   .Add(new PathFindingAgent());
            }
        }
        
        public override void Update()
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
                world.Delta = (float)DeltaTime;
                world.Time = (float)TotalElapsedTime;
                pipeline_update.Update(world);
            }

            // Perform rendering
            // TODO make safe
            // TODO, For each Camera
            unsafe
            {
                
                graphics.SetSystemUniform(new SystemUniform()
                {
                    projectionMatrix = camera.projectionMatrix,
                    viewMatrix = camera.viewMatrix,
                    frame = Frame
                });

                // Get the texture where to draw the next frame
                nint textureView = window.GetCurretTextureView().Handle;
                if (textureView == 0)
                    throw new Exception("faild to get texture view for next frame");
                
                // Clear the screen


                //spriteRenderer.Draw(new Sprite(0, 0, 0), )

                // We can now release the textureview
                wgpu.TextureViewRelease(textureView);

                // Preset swapchain
                window.swapchain.Present();
            }
        }
    }
}