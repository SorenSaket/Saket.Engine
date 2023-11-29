﻿
using Saket.ECS;
using Saket.Engine;
using Saket.Engine.Formats;
using Saket.Engine.Graphics;
using Saket.Engine.Graphics.Renderers;
using Saket.Engine.Platform;
using Saket.Engine.Resources.Loaders;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Numerics;
using HackAttack.Components;

using WebGpuSharp;
using WebGpuSharp.FFI;
using Saket.Engine.Platform.Windowing;

namespace HackAttack;

internal class Application : Saket.Engine.Application
{
    private World world;
    private Pipeline pipeline_update;
    private Pipeline pipeline_render;

    private RendererSpriteSimple spriteRenderer;

    private Window window;
    private GraphicsContext graphics;

    Shader shader_sprite;

    TextureAtlas atlas;

    CameraOrthographic camera;
    IDesktopPlatform platform;

    public unsafe Application()
    {
        // Windows only application for now
        platform = new Saket.Engine.Platform.SDL.Platform();

        // Create new graphics Context
        graphics = new GraphicsContext();
        //only BGRA8Unorm is supported for webgpu dawn
        graphics.applicationpreferredFormat = TextureFormat.BGRA8Unorm;

        window = platform.CreateWindow(new WindowCreationArgs("Hack Attack", 30, 30, 1280,720));

        graphics.AddWindow(window);

        spriteRenderer = new RendererSpriteSimple(graphics, 1000);

        shader_sprite = Saket.Engine.Graphics.Shaders.SpriteSimple.CreateShader(graphics, File.ReadAllBytes(@".\Assets\Shaders\Sprite\shader_sprite.wgsl"));
         
        atlas = Pyxel.Load(File.OpenRead(@".\Assets\tileset.pyxel"));

        // Setup ecs
        world = new();
        pipeline_update = new();
        pipeline_update.AddStage(new Stage().Add(Systems.Move));
        pipeline_render = new();

        camera = new CameraOrthographic(5f, 0.1f, 100f);
        {
            world.CreateEntity()
                .Add(new Transform2D() { })
                .Add(new Sprite() { color = Saket.Engine.Graphics.Color.White, spr = 0 })
                .Add(new PathFindingAgent());

            world.CreateEntity()
               .Add(new Transform2D() {Position = new Vector2(2,0) })
               .Add(new Sprite() { color = Saket.Engine.Graphics.Color.White, spr = 1 })
               .Add(new PathFindingAgent());
        }
    }

    public override void Update()
    {
        // Poll all events
        while (window.PollEvent() != WindowEvent.None)
        {

        }

        // Non-standardized behavior: submit empty queue to flush callbacks
        // (wgpu-native also has a device.poll but its API is more complex)
        //wgpu.QueueSubmit(graphics.queue.Handle, 0, 0);
        //graphics.device.Tick();
        graphics.instance.ProcessEvents();
    #if true
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
            var swapchain = graphics.windows[window].swapchain;


            graphics.SetSystemUniform(new SystemUniform()
            {
                projectionMatrix = camera.projectionMatrix,
                viewMatrix = camera.viewMatrix,
                frame = Frame
            });

            // Get the texture where to draw the next frame
            var textureView = swapchain.GetCurrentTextureView();

            // Clear the screen
            graphics.Clear(textureView, new Saket.Engine.Graphics.Color(255, 0, 0));

            spriteRenderer.Draw(new Sprite(0, 0, Saket.Engine.Graphics.Color.Blue), new Transform2D());
            spriteRenderer.SubmitBatch(textureView, shader_sprite.pipeline, atlas);


            // This is not how you use TextureViewHandle.Release
            //TextureViewHandle.Release( WebGPUMarshal.GetOwnedHandle(textureView));

            // Preset swapchain
            swapchain.Present();
        }
    #endif 
    }

}