using Saket.Engine;
using Saket.Engine.Platform.Win;
using System;
using System.Runtime.InteropServices;
using Saket.WebGPU;
using System.Threading.Tasks;
using System.Text;
using Saket.WebGPU.Native;

namespace Saket.Engine.Example
{
    // https://learn.microsoft.com/en-us/windows/win32/learnwin32/creating-a-window

    public static class Program
    {
        static string shaderSource = "@vertex\r\nfn vs_main(@builtin(vertex_index) in_vertex_index: u32) -> @builtin(position) vec4<f32> {\r\n\tvar p = vec2<f32>(0.0, 0.0);\r\n\tif (in_vertex_index == 0u) {\r\n\t\tp = vec2<f32>(-0.5, -0.5);\r\n\t} else if (in_vertex_index == 1u) {\r\n\t\tp = vec2<f32>(0.5, -0.5);\r\n\t} else {\r\n\t\tp = vec2<f32>(0.0, 0.5);\r\n\t}\r\n\treturn vec4<f32>(p, 0.0, 1.0);\r\n}\r\n\r\n@fragment\r\nfn fs_main() -> @location(0) vec4<f32> {\r\n    return vec4<f32>(0.0, 0.4, 1.0, 1.0);\r\n}";
        
        [STAThread]
		static unsafe void Main()
        {
            // Create the instance

            WGPUInstanceDescriptor desc = new WGPUInstanceDescriptor() { };
            nint instance = wgpu.CreateInstance(desc);

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
            nint adapter = WebGPU.Helper.RequestAdapter(instance, options);
            if(adapter == 0)
                throw new Exception("Could not initialize WebGPU");
            
            
            // Create Device
            var descriptor = new WGPUDeviceDescriptor(){};
            nint device = WebGPU.Helper.RequestDevice(adapter, descriptor);
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

            nint swapchain = wgpu.DeviceCreateSwapChain(device, surface, swapChainDescriptor);
            
            if (swapchain == 0)
                throw new Exception("Could not initialize WebGPU");
           
            // Create queue
            nint queue = wgpu.DeviceGetQueue(device);

            if (queue == 0)
                throw new Exception("Could not initialize WebGPU");

            // Create shader
            nint shaderModule = 0;
           
            WGPUShaderModuleWGSLDescriptor shaderCodeDesc = new()
            {
                chain = new WGPUChainedStruct() { next = null, sType = WGPUSType.ShaderModuleWGSLDescriptor },
                code = shaderSource,
            };
            WGPUShaderModuleDescriptor shaderDesc = new WGPUShaderModuleDescriptor() { 
                hintCount = 0,
                hints = null,
                nextInChain = &shaderCodeDesc.chain
            };

            shaderModule = wgpu.DeviceCreateShaderModule(device, shaderDesc);

            if (shaderModule == 0)
                throw new Exception("failed to create shader");
            

            // Create pipeline
            nint pipeline = 0;
            {
                WGPUPipelineLayoutDescriptor layoutDesc = new() { 
                };
                nint layout = wgpu.DeviceCreatePipelineLayout(device, layoutDesc);

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
                    entryPoint = "fs_main",
                    module = shaderModule,
                    targets = &colorTarget,
                    targetCount = 1
                };

                WGPURenderPipelineDescriptor pipeline_render = new()
                {
                    vertex = new()
                    {
                        module = shaderModule,
                        entryPoint = "vs_main",
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
                nint encoder = wgpu.DeviceCreateCommandEncoder(device, encoderDesc);

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
                nint renderPass = wgpu.CommandEncoderBeginRenderPass(encoder, renderPassDesc);

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
                wgpu.QueueSubmit(queue, 1, command);
                
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


            // cleanup
            wgpu.SwapChainRelease(swapchain);
            wgpu.DeviceRelease(device);
            wgpu.AdapterRelease(adapter);
            wgpu.InstanceRelease(instance);

            Platform_Windows_PInvoke.DestroyWindow(windowHandle);

        }
    }
}