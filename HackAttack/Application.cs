using Saket.ECS;
using Saket.Engine;
using Saket.Engine.Input;
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
using SDL2;
using Saket.Engine.Components;
using System.Reflection.Metadata;
using NAudio.Wave;
using System.Diagnostics;

using Saket.Navigation.VectorField;

namespace HackAttack;


// TODO.
// Handle windows in a generalized way. Add window/surface to saket.engine?
// Handle window resizing. Swapchain, Depthbuffer, camera matricies etc..
// https://eliemichel.github.io/LearnWebGPU/basic-3d-rendering/some-interaction/resizing-window.html
//


// Input 
// Input system
//

// Make the player move
// Animation
// Footstep sounds
// Footstep particle effect

// Collision System for Saket.Engine
// Not physics only collisions!
// Acceleration structure

// Stat system

// UI system

// Navigation


// pixl art scaling : https://colececil.io/blog/2017/scaling-pixel-art-without-destroying-it/
class Camera
{
    public ECSPointer camera;
}


public class GameState
{
    public int lastNavVersion = 0;
    public int navVersion = 1;

    public void SetDirty() { navVersion++; }
    public bool IsDirty => navVersion > lastNavVersion;

    public Saket.Navigation.MapData mapData;
    public float[,] heatmap;
    public Vector2[,] field;
}


internal class Application : Saket.Engine.Application
{
    World world;
    Pipeline pipeline_update;

    RendererSpriteSimple spriteRenderer;

    RenderTarget rt;
    nint sdlwindow;
    WindowInfo windowInfo;

    GraphicsContext graphics;

    Shader shader_sprite;

    TextureAtlas atlas;

    Camera camera;

    nint keyboardStatePtr;
    KeyboardState keyboardState;
    MouseState mouseState;


   
    GameState gameState;

    public unsafe Application()
    {
        // Setup window and graphics
        int initialWidth = 1280, initialHeight = 720;
        {
            // Create new graphics Context
            graphics = new GraphicsContext();

            //only BGRA8Unorm is supported for webgpu dawn
            graphics.applicationpreferredFormat = TextureFormat.BGRA8Unorm;
            
            // Setup windowing with SDL. 
            SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING);
            // Create SDL window
            sdlwindow = SDL.SDL_CreateWindow("Hack Attack", 30, 30, (int)initialWidth, (int)initialHeight, SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

            rt = graphics.CreateRenderTarget(CreateWebGPUSurfaceFromSDLWindow(graphics.instance, sdlwindow), initialWidth, initialHeight, graphics.applicationpreferredFormat);

            spriteRenderer = new RendererSpriteSimple(graphics, 1000);

            shader_sprite = Saket.Engine.Graphics.Shaders.SpriteSimple.CreateShader(graphics, File.ReadAllBytes(@".\Assets\Shaders\Sprite\shader_sprite.wgsl"));

            atlas = Pyxel.Load(File.OpenRead(@".\Assets\tileset.pyxel"));
        }
        // Setup ecs
        { 
            world = new();
            pipeline_update = new();
            pipeline_update.AddStage(
                new Stage()
                .Add(Systems.ClientInput)
                .Add(Systems.PlayerMove)
                .Add(Systems.PlayerBuild)
                .Add(Systems.PlayerAnimation)
                 
               
                .Add(Systems.Enemy_Navigation)
                .Add(Systems.Enemy)

                .Add(Systems.Move)


                .Add(Systems.MoveTowardsTarget)
                .Add(Systems.RotateTowardsTarget)

                .Add(Saket.Engine.Camera.CameraSystem2D)
                );
        }

        int mapWidth = 16, mapHeight = 16;



        
        // Create Entities
        {
            var entity_b = world.CreateEntity()
            .Add(new Transform2D(-10, 0,-1))
            .Add(new Sprite() { color = Saket.Engine.Graphics.Color.White, spr = 48 });

            var entity_player = world.CreateEntity()
                .Add(new Transform2D(0, 0))
                .Add(new Sprite() { color = Saket.Engine.Graphics.Color.White, spr = 0 })
                .Add(new Velocity())
                .Add(new Player() { buildobj = entity_b.EntityPointer})
                .Add(new Collider2DBox(Vector2.One*0.2f));
        

            camera = new();
            camera.camera = world.CreateEntity()
                 .Add(new Transform2D(0, 0,-10))
                 .Add(new MoveTowardsTarget() { moveSpeed = 1, target = entity_player.EntityPointer})
                 .Add(new CameraOrthographic(16f, rt.AspectRatio, 0.1f, 100f)).EntityPointer;
            world.SetResource(camera);

            var enemy = world.CreateEntity()
                   .Add(new Transform2D(-5, 0))
                   .Add(new Sprite() { color = Saket.Engine.Graphics.Color.White, spr = 16 })
                   .Add(new Velocity(1, 0))
                   .Add(new Enemy());

            world.CreateEntity()
                .Add(new Transform2D(0, -2))
                .Add(new Sprite() { color = Saket.Engine.Graphics.Color.White, spr = 32 })
                .Add(new RotateTowardsTarget() { rotationSpeed = 5, target = enemy.EntityPointer });

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    world.CreateEntity()
                     .Add(new Transform2D(x, y))
                     .Add(new Sprite() { color = new Saket.Engine.Graphics.Color(0,40,0), spr = 51 });
                }
            }
        }


        // Setup Navigation
        {
            gameState = new GameState();
            gameState.mapData = new(mapWidth, mapHeight, 1.0f);

            world.SetResource(gameState);
        }


        // Setup Resources
        {
            windowInfo = new();
            world.SetResource(windowInfo);
            windowInfo.width = initialWidth;
            windowInfo.height = initialHeight;

            keyboardStatePtr = SDL.SDL_GetKeyboardState(out int numKeys);
            keyboardState = new(numKeys);
            world.SetResource(keyboardState);

            mouseState = new();
            world.SetResource(mouseState);
        }



        {
            /*
            Task.Run(() => {
                using (var audioFile = new NAudio.Vorbis.VorbisWaveReader(@"./Assets/Audio/ambience.ogg"))
                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(1000);
                    }
                }
            });*/

        }

