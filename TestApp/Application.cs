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

using WebGpuSharp;
using ImGuiNET;
using Saket.Engine.IMGUI;
using SDL2;
using Saket.Engine.GUI;


namespace TestApp;


class Resource_Camera
{
    public ECSPointer camera;
}

struct Asset
{
    public string name;
    public string path;
    public string filetype;
}
public enum AssetType
{
    Model,
    System,
    Component
}

public struct Mesh
{
    public uint id;
    public uint color = uint.MaxValue;

    public Mesh()
    {
    }
}

internal class Application : Saket.Engine.Application
{
    World world;
    Pipeline pipeline_update;
    Pipeline pipeline_render;


    nint sdlwindow;
    WindowInfo windowinfo;

    GraphicsContext graphics;
    Surface surface;
    nint keyboardStatePtr;
    KeyboardState keyboardState;
    MouseState mouseState;

    ImGui_Impl_WebGPUSharp ImGuiImpl;

    RendererMesh meshRenderer;

    int score = 0;
    Tuple<Texture, TextureView, Sampler> depth;

    Resource_Camera camera;

    public List<Asset> assets = new();
    Assimp.AssimpContext a = new Assimp.AssimpContext();

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


            surface = CreateWebGPUSurfaceFromSDLWindow(graphics.instance, sdlwindow)!;

            SurfaceConfiguration config = new ()
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


