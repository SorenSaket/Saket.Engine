using WebGpuSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WebGpuSharp.FFI;
using System.Text.Unicode;

namespace Saket.Engine.Graphics
{
    public class RenderTarget
    {
        public int Width;
        public int Height;

        public float AspectRatio { get; internal set; }
        public TextureFormat Format;

        public readonly Surface Surface;
        public readonly TextureFormat PreferredFormat;

        public SwapChain Swapchain { get; internal set; }


        internal RenderTarget(Surface surface, TextureFormat preferredFormat)
        {
            this.Surface = surface;
            this.PreferredFormat = preferredFormat;
        }

        public TextureView GetCurretTextureView()
        {
            return Swapchain.GetCurrentTextureView();
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


        // By assuming all surfaces are prefering the same texture format?
        public TextureFormat applicationpreferredFormat = TextureFormat.Undefined;

        public WebGpuSharp.Buffer systemBuffer;
        public BindGroup systemBindGroup;
        public BindGroupLayout systemBindGroupLayout;

        public GraphicsContext()
        {
            // Setup WebGPU
            {
                // This is to enable debug logging with wgpu
#if DEBUG
                Environment.SetEnvironmentVariable("RUST_BACKTRACE", "1");
                Environment.SetEnvironmentVariable("RUST_BACKTRACE", "full");
#endif

                // Create WebGPU instance 
                instance = WebGPU.CreateInstance()!;

                // Create Adapter
                adapter = instance.RequestAdapterAsync(new()
                {
                    PowerPreference = PowerPreference.HighPerformance,
                    BackendType = BackendType.D3D12
                }).GetAwaiter().GetResult() ?? throw new Exception("Failed to acquire Adapter");

                SupportedLimits supportedLimits = adapter.GetLimits()!.Value;

                // Create Device
                DeviceDescriptor deviceDescriptor = new DeviceDescriptor() { RequiredLimits = new WGPUNullableRef<RequiredLimits>(new RequiredLimits(supportedLimits.Limits)), DefaultQueue = new QueueDescriptor() };
                device = adapter.RequestDeviceAsync(deviceDescriptor).GetAwaiter().GetResult() ?? throw new Exception("Failed to acquire Device");

                device.AddUncapturedErrorCallback((ErrorType type, ReadOnlySpan<byte> message) =>
                {
                    Console.Error.WriteLine($"{Enum.GetName(type)} : {Encoding.UTF8.GetString(message)}");
                });
                device.AddDeviceLostCallback((DeviceLostReason lostReason, ReadOnlySpan<byte> message) => {
                    Console.Error.WriteLine($"Device lost! Reason: {Enum.GetName(lostReason)} : {Encoding.UTF8.GetString(message)}");
                });
            }

            // Create Queue
            queue = device.GetQueue()!;

            // Create the default sampler. For miscellaneous uses
            defaultSampler = device.CreateSampler(new SamplerDescriptor()
            {
                AddressModeU = AddressMode.ClampToEdge,
                AddressModeV = AddressMode.ClampToEdge,
                AddressModeW = AddressMode.ClampToEdge,
                MagFilter = FilterMode.Nearest,
                MinFilter = FilterMode.Nearest,
                MipmapFilter = MipmapFilterMode.Nearest,
                LodMinClamp = 0,
                LodMaxClamp = 32,
                Compare = CompareFunction.Undefined,
                MaxAnisotropy = 1,
            })!;

            // Create system uniform bindgroup
            {
                // This buffer contains only the SystemUniform
                ulong size = (ulong)Marshal.SizeOf<SystemUniform>();

                systemBuffer =  device.CreateBuffer(new BufferDescriptor()
                {
                    Size = size,
                    Usage = BufferUsage.Uniform | BufferUsage.CopyDst,
                })!;

                systemBindGroupLayout = device.CreateBindGroupLayout(new BindGroupLayoutDescriptor()
                {
                    Entries =
                    [
                        new()
                        {
                            Binding = 0,
                            Visibility = ShaderStage.Vertex | ShaderStage.Fragment | ShaderStage.Compute,
                            Buffer = new()
                            {
                                Type = BufferBindingType.Uniform,
                            }
                        }
                    ]
                })!;
                
                systemBindGroup = device.CreateBindGroup(new BindGroupDescriptor()
                {
                    Layout = systemBindGroupLayout,
                    Entries = [
                        new()
                        {
                            Binding = 0,
                            Buffer = systemBuffer,
                            Size = size,
                        }
                    ]
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
                        ResolveTarget = new(null),
                        LoadOp = LoadOp.Clear,
                        StoreOp = StoreOp.Store,
                        ClearValue = clearColor,
                    }
                };
               
                RenderPassDescriptor renderPassDesc = new()
                {
                    ColorAttachments = ColorAttachments,
                    DepthStencilAttachment = null,
                };

                // Command Encoder
                var commandEncoder = device.CreateCommandEncoder(new() { })!;

                var RenderPassEncoder = commandEncoder.BeginRenderPass(renderPassDesc);

                RenderPassEncoder.End();

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

        public void OnRenderTargetSurfaceResized(RenderTarget rs, int width, int height)
        {
            rs.Width = width;
            rs.Height = height;
            rs.AspectRatio =   (float)width/ height;

            // dispose the Swapchain
            // TODO Dispose eventual depthbuffer
            if(rs.Swapchain != null)
               WebGPUMarshal.GetOwnedHandle(rs.Swapchain).Dispose();
            
            // Create new swapchain with the new size
            SwapChainDescriptor swapChainDescriptor = new()
            {
                Format = rs.Format,
                Width = (uint)width,
                Height = (uint)height,
                Usage = TextureUsage.RenderAttachment,
                PresentMode = PresentMode.Fifo,
            };
            rs.Swapchain = device.CreateSwapChain(rs.Surface, swapChainDescriptor)!;

            // TODO create depth buffer
        }

        public RenderTarget CreateRenderTarget(Surface surface, int width, int height, TextureFormat format)
        {
            var rt = new RenderTarget(surface, surface.GetPreferredFormat(adapter))
            {
                Format = format
            };
            OnRenderTargetSurfaceResized(rt, width, height);
            return rt;
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