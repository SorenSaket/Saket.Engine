using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using Saket.Engine;
using Saket.Engine.Platform.Windows;
using System;
using System.Runtime.InteropServices;
using Saket.Engine.WebGPU;
using System.Threading.Tasks;
using System.Text;

namespace Saket.Engine.Example
{
    // https://learn.microsoft.com/en-us/windows/win32/learnwin32/creating-a-window

    public static class Program
    {
        static byte[] shaderSource = Encoding.UTF8.GetBytes("@vertex\r\nfn vs_main(@builtin(vertex_index) in_vertex_index: u32) -> @builtin(position) vec4<f32> {\r\n\tvar p = vec2<f32>(0.0, 0.0);\r\n\tif (in_vertex_index == 0u) {\r\n\t\tp = vec2<f32>(-0.5, -0.5);\r\n\t} else if (in_vertex_index == 1u) {\r\n\t\tp = vec2<f32>(0.5, -0.5);\r\n\t} else {\r\n\t\tp = vec2<f32>(0.0, 0.5);\r\n\t}\r\n\treturn vec4<f32>(p, 0.0, 1.0);\r\n}\r\n\r\n@fragment\r\nfn fs_main() -> @location(0) vec4<f32> {\r\n    return vec4<f32>(0.0, 0.4, 1.0, 1.0);\r\n}");

       [STAThread]
		static unsafe void Main()
        {
            // Create the instance

            WebGPU.WGPUInstanceDescriptor desc = new WebGPU.WGPUInstanceDescriptor() { };
            nint instance = wgpu.CreateInstance(ref desc);

            if (instance == 0)
                throw new Exception("Could not initialize WebGPU");

            #region Create Window
            void ThrowWin32Error()
            {
                var err = Platform_Windows_PInvoke.GetLastError();
                if (err != 0)
                    throw new Exception();
            }

            var hInstance = System.Diagnostics.Process.GetCurrentProcess().Handle;

            // The function to be called every frame
            Platform_Windows_PInvoke.del_WindowProc p = (IntPtr hwnd, WindowMessage uMsg, IntPtr wParam, IntPtr lParam) =>
            {
                return Platform_Windows_Del.DefWindowProc(hwnd, uMsg, wParam, lParam);
            };


            string className = "mainclass";
            var windowclass = new WNDCLASSEX()
            {
                cbSize = (uint)Marshal.SizeOf<WNDCLASSEX>(),
                style =
                ClassStyles.VerticalRedraw |
                ClassStyles.HorizontalRedraw,
                lpfnWndProc = Marshal.GetFunctionPointerForDelegate(p),
                lpszClassName = className,
                hInstance = hInstance,
            };

            var classAtom = Platform_Windows_PInvoke.RegisterClassExW(ref windowclass);

            ThrowWin32Error();

            var windowHandle = Platform_Windows_PInvoke.CreateWindowExW(
                0,
                className,
                className,
                WindowStyles.WS_OVERLAPPEDWINDOW,
                0, 0, 1280, 720,
                IntPtr.Zero, IntPtr.Zero, hInstance, IntPtr.Zero);


            bool visible = Platform_Windows_PInvoke.ShowWindow(windowHandle, 5);
            #endregion

            #region WGPU
            
            // Create surface from windows HWND
            nint surface = WebGPU.Helper.CreateSurfaceWin(instance, hInstance, windowHandle);


            // Create Adapter
            WGPURequestAdapterOptions options = new WGPURequestAdapterOptions() { 
                compatibleSurface = surface
            };
            nint adapter = WebGPU.Helper.RequestAdapter(instance, ref options);
            if(adapter == 0)
                throw new Exception("Could not initialize WebGPU");
            
            
            // Create Device
            var descriptor = new WGPUDeviceDescriptor(){};
            nint device = WebGPU.Helper.RequestDevice(adapter, ref descriptor);
            if (device == 0)
                throw new Exception("Could not initialize WebGPU");

            // Device Error callback
            unsafe
            {
                wgpu.DeviceSetUncapturedErrorCallback(device, (WGPUErrorType type,
                  char* message,
                  void* userdata) => {

                      Console.WriteLine(new string(message));

                  }, null);
            }

            // Create Swapchain
            WGPUTextureFormat swapchainFormat = wgpu.SurfaceGetPreferredFormat(surface, adapter);
            WGPUSwapChainDescriptor swapChainDescriptor = new()
            {
                format = swapchainFormat,
                height = 720,
                width = 1280,
                usage = WGPUTextureUsage.RenderAttachment,
                presentMode = WGPUPresentMode.Fifo
            };

            nint swapchain = wgpu.DeviceCreateSwapChain(device, surface, ref swapChainDescriptor);

            if (swapchain == 0)
                throw new Exception("Could not initialize WebGPU");
           
            // Create queue
            nint queue = wgpu.DeviceGetQueue(device);

            if (queue == 0)
                throw new Exception("Could not initialize WebGPU");

            // Create shader
            nint shaderModule = 0;
            fixed(byte* ptr = shaderSource)
            {
                WGPUShaderModuleWGSLDescriptor shaderCodeDesc = new()
                {
                    chain = new WGPUChainedStruct() { next = null, sType = WGPUSType.ShaderModuleWGSLDescriptor },
                    code = (char*)ptr,
                };
                WGPUShaderModuleDescriptor shaderDesc = new WGPUShaderModuleDescriptor() { 
                    hintCount = 0,
                    hints = null,
                    nextInChain = &shaderCodeDesc.chain
                };

                shaderModule = wgpu.DeviceCreateShaderModule(device, ref shaderDesc);

                if (shaderModule == 0)
                    throw new Exception("failed to create shader");
            }

            // Create pipeline
            nint pipeline = 0;
            fixed (byte* entrypoint_vertex = "vs_main"u8)
            fixed (byte* entrypoint_fragment = "fs_main"u8)

            {
                WGPUPipelineLayoutDescriptor layoutDesc = new();
                nint layout = wgpu.DeviceCreatePipelineLayout(device, ref layoutDesc);

                WGPUBlendState blendState = new()
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

                WGPUColorTargetState colorTarget = new()
                {
                    format = swapchainFormat,
                    blend = &blendState,
                    writeMask = WGPUColorWriteMask.All
                };

                WGPUFragmentState fragment = new()
                {
                    entryPoint = (char*)entrypoint_fragment,
                    module = shaderModule,
                    targets = &colorTarget,
                    targetCount = 1
                };

                WGPURenderPipelineDescriptor pipeline_render = new()
                {
                    vertex = new()
                    {
                        module = shaderModule,
                        entryPoint = (char*) entrypoint_vertex,
                    },
                    primitive = new()
                    {
                        topology = WGPUPrimitiveTopology.TriangleList,
                        stripIndexFormat = WGPUIndexFormat.Undefined,
                        frontFace = WGPUFrontFace.CCW,
                        cullMode = WGPUCullMode.None,
                    },
                    fragment = &fragment,
                    depthStencil = null,
                    multisample = new()
                    {
                        count = 1,
                        mask = ~0u,
                        alphaToCoverageEnabled = false
                    },
                    layout = layout
                };

                pipeline = wgpu.DeviceCreateRenderPipeline(device, ref pipeline_render);
            }

            if (pipeline == 0)
                throw new Exception("Could not initialize WebGPU");


            void UpdateLoop()
            {
                // Get the texture where to draw the next frame
                nint nextTexture = wgpu.SwapChainGetCurrentTextureView(swapchain);

                // PANIC if the next texture is invalid
                if (nextTexture == 0)
                {
                    throw new Exception("could not get next texture view"); 
                }

                // Command Encover
                WGPUCommandEncoderDescriptor encoderDesc = new()
                {

                };
                nint encoder = wgpu.DeviceCreateCommandEncoder(device, ref encoderDesc);

                // RenderPass
                WGPURenderPassColorAttachment renderPassColorAttachment = new()
                {
                    view = nextTexture,
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
                nint renderPass = wgpu.CommandEncoderBeginRenderPass(encoder, ref renderPassDesc);

                // Set the pipline for the renderpass
                wgpu.RenderPassEncoderSetPipeline(renderPass, pipeline);

                wgpu.RenderPassEncoderDraw(renderPass, 3, 1, 0, 0);

                wgpu.RenderPassEncoderEnd(renderPass);

                // We can now release the textureview
                wgpu.TextureViewRelease(nextTexture);


                WGPUCommandBufferDescriptor cmdBufferDesc = new WGPUCommandBufferDescriptor()
                {
                    
                };

                nint command = wgpu.CommandEncoderFinish(encoder, ref cmdBufferDesc);
                wgpu.QueueSubmit(queue, 1, ref command);
                
                // 
                wgpu.SwapChainPresent(swapchain);
            }


            #endregion




            #region Run Window


            ThrowWin32Error();

            WindowMessage message;

            while (true)
            {
                var result = Platform_Windows_PInvoke.GetMessage(out message, 0, 0, 0);

                if (message == WindowMessage.DESTROY)
                    break;

                if (result > 0)
                {
                    Platform_Windows_PInvoke.TranslateMessage(ref message);
                    Platform_Windows_PInvoke.DispatchMessage(ref message);
                }
                else
                {
                    break;
                }

                UpdateLoop();
            }

            ThrowWin32Error();
            #endregion



            wgpu.SwapChainRelease(swapchain);
            wgpu.DeviceRelease(device);
            wgpu.AdapterRelease(adapter);
            wgpu.InstanceRelease(instance);

            Platform_Windows_PInvoke.DestroyWindow(windowHandle);








            /*
            GameWindowSettings gameWindowSettings = new GameWindowSettings();
          
            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings();
            nativeWindowSettings.Size = new OpenTK.Mathematics.Vector2i(1280, 720);
            nativeWindowSettings.Flags = OpenTK.Windowing.Common.ContextFlags.Debug;

                
            var game = new ExampleProgram(gameWindowSettings, nativeWindowSettings);
#if DEBUG
            GL.DebugMessageCallback(OnDebugMessage, IntPtr.Zero);
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);
#endif
            game.Run();*/
        }




       





















        private static void OnDebugMessage(
        DebugSource source,     // Source of the debugging message.
        DebugType type,         // Type of the debugging message.
        int id,                 // ID associated with the message.
        DebugSeverity severity, // Severity of the message.
        int length,             // Length of the string in pMessage.
        IntPtr pMessage,        // Pointer to message string.
        IntPtr pUserParam)      // The pointer you gave to OpenGL, explained later.
        {
            // In order to access the string pointed to by pMessage, you can use Marshal
            // class to copy its contents to a C# string without unsafe code. You can
            // also use the new function Marshal.PtrToStringUTF8 since .NET Core 1.1.
            string message = Marshal.PtrToStringAnsi(pMessage, length);

            // The rest of the function is up to you to implement, however a debug output
            // is always useful.
            Console.WriteLine("[{0} source={1} type={2} id={3}] {4}", severity, source, type, id, message);

            // Potentially, you may want to throw from the function for certain severity
            // messages.
            if (type == DebugType.DebugTypeError)
            {
                throw new Exception(message);
            }
        }
    }
}