using Saket.Engine.Platform;
using Saket.Engine.Platform.Windows;
using Saket.WebGPU;
using Saket.WebGPU.Native;
using Saket.WebGPU.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Graphics
{
    /// <summary>
    /// Helper functions for webgpu
    /// </summary>
    public class GraphicsContext
    {
        public Instance instance;
        public Adapter adapter;
        public Device device;
        public Saket.WebGPU.Objects.Queue queue;

        public Sampler defaultSampler;

        public List<Platform.Window> windows;

        // By assuming all surfaces are prefering the same texture format?
        public WGPUTextureFormat applicationpreferredFormat = WGPUTextureFormat.Undefined;



        public WebGPU.Objects.Buffer systemBuffer;
        public BindGroup systemBindGroup;
        public BindGroupLayout systemBindGroupLayout;

        public GraphicsContext()
        {
#if DEBUG
            Environment.SetEnvironmentVariable("RUST_BACKTRACE", "1");
            Environment.SetEnvironmentVariable("RUST_BACKTRACE", "full");
#endif
            windows = new List<Platform.Window>();
            instance = new Instance();
            
            // Create Adapter
            adapter = instance.RequestAdapter(new()
            {
                powerPreference = WGPUPowerPreference.HighPerformance
            });
            // Create Device
            device = adapter.CreateDevice(null);

            queue = device.GetQueue();

            defaultSampler = device.CreateSampler(new WGPUSamplerDescriptor() { });


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
                        visibility = WGPUShaderStage.Vertex | WGPUShaderStage.Fragment | WGPUShaderStage.Compute,
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
                }, "bingroup_system"u8);
            }

        }

     


        public void SetSystemUniform(SystemUniform uniform)
        {
            unsafe
            {
                queue.WriteBuffer(systemBuffer, 0, &uniform, (nuint)sizeof(SystemUniform));
            }
        }

        public unsafe void AddWindow(Platform.Window window)
        {
            if (windows.Contains(window))
                return;

            if (window is Platform.Windows.Window windowsWindow)
            {
                window.surface = instance.CreateSurfaceFromWindowsHWND(windowsWindow.HInstance, windowsWindow.WindowHandle);

                if(applicationpreferredFormat == WGPUTextureFormat.Undefined)
                {
                    window.preferredFormat = applicationpreferredFormat = window.surface.GetPreferredFormat(adapter);
                }
                else
                {
                    window.preferredFormat = applicationpreferredFormat;
                }

                WGPUSwapChainDescriptor swapChainDescriptor = new()
                {
                    format = window.preferredFormat,
                    height = 720,
                    width = 1280,
                    usage = WGPUTextureUsage.RenderAttachment,
                    presentMode = WGPUPresentMode.Fifo,
                };
                window.swapchain = device.CreateSwapchain(window.surface, swapChainDescriptor);
            }

            windows.Add(window);
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 144)]
    public struct SystemUniform
    {
        [FieldOffset(0)]
        public Matrix4x4 projectionMatrix;
        [FieldOffset(64)]
        public Matrix4x4 viewMatrix;
        [FieldOffset(128)]
        public UInt32 frame;
    }
}