            depth = GraphicsContext.CreateDeapthTexture(graphics.device, 1280, 720, TextureFormat.Depth32Float, "DepthBuffer");
        }

        {
            meshRenderer = new RendererMesh(graphics, new ());
        }


        // Setup IMGUI
        {
            IntPtr context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);

          
            var io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            //io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;


            var initInfo = new ImGui_Impl_WebGPUSharp.ImGui_ImplWGPU_InitInfo()
            {
                device = graphics.device,
                num_frames_in_flight = 3,
                rt_format = graphics.applicationpreferredFormat,
                depth_format = TextureFormat.Undefined,
            };

            ImGuiImpl = new ImGui_Impl_WebGPUSharp(initInfo);

            io.Fonts.AddFontDefault();
            io.Fonts.Build();


        }


        // Setup ecs
        {
            world = new();
            pipeline_update = new();
            pipeline_update.AddStage(
                new Stage()
                  .Add(Saket.Engine.Component_Camera.System_CameraControl)
                .Add(Saket.Engine.Component_Camera.System_Camera)
                );

        }

        // Setup Resources
        {
            windowinfo = new();
            windowinfo.width = initialWidth;
            windowinfo.height = initialHeight;
            world.SetResource(windowinfo);

            keyboardStatePtr = SDL.SDL_GetKeyboardState(out int numKeys);
            keyboardState = new(numKeys);
            world.SetResource(keyboardState);

            mouseState = new();
            world.SetResource(mouseState);
        }

        // Create entities
        {
            camera = new();
            camera.camera = world.CreateEntity()
                 .Add(new Transform() { Position = new Vector3(0, 0, -50) })
                 .Add(new CameraControl() { lookAt = true})
                 .Add(new Camera(CameraType.Perspective, 90f, 16f / 9f, 0.1f, 100f)).EntityPointer;
            world.SetResource(camera);
        }
    }

    public override void Update()
    {
        Vector2 scroll = new Vector2();
        // Poll Window events
        while (SDL.SDL_PollEvent(out var e) != 0)
        {
            // https://wiki.libsdl.org/SDL2/SDL_MouseWheelEvent
            if (e.type == SDL.SDL_EventType.SDL_MOUSEWHEEL)
            {
                scroll = new Vector2(e.wheel.preciseX, e.wheel.preciseY);
            }
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
                    windowinfo.width = e.window.data1;
                    windowinfo.height = e.window.data2;
                }
            }
            if(e.type == SDL.SDL_EventType.SDL_DROPFILE)
            {
                OnFileDropped(Marshal.PtrToStringUTF8(e.drop.file));
                
                
            }
        }
        // Inject input
        unsafe
        {
            Span<byte> bytes = new Span<byte>((void*)keyboardStatePtr, keyboardState.KeyCount);
            keyboardState.SetKeyboardState(bytes);
            //SDL.SDL_Scancode
            //SDL.SDL_Keycode
            //SDL.SDL_Scancode.SDL_SCANCODE_D
            var a = SDL.SDL_GetMouseState(out int x, out int y);
            mouseState.SetState((int)a, x, y);
            mouseState.SetScroll(scroll.X, scroll.Y);
        }

        // Poll graphic events
        graphics.instance.ProcessEvents();

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
            
            // Get the texture where to draw the next frame
            SurfaceTexture surfaceTexture = surface.GetCurrentTexture();
            if (surfaceTexture.Status != SurfaceGetCurrentTextureStatus.Success)
                return;
            TextureViewDescriptor viewdescriptor = new ()
            {
                Format = surfaceTexture.Texture.GetFormat(),
                Dimension = TextureViewDimension.D2,
                MipLevelCount = 1,
                BaseMipLevel = 0,
                BaseArrayLayer = 0,
                ArrayLayerCount = 1,
                Aspect = TextureAspect.All,
            };
            TextureView textureView = surfaceTexture.Texture.CreateView(viewdescriptor);
            // Clear the screen
            graphics.Clear(textureView, new Saket.Engine.Graphics.Color(0, 40, 0));

      

            {
                ImGuiIOPtr io = ImGui.GetIO();
                io.DisplaySize = new Vector2(windowinfo.width, windowinfo.height);
                io.DisplayFramebufferScale = Vector2.One;
                io.DeltaTime = (float)DeltaTime;
                MouseState MouseState = mouseState;
                KeyboardState KeyboardState = keyboardState;
                io.MouseDown[0] = MouseState[MouseButton.Left];
                io.MouseDown[1] = MouseState[MouseButton.Right];
                io.MouseDown[2] = MouseState[MouseButton.Middle];
                io.MousePos = new Vector2(MouseState.X, MouseState.Y);

                /*
                foreach (Keys key in Enum.GetValues<Keys>())
                {
                    io.AddKeyEvent((ImGuiKey)(key), KeyboardState.IsKeyDown(key));
                }*/

                ImGuiImpl.NewFrame();
                ImGui.NewFrame();
                //

                RenderUI();


                ImGui.EndFrame();

                ImGui.Render();


                // Command Encoder
                var commandEncoder = graphics.device.CreateCommandEncoder(new() {Label = "Main Command Encoder" });

                {
                    RenderPassDescriptor renderPassDesc = new()
                    {
                        ColorAttachments =
                    [
                        new()
                        {
                            View = textureView,
                            ResolveTarget = default,
                            LoadOp = LoadOp.Load,
                            StoreOp = StoreOp.Store,
                        }
                    ],
                        DepthStencilAttachment = new RenderPassDepthStencilAttachment()
                        {
                            View = depth.Item2,
                            DepthLoadOp = LoadOp.Clear,
                            DepthStoreOp = StoreOp.Store,
                            DepthClearValue = 1.0f,
                        },
                    };
                    var RenderPassEncoder = commandEncoder.BeginRenderPass(renderPassDesc);

                    Camera mainCam = world.GetEntity(camera.camera).Get<Camera>();
                    Matrix4x4 vp = mainCam.ViewMatrix * mainCam.projectionMatrix;

                    meshRenderer.RenderWorld(world, RenderPassEncoder, vp);

                    RenderPassEncoder.End();
                }

                {
                    RenderPassDescriptor renderPassDesc = new()
                    {
                        ColorAttachments =
                        [
                            new()
                            {
                                View = textureView,
                                ResolveTarget = default,
                                LoadOp = LoadOp.Load,
                                StoreOp = StoreOp.Store,
                            }
                        ],
                        DepthStencilAttachment = null
                    };
                    var RenderPassEncoder = commandEncoder.BeginRenderPass(renderPassDesc);

                    ImGuiImpl.RenderDrawData(ImGui.GetDrawData(), RenderPassEncoder);

                    RenderPassEncoder.End();
                }
              


                // Finish Rendering
           
                var commandBuffer = commandEncoder.Finish(new() { });

                //GC.Collect(); // To test if gc is causing interop issues.
                graphics.queue.Submit(commandBuffer);

                
                
            }

            // Preset swapchain
            surface.Present();
           
        }

    }

    void RenderUI()
    {
        ImGui.ShowDemoWindow();
        //return;
        //ImGui.SetNextWindowPos(Vector2.Zero);
        //ImGui.SetNextWindowSize(windowinfo.Size);


        //var dock = ImGui.DockSpaceOverViewport(0, 0, ImGuiDockNodeFlags.None, 0);

        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Edit"))
                {
                    if (ImGui.MenuItem("Undo", "CTRL+Z")) { }
                    if (ImGui.MenuItem("Redo", "CTRL+Y", false, false)) { }  // Disabled item
                    ImGui.Separator();
                    if (ImGui.MenuItem("Cut", "CTRL+X")) { }
                    if (ImGui.MenuItem("Copy", "CTRL+C")) { }
                    if (ImGui.MenuItem("Paste", "CTRL+V")) { }
                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }
        }
        {
            //ImGui.SetNextWindowDockID(dock);
            
            ImGui.Begin("a");
            ImGui.Button("C");


            var mainCam = world.GetEntity(camera.camera);
            var transform = mainCam.Get<Transform>();
            var cam = mainCam.Get<Camera>();
            ImGui.Text(transform.Position.ToString());
            ImGui.Text(ToEulerAngles(transform.Rotation).ToString());

            ImGui.End();
        }
 

        {
            //ImGui.SetNextWindowDockID(dock);
            ImGui.Begin("Assets");
            ImGui.BeginTable("hi", 4, 
                ImGuiTableFlags.Sortable
                );

            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("Path");
            ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.NoResize | ImGuiTableColumnFlags.NoDirectResize | ImGuiTableColumnFlags.WidthFixed, 50f);
            ImGui.TableSetupColumn(null, ImGuiTableColumnFlags.NoResize | ImGuiTableColumnFlags.NoDirectResize | ImGuiTableColumnFlags.WidthFixed , 32f);
            ImGui.TableHeadersRow();

            for (int i = 0; i < assets.Count; i++)
            {
               
                ImGui.TableNextRow();
               
                ImGui.TableNextColumn();
                ImGui.Selectable(assets[i].name, true, ImGuiSelectableFlags.SpanAllColumns | ImGuiSelectableFlags.AllowOverlap);
                if (ImGui.BeginDragDropSource(ImGuiDragDropFlags.SourceAllowNullID))
                {
                    ImGui.SetDragDropPayload(assets[i].name, 0, 0);
                    ImGui.Text(assets[i].name);
                    ImGui.EndDragDropSource();
                }
                ImGui.TableNextColumn();
                ImGui.Text(assets[i].path);
                ImGui.TableNextColumn();
                ImGui.Text(assets[i].filetype);
                ImGui.TableNextColumn();
               if( ImGui.Button("Add", new Vector2(32, 16)))
                {
                    var ms = meshIDs[assets[i].path];
                    for (int m = 0; m < ms.Length; m++)
                    {
                        world.CreateEntity()
                           .Add(new Transform() { })
                           .Add(new Mesh() { id = ms[m], color = uint.MaxValue });
                    }
               }
             

            }
            ImGui.EndTable();
            ImGui.End();

        }
     
        {
            //ImGui.SetNextWindowDockID(dock);
            ImGui.Begin("Hierarchy");
            

            ImGui.End();

        }

    }
    public static Vector3 ToEulerAngles(Quaternion q)
    {
        Vector3 angles = new();

        // roll / x
        double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
        double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
        angles.X = (float)Math.Atan2(sinr_cosp, cosr_cosp);

        // pitch / y
        double sinp = 2 * (q.W * q.Y - q.Z * q.X);
        if (Math.Abs(sinp) >= 1)
        {
            angles.Y = (float)Math.CopySign(Math.PI / 2, sinp);
        }
        else
        {
            angles.Y = (float)Math.Asin(sinp);
        }

        // yaw / z
        double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
        double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
        angles.Z = (float)Math.Atan2(siny_cosp, cosy_cosp);

        return angles;
    }
    void OnFileDropped(string path)
    {
        if (!a.IsImportFormatSupported(Path.GetExtension(path)))
            return;

        assets.Add(new Asset() { 
            path = path, 
            name = Path.GetFileNameWithoutExtension(path),
            filetype = Path.GetExtension(path)
        });

        meshIDs.Add(path, meshRenderer.UploadScene(a.ImportFile(path), path));
        return;
    }
    Dictionary<string, uint[]> meshIDs = new Dictionary<string, uint[]>();

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
                SurfaceDescriptor descriptor_surface = new (ref wsDescriptor);
                return instance.CreateSurface(descriptor_surface);
            }

            throw new Exception("Platform not supported");
        }
    }
}