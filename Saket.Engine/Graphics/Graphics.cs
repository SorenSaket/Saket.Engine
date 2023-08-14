using Saket.Engine.Platform;
using Saket.Engine.Platform.Win;
using Saket.WebGPU;
using Saket.WebGPU.Native;
using Saket.WebGPU.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Graphics
{
    public struct GraphicsWindow
    {
        public Surface surface;
        public WGPUTextureFormat preferredFormat;
        public Swapchain swapchain;
        public readonly TextureView GetCurretTextureView() => swapchain.GetCurrentTextureView();
    }

    /// <summary>
    /// Helper functions for webgpu
    /// </summary>
    public class Graphics
    {
        public Instance instance;
        public Adapter adapter;
        public Device device;
        public Saket.WebGPU.Objects.Queue queue;

        public Sampler defaultSampler;

        public Dictionary<Window, GraphicsWindow> windows;

        // By assuming all surfaces are prefering the same texture format?
        public WGPUTextureFormat applicationpreferredFormat;



        public WebGPU.Objects.Buffer systemBuffer;
        public BindGroup systemBindGroup;
        public BindGroupLayout systemBindGroupLayout;

        public Graphics()
        {
#if DEBUG
            Environment.SetEnvironmentVariable("RUST_BACKTRACE", "1");
            Environment.SetEnvironmentVariable("RUST_BACKTRACE", "full");
#endif

            instance = new Instance();
            
            // Create Adapter
            adapter = instance.RequestAdapter(new()
            {
                powerPreference = WGPUPowerPreference.HighPerformance
            });
            // Create Device
            device = adapter.CreateDevice(null);

            queue = device.GetQueue();

            windows = new Dictionary<Window, GraphicsWindow>();

            defaultSampler = device.CreateSampler(new WGPUSamplerDescriptor());


            // Create system uniform bindgroup
            unsafe
            {

                systemBuffer =  device.CreateBuffer(new WGPUBufferDescriptor()
                {
                    size = (ulong)sizeof(SystemUniform),
                    usage = WGPUBufferUsage.Uniform | WGPUBufferUsage.CopyDst,
                });

                systemBindGroupLayout = device.CreateBindGroupLayout(stackalloc WGPUBindGroupLayoutEntry[]
                {
                    new()
                    {
                        binding = 0,
                        buffer = new()
                        {
                            type = WGPUBufferBindingType.Uniform,
                        }
                    }
                });

                systemBindGroup = device.CreateBindGroup(systemBindGroupLayout, stackalloc WGPUBindGroupEntry[]
                {
                    new()
                    {
                        binding = 0,
                        buffer = systemBuffer.Handle,
                        size = (ulong)sizeof(SystemUniform),
                    }
                });
            }

        }

     


        public void SetSystemUniform(SystemUniform uniform)
        {
            unsafe
            {
                queue.WriteBuffer(systemBuffer, 0, &uniform, (nuint)sizeof(SystemUniform));
            }
        }

        public unsafe void AddWindow(Window window)
        {
            if (windows.ContainsKey(window))
                return;


            GraphicsWindow graphicsWindow = new();

            if (window is Window_Windows windowsWindow)
            {
                graphicsWindow.surface = instance.CreateSurfaceFromWindowsHWND(windowsWindow.hInstance, windowsWindow.windowHandle);

                graphicsWindow.preferredFormat = applicationpreferredFormat = graphicsWindow.surface.GetPreferredFormat(adapter);

                WGPUSwapChainDescriptor swapChainDescriptor = new()
                {
                    format = graphicsWindow.preferredFormat,
                    height = 720,
                    width = 1280,
                    usage = WGPUTextureUsage.RenderAttachment,
                    presentMode = WGPUPresentMode.Fifo
                };
                graphicsWindow.swapchain = device.CreateSwapchain(graphicsWindow.surface, swapChainDescriptor);
            }

            windows.Add(window, graphicsWindow);
        }
    }

    public struct SystemUniform
    {
        public Matrix4x4 projectionMatrix;
        public Matrix4x4 viewMatrix;
        public UInt32 frame;
    }
}