#if false
SDLMixer test
        {
            SDL.SDL_AudioSpec requestedSpec = new()
            {
                freq = SDL_mixer.MIX_DEFAULT_FREQUENCY,
                format = SDL_mixer.MIX_DEFAULT_FORMAT,
                channels = 2,
                samples = 4096,
            };
            if (SDL.SDL_OpenAudio(ref requestedSpec, out var spec) < 0)
            {
                throw new Exception(SDL.SDL_GetError());
            }

            //var wave = SDL.SDL_LoadWAV("Assets/Audio/shot.wav", out _, out var buf, out var len);

            //SDL_mixer.Mix_PlayChannel(-1, wave, 1);

            SDL_mixer.MIX_InitFlags flags = (SDL_mixer.MIX_InitFlags.MIX_INIT_OGG);
            SDL_mixer.MIX_InitFlags initted = (SDL_mixer.MIX_InitFlags)SDL_mixer.Mix_Init(flags);

            if ((initted & flags) != flags)
            {
                throw new Exception($"Mix_Init: Failed to init required ogg and mod support!\n");
                throw new Exception($"Mix_Init: {SDL_mixer.Mix_GetError()}");
                // handle error
            }

            var mus = SDL_mixer.Mix_LoadMUS("Assets/Audio/ambience.ogg");
            if(mus == 0)
            {
                throw new Exception(SDL.SDL_GetError());
            }

            SDL_mixer.Mix_VolumeMusic(SDL_mixer.MIX_MAX_VOLUME);
            SDL_mixer.Mix_PlayMusic(mus, -1);
        }
#endif





    }

    public override void Update()
    {
        // Poll Window events
        while (SDL.SDL_PollEvent(out var e) != 0)
        {
            if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
            {
                if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_SPACE)
                {
                    //

                }
            }
            if (e.type == SDL.SDL_EventType.SDL_QUIT)
            {
                Termiate();
            }
            if (e.type == SDL.SDL_EventType.SDL_WINDOWEVENT)
            {
                if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
                {
                    graphics.OnRenderTargetSurfaceResized(rt, e.window.data1, e.window.data2);
                    world.GetEntity(camera.camera).Set( new CameraOrthographic(16f, rt.AspectRatio, 0.1f, 100f)) ;
                    windowInfo.width = e.window.data1;
                    windowInfo.height = e.window.data2;
                }
            }
        }

        // Inject input into ECS
        unsafe
        {
            Span<byte> bytes = new Span<byte>((void*)keyboardStatePtr, keyboardState.KeyCount);
            keyboardState.SetKeyboardState(bytes);
            //SDL.SDL_Scancode
            //SDL.SDL_Keycode

            var a = SDL.SDL_GetMouseState(out int x, out int y);
            mouseState.SetState((int)a, x, y);
        }

        // Poll graphic events
        graphics.instance.ProcessEvents();

        if (gameState.IsDirty)
        {
            gameState.heatmap = HeatmapGenerator.GenerateAffectorHeatmap(gameState.mapData, new Saket.Navigation.Affector() { Position = new Vector3(16 / 2, 16 / 2, 0), Main = true });
            gameState.field = VectorFieldGenerator.GenerateVectorField(gameState.heatmap);
            gameState.lastNavVersion = gameState.navVersion;
        }

        // Perform ECS Update
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

            var cam = world.GetEntity(camera.camera).Get<CameraOrthographic>();

            graphics.SetSystemUniform(new SystemUniform()
            {
                projectionMatrix = cam.projectionMatrix,
                viewMatrix = Matrix4x4.Transpose(cam.viewMatrix),
                frame = Frame
            });

            // Get the texture where to draw the next frame
            var textureView = swapchain.GetCurrentTextureView();

            // Clear the screen
            graphics.Clear(textureView, new Saket.Engine.Graphics.Color(0, 40, 0));

            spriteRenderer.IterateForRender(world, (f) => { spriteRenderer.SubmitBatch(textureView, shader_sprite.pipeline, atlas); });
            //spriteRenderer.Draw(new Sprite(0, 0, Saket.Engine.Graphics.Color.Blue), new Transform2D());
            // spriteRenderer.SubmitBatch(textureView, shader_sprite.pipeline, atlas);


            // This is not how you use TextureViewHandle.Release
            //TextureViewHandle.Release( WebGPUMarshal.GetOwnedHandle(textureView));

            // Preset swapchain
            swapchain.Present();
        }

    }

    public static Surface? CreateWebGPUSurfaceFromSDLWindow(Instance instance, nint windowHandle)
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