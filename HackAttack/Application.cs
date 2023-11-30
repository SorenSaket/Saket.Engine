
using Saket.ECS;
using Saket.Engine;
using Saket.Engine.Formats;
using Saket.Engine.Graphics;
using Saket.Engine.Graphics.Renderers;
using Saket.Engine.Resources.Loaders;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Numerics;
using HackAttack.Components;

using WebGpuSharp;
using WebGpuSharp.FFI;
using SDL2;
using Saket.Engine.Components;
using System.Reflection.Metadata;

namespace HackAttack;





// TODO.
// Handle windows in a generalized way. Add window/surface to saket.engine?
// Handle window resizing. Swapchain, Depthbuffer, camera matricies etc..
    // https://eliemichel.github.io/LearnWebGPU/basic-3d-rendering/some-interaction/resizing-window.html
    //






internal class Application : Saket.Engine.Application
{
    private World world;
    private Pipeline pipeline_update;
    private Pipeline pipeline_render;

    private RendererSpriteSimple spriteRenderer;

    private RenderTarget rt;
    private nint sdlwindow;
    private GraphicsContext graphics;

    Shader shader_sprite;

    TextureAtlas atlas;

    CameraOrthographic camera;

    public unsafe Application()
    {
        // Windows only application for now
        SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING);

        // Create new graphics Context
        graphics = new GraphicsContext();
        //only BGRA8Unorm is supported for webgpu dawn
        graphics.applicationpreferredFormat = TextureFormat.BGRA8Unorm;

        int initialWidth = 1280, initialHeight = 720;
        sdlwindow = SDL.SDL_CreateWindow("Hack Attack", 30, 30, (int)initialWidth, (int)initialHeight, SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);


         rt =graphics.CreateRenderTarget( CreateWebGPUSurfaceFromSDLWindow(graphics.instance, sdlwindow), initialWidth, initialHeight, graphics.applicationpreferredFormat);



        spriteRenderer = new RendererSpriteSimple(graphics, 1000);

        shader_sprite = Saket.Engine.Graphics.Shaders.SpriteSimple.CreateShader(graphics, File.ReadAllBytes(@".\Assets\Shaders\Sprite\shader_sprite.wgsl"));
         
        atlas = Pyxel.Load(File.OpenRead(@".\Assets\tileset.pyxel"));

        // Setup ecs
        world = new();
        pipeline_update = new();
        pipeline_update.AddStage(new Stage().Add(Systems.Move));
        pipeline_render = new();

        camera = new CameraOrthographic(5f,rt.AspectRatio, 0.1f, 100f);
        {
            world.CreateEntity()
                .Add(new Transform2D() { })
                .Add(new Sprite() { color = Saket.Engine.Graphics.Color.White, spr = 0 })
                .Add(new PathFindingAgent());

            world.CreateEntity()
               .Add(new Transform2D() {Position = new Vector2(1,0) })
               .Add(new Sprite() { color = Saket.Engine.Graphics.Color.White, spr = 1 })
               .Add(new PathFindingAgent());
        }
    }

    public override void Update()
    {
        // Poll all events
        while (SDL.SDL_PollEvent(out var e) != 0)
        {
            if (e.type == SDL.SDL_EventType.SDL_QUIT)
            {
                Termiate();
            }
            if(e.type == SDL.SDL_EventType.SDL_WINDOWEVENT)
            {
                if(e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
                {
                    graphics.OnRenderTargetSurfaceResized(rt, e.window.data1, e.window.data2);
                    camera = new(5f, rt.AspectRatio, 0.1f, 100f);
                }
            }
        }

        // Non-standardized behavior: submit empty queue to flush callbacks
        // (wgpu-native also has a device.poll but its API is more complex)
        //wgpu.QueueSubmit(graphics.queue.Handle, 0, 0);
        //graphics.device.Tick();
        graphics.instance.ProcessEvents();

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
            // To present frame to the window use the swapchain
            var swapchain = rt.Swapchain;


            graphics.SetSystemUniform(new SystemUniform()
            {
                projectionMatrix = camera.projectionMatrix,
                viewMatrix = camera.viewMatrix,
                frame = Frame
            });

            // Get the texture where to draw the next frame
            var textureView = swapchain.GetCurrentTextureView();

            // Clear the screen
            graphics.Clear(textureView, new Saket.Engine.Graphics.Color(0, 0, 0));

            spriteRenderer.Draw(new Sprite(0, 0, Saket.Engine.Graphics.Color.Blue), new Transform2D());
            spriteRenderer.SubmitBatch(textureView, shader_sprite.pipeline, atlas);


            // This is not how you use TextureViewHandle.Release
            //TextureViewHandle.Release( WebGPUMarshal.GetOwnedHandle(textureView));

            // Preset swapchain
            swapchain.Present();
        }
        
    }

    public Surface? CreateWebGPUSurfaceFromSDLWindow(Instance instance, nint windowHandle)
    {
        unsafe
        {
            SDL.SDL_SysWMinfo info = new();
            SDL.SDL_GetVersion(out info.version);
            SDL.SDL_GetWindowWMInfo(windowHandle, ref info);

            if (info.subsystem == SDL.SDL_SYSWM_TYPE.SDL_SYSWM_WINDOWS)
            {
                var wsDescriptor = new WebGpuSharp.FFI.SurfaceDescriptorFromWindowsHWNDFFI()
                {
                    Hinstance = (void*)info.info.win.hinstance,
                    Hwnd = (void*)info.info.win.window,
                    Chain = new ChainedStruct()
                    {
                        SType = SType.SurfaceDescriptorFromWindowsHWND
                    }
                };
                SurfaceDescriptor descriptor_surface = new SurfaceDescriptor(ref wsDescriptor);
                return instance.CreateSurface(descriptor_surface);
            }

            throw new Exception("Platform not supported");
        }
    }
}