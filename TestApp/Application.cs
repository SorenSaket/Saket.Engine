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

namespace TestApp;



internal class Application : Saket.Engine.Application
{
    nint sdlwindow;
    WindowInfo windowinfo;

    GraphicsContext graphics;
    Surface surface;
    nint keyboardStatePtr;
    KeyboardState keyboardState;
    MouseState mouseState;


    ImGui_Impl_WebGPUSharp ImGuiImpl;

    int score = 0;

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


        // Setup Resources
        {
            windowinfo = new();
            windowinfo.width = initialWidth;
            windowinfo.height = initialHeight;

            keyboardStatePtr = SDL.SDL_GetKeyboardState(out int numKeys);
            keyboardState = new(numKeys);

            mouseState = new();
        }

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
                    windowinfo.width = e.window.data1;
                    windowinfo.height = e.window.data2;
                }
            }
        }
        // Inject input
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
            //graphics.Clear(textureView, new Saket.Engine.Graphics.Color(0, 40, 0));

            {
                ImGuiIOPtr io = ImGui.GetIO();
                io.DisplaySize = new Vector2(1280, 720);
                io.DisplayFramebufferScale = Vector2.One;
                io.DeltaTime = (float)DeltaTime;
                
                ImGuiImpl.NewFrame();
                ImGui.NewFrame();
                //

                ImGui.Begin("a");
                ImGui.Text("Wif(e)y game");
                ImGui.Text($"You currently have {score} points");
                if (ImGui.Button("Click to get a point"))
                    score++;

                if (ImGui.Button("Buy kiss 5 points"))
                {
                    if (score >= 5)
                        score -= 5;
                }

                if (ImGui.Button("Buy marrige 999.999 points"))
                {
                    if (score >= 999999)
                        score -= 999999;
                }


                ImGui.End();
               


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
                GC.Collect(); // To test if gc is causing interop issues.
                graphics.queue.Submit(commandBuffer);
            }

            // Preset swapchain
            surface.Present();
           
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
                SurfaceDescriptor descriptor_surface = new (ref wsDescriptor);
                return instance.CreateSurface(descriptor_surface);
            }

            throw new Exception("Platform not supported");
        }
    }
}