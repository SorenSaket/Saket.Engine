using Saket.Engine.Platform;
using WebGpuSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WebGpuSharp.FFI;

namespace Saket.Engine.Graphics
{

    public class WGPUWindow
    {
        public Window window;
        public Surface surface;
        public TextureFormat preferredFormat;
        public SwapChain swapchain;
        

        public TextureView GetCurretTextureView()
        {
            return swapchain.GetCurrentTextureView();
        }
    }

    /// <summary>
    /// Helper functions for webgpu
    /// </summary>
    public class GraphicsContext
    {
        public Instance instance;
        public Adapter adapter;
        public Device device;
        public WebGpuSharp.Queue queue;

        public Sampler defaultSampler;

        public Dictionary<Saket.Engine.Platform.Window, WGPUWindow> windows;

        // By assuming all surfaces are prefering the same texture format?
        public TextureFormat applicationpreferredFormat = TextureFormat.Undefined;



        public WebGpuSharp.Buffer systemBuffer;
        public BindGroup systemBindGroup;
        public BindGroupLayout systemBindGroupLayout;

        public GraphicsContext()
        {
#if DEBUG
            Environment.SetEnvironmentVariable("RUST_BACKTRACE", "1");
            Environment.SetEnvironmentVariable("RUST_BACKTRACE", "full");
#endif
            windows = new();
            instance = WebGPU.CreateInstance()!;
            
            // Create Adapter
            adapter =  instance.RequestAdapterAsync(new()
            {
                PowerPreference = PowerPreference.HighPerformance
            }).GetAwaiter().GetResult()!;
            // Create Device
            DeviceDescriptor deviceDescriptor = new DeviceDescriptor();
            device = adapter.RequestDeviceAsync(deviceDescriptor).GetAwaiter().GetResult()!;

            queue = device.GetQueue()!;

            defaultSampler = device.CreateSampler(new SamplerDescriptor() { })!;


            // Create system uniform bindgroup
            unsafe
            {

                systemBuffer =  device.CreateBuffer(new BufferDescriptor()
                {
                    Size = (ulong)sizeof(SystemUniform),
                    Usage = BufferUsage.Uniform | BufferUsage.CopyDst,
                })!;

                systemBindGroupLayout = device.CreateBindGroupLayout(new BindGroupLayoutDescriptor()
                {
                    Entries = stackalloc BindGroupLayoutEntry[]
                    {
                        new()
                        {
                            Binding = 0,
                            Visibility = ShaderStage.Vertex | ShaderStage.Fragment | ShaderStage.Compute,
                            Buffer = new()
                            {
                                Type = BufferBindingType.Uniform,
                            }
                        }

                    }
                })!;

                var b = new BindGroupEntry[]
                {
                    new()
                    {
                        Binding = 0,
                        Buffer = systemBuffer,
                        Size = (ulong)sizeof(SystemUniform),
                    }
                };

                systemBindGroup = device.CreateBindGroup(new BindGroupDescriptor()
                {
                    Layout = systemBindGroupLayout,
                    Entries = b
                })!;
            }

        }


        public void Clear(TextureView target, Saket.Engine.Graphics.Color clearColor)
        {
            unsafe
            {
                var ColorAttachments = new RenderPassColorAttachment[]
                {
                     new()
                    {
                        View = target,
                        ResolveTarget = null,
                        LoadOp = LoadOp.Clear,
                        StoreOp = StoreOp.Store,
                        ClearValue = clearColor,
                    }
                };
               
                RenderPassDescriptor renderPassDesc = new()
                {
                    ColorAttachments = ColorAttachments,
                    DepthStencilAttachment = null,
                    TimestampWrites = null
                };

                // Command Encoder
                var commandEncoder = device.CreateCommandEncoder(new() { })!;

                var RenderPassEncoder = commandEncoder.BeginRenderPass(renderPassDesc);

                var commandBuffer = commandEncoder.Finish(new() { })!;

                queue.Submit(commandBuffer);

               // wgpu.RenderPassEncoderRelease(RenderPassEncoder);
               // wgpu.CommandEncoderRelease(commandEncoder);
            }
        }


        public void SetSystemUniform(SystemUniform uniform)
        {
            unsafe
            {
                queue.WriteBuffer(systemBuffer, 0, stackalloc SystemUniform[] { uniform } );
            }
        }

        public unsafe void AddWindow(Platform.Window window)
        {
            if (windows.ContainsKey(window))
                return;

            var ww = new WGPUWindow();
            ww.window = window;
#if true
            // TODO. Move this somewhere else. This is cyclical reference hell
            if (window is Platform.MSWindows.Windowing.Window windowsWindow)
            {
                var wsDescriptor = new WebGpuSharp.FFI.SurfaceDescriptorFromWindowsHWNDFFI()
                {
                    Hinstance =(void*) windowsWindow.HInstance,
                    Hwnd = (void*)windowsWindow.WindowHandle,
                };
                SurfaceDescriptor descriptor_surface = new SurfaceDescriptor(ref wsDescriptor);
                ww.surface = instance.CreateSurface(descriptor_surface);

                if(applicationpreferredFormat == TextureFormat.Undefined)
                {
                    ww.preferredFormat = applicationpreferredFormat = ww.surface.GetPreferredFormat(adapter);
                }
                else
                {
                    ww.preferredFormat = applicationpreferredFormat;
                }

                SwapChainDescriptor swapChainDescriptor = new()
                {
                    Format = ww.preferredFormat,
                    Height = 720,
                    Width = 1280,
                    Usage = TextureUsage.RenderAttachment,
                    PresentMode = PresentMode.Fifo,
                };
                ww.swapchain = device.CreateSwapChain(ww.surface, swapChainDescriptor)!;
            }
#endif 
            windows.Add(window, ww);
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