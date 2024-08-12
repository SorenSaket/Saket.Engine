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
using NAudio.Wave;
using System.Diagnostics;

using Saket.Navigation.VectorField;
using Saket;
using Saket.Engine.GUI;
using Saket.Engine.GUI.Styling;
using Saket.Engine.GUI.Layouting;
using ImGuiNET;
using Saket.Engine.IMGUI;

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
    World uiworld;
    Pipeline pipeline_update;

    RendererSpriteSimple spriteRenderer;

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

    Entity[] map;
    Entity[] dirs;
    int mapWidth = 16, mapHeight = 16;

    Document doc;

    ImGui_Impl_WebGPUSharp ImGuiImpl;

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

            Surface surface = CreateWebGPUSurfaceFromSDLWindow(graphics.instance, sdlwindow);
            
            SurfaceConfiguration config = new SurfaceConfiguration()
            {
                Device = graphics.device,
                Format = TextureFormat.BGRA8Unorm,
                Width = (uint)initialWidth,
                Height = (uint)initialHeight,
                Usage = TextureUsage.RenderAttachment,
                PresentMode = PresentMode.Fifo,
                AlphaMode = CompositeAlphaMode.Auto
            };
            surface.Configure(config);
            

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

        // Setup IMGUI
        {
            IntPtr context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);

            var io = ImGui.GetIO();
            

            var initInfo = new ImGui_ImplWGPU_InitInfo()
            {
                device = graphics.device,
                num_frames_in_flight = 3,
                rt_format = graphics.applicationpreferredFormat,
                depth_format = TextureFormat.Undefined,
            };

            ImGuiImpl = new ImGui_Impl_WebGPUSharp(initInfo);

            io.Fonts.AddFontDefault();
        }

        // Create GUI
        if(false)
        {
            uiworld = new();
            doc = new Document(uiworld);

            Style style_window = new Style()
            {
                Width = new(100, Measurement.Percentage),
                Height = new(100, Measurement.Percentage),
                Axis = Axis.Vertical,
                AlignContent = AlignContent.end,
                AlignItems = AlignItems.center
            };

            // we want the toolbar to always fill 80% of the screen
            Style style_toolbar = new Style()
            {
                Width = new(80, Measurement.Percentage),
                Height = new(64),
                Axis = Axis.Horizontal,
                AlignContent = AlignContent.center,
                AlignItems = AlignItems.center
            };


            Style buttonStyle = new Style()
            {
                Width = new(32),
                Height = new(32)
            };
           
            // Create window which is the base container
            var window = doc.CreateGUIEntity(new () { style = style_window });

            // Create toolbar
            var toolbar = doc.CreateGUIEntity(new () { parent = window, style = style_toolbar });

            // Create buttons
            doc.CreateGUIEntity(new () { parent = toolbar, style = buttonStyle });
            doc.CreateGUIEntity(new () { parent = toolbar, style = buttonStyle });
            
            // Do layouting at startup since it doesn't change at runtime
           // Layouting.LayoutEntity(window, new Constraints(0, 1920, 0, 1080));
        }
        
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
                 .Add(new CameraOrthographic(16f, 16f/9f, 0.1f, 100f)).EntityPointer;
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


            map = new Entity[mapHeight * mapWidth];
            dirs = new Entity[mapHeight * mapWidth];
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    map[x+y*mapWidth] = world.CreateEntity()
                                     .Add(new Transform2D(x, y))
                                     .Add(new Sprite() { color = new Saket.Engine.Graphics.Color(0,40,0), spr = 51 });

                    dirs[x + y * mapWidth] = world.CreateEntity()
                                     .Add(new Transform2D(x, y))
                                     .Add(new Sprite() { color = new Saket.Engine.Graphics.Color(0, 40, 0), spr = 52 });
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
                    //graphics.OnRenderTargetSurfaceResized(rt, e.window.data1, e.window.data2);
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

        // Navigation
        if (gameState.IsDirty)
        {
            gameState.heatmap = HeatmapGenerator.GenerateAffectorHeatmap(gameState.mapData, new Saket.Navigation.Affector() { Position = new Vector3(16 / 2, 16 / 2, 0), Main = true });



            float max = gameState.heatmap.Max();   
            float min = gameState.heatmap.Min();   


            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    Sprite spr = map[x + y * mapWidth].Get<Sprite>();
                    float val = Mathf.Remap(gameState.heatmap[x, y], min, max, 0, 1);
                    spr.color = new Saket.Engine.Graphics.Color(val, val, val);
                    map[x + y * mapWidth].Set<Sprite>(spr);
                }
            }


            gameState.field = VectorFieldGenerator.GenerateVectorFieldA(gameState.heatmap);

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    Transform2D transform = dirs[x + y * mapWidth].Get<Transform2D>();
                    transform.rx = MathF.Atan2(gameState.field[x, y].Y, gameState.field[x, y].X);
                    dirs[x + y * mapWidth].Set(transform);
                }
            }

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

            {
                ImGuiIOPtr io = ImGui.GetIO();
                io.DisplaySize = new Vector2(1280, 720);
                io.DisplayFramebufferScale = Vector2.One;
                io.DeltaTime = (float)DeltaTime;
                
                ImGuiImpl.NewFrame();
                ImGui.NewFrame();
                //

                ImGui.Begin("a");
                ImGui.Text("This is some useful text.");
                ImGui.End();
               


                MouseState MouseState = mouseState;
                KeyboardState KeyboardState = keyboardState;

                io.MouseDown[0] = MouseState[MouseButton.Left];
                io.MouseDown[1] = MouseState[MouseButton.Right];
                io.MouseDown[2] = MouseState[MouseButton.Middle];
                io.MousePos = new Vector2(MouseState.X, MouseState.Y);

                foreach (Keys key in Enum.GetValues(typeof(Keys)))
                {
                    io.AddKeyEvent((ImGuiKey)(key), KeyboardState.IsKeyDown(key));
                }

                ImGui.Render();

                RenderPassDescriptor renderPassDesc = new()
                {
                    ColorAttachments = new RenderPassColorAttachment[]
                {
                        new()
                        {
                            View = textureView,
                            ResolveTarget = default,
                            LoadOp = LoadOp.Load,
                            StoreOp = StoreOp.Store,
                        }
                },
                    DepthStencilAttachment = null,
                };
                // Command Encoder
                var commandEncoder = graphics.device.CreateCommandEncoder(new() { });

                var RenderPassEncoder = commandEncoder.BeginRenderPass(renderPassDesc);
                ImGuiImpl.RenderDrawData(ImGui.GetDrawData(), RenderPassEncoder);

                // Finish Rendering
                RenderPassEncoder.End();
                var commandBuffer = commandEncoder.Finish(new() { });

                graphics.queue.Submit(commandBuffer);
            }
            // Render GUI
            /* {
                 graphics.SetSystemUniform(new SystemUniform()
                 {
                     projectionMatrix = Matrix4x4.CreateOrthographic(1920,1080,0,10),
                     viewMatrix = Matrix4x4.Identity,
                     frame = Frame
                 });

                 doc.Render(spriteRenderer);
                 spriteRenderer.SubmitBatch(textureView, shader_sprite.pipeline, atlas);
             }*/



            //spriteRenderer.Draw(new Sprite(0, 0, Saket.Engine.Graphics.Color.Blue), new Transform2D());
            // spriteRenderer.SubmitBatch(textureView, shader_sprite.pipeline, atlas);


            // Preset swapchain
            swapchain.Present();
        }

    }



    // only works with windows
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