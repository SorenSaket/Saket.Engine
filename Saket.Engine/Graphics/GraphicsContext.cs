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
using System.Diagnostics;

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
                    BackendType = BackendType.D3D12,
                    CompatibleSurface = null,
                }).GetAwaiter().GetResult() ?? throw new Exception("Failed to acquire Adapter");

                SupportedLimits supportedLimits = adapter.GetLimits()!.Value;

                // Create Device
                DeviceDescriptor deviceDescriptor = new DeviceDescriptor()
                {
                    RequiredLimits = new WGPUNullableRef<RequiredLimits>(
                        new RequiredLimits(supportedLimits.Limits)
                        ),
                    DefaultQueue = new QueueDescriptor(),
                    UncapturedErrorCallback = (ErrorType type, ReadOnlySpan<byte> message) =>
                    {
                        Console.Error.WriteLine($"{Enum.GetName(type)} : {Encoding.UTF8.GetString(message)}");
                    },
                    DeviceLostCallback = (DeviceLostReason lostReason, ReadOnlySpan<byte> message) =>
                    {
                        Console.Error.WriteLine($"Device lost! Reason: {Enum.GetName(lostReason)} : {Encoding.UTF8.GetString(message)}");
                        Debugger.Break();
                    }

                };
                device = adapter.RequestDeviceAsync(deviceDescriptor).GetAwaiter().GetResult() ?? throw new Exception("Failed to acquire Device");

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

                systemBuffer = device.CreateBuffer(new BufferDescriptor()
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

            RenderPassDescriptor renderPassDesc = new()
            {
                ColorAttachments = [
                    new()
                    {
                        View = target,
                        ResolveTarget = default,
                        LoadOp = LoadOp.Clear,
                        StoreOp = StoreOp.Store,
                        ClearValue = clearColor,
                    }
                ],
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


        public void SetSystemUniform(SystemUniform uniform)
        {
            queue.WriteBuffer(systemBuffer, 0, stackalloc SystemUniform[] { uniform });
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