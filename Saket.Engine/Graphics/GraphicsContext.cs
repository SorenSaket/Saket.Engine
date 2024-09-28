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
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Emit;
using System.Threading;

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
                instance = WebGPU.CreateInstance() ?? throw new Exception("Failed to create webgpu instance");

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
                        new RequiredLimits() { Limits = supportedLimits.Limits }
                        ),
                    DefaultQueue = new QueueDescriptor(),
                    UncapturedErrorCallback = (ErrorType type, ReadOnlySpan<byte> message) =>
                    {
                        string str = $"{Enum.GetName(type)} : {Encoding.UTF8.GetString(message)}";

                        Console.Error.WriteLine(str);
                        Debug.WriteLine(str);
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
            queue = device.GetQueue() ?? throw new Exception("Failed to create Queue");

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
                ulong size = Extensions_Math.RoundUpToNextMultiple<ulong>((ulong) Marshal.SizeOf<SystemUniform>(),16);

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
        }

        public void SetSystemUniform(SystemUniform uniform)
        {
            queue.WriteBuffer(systemBuffer, 0, uniform);
        }

        public static TextureGroup CreateDeapthTexture(Device device, uint width, uint height, TextureFormat format, string label)
        {
            TextureDescriptor texturedescriptor = new()
            {
                Label = label,
                Dimension = TextureDimension.D2,
                Size = new Extent3D(width, height, 1),
                SampleCount = 1,
                Format = format,
                MipLevelCount = 1,
                Usage = TextureUsage.RenderAttachment | TextureUsage.TextureBinding,
            };
            Texture gputex = device.CreateTexture(texturedescriptor)!;

            TextureViewDescriptor textureViewDescriptor = new()
            {
                Label = label + " TextureView",
                Format = format,
                Dimension = TextureViewDimension.D2,
                BaseMipLevel = 0,
                MipLevelCount = 1,
                BaseArrayLayer = 0,
                ArrayLayerCount = 1,
                Aspect = TextureAspect.All,
            };
            TextureView gputexview = gputex.CreateView(textureViewDescriptor)!;

            SamplerDescriptor descriptor_sampler = new()
            {
                AddressModeU = AddressMode.ClampToEdge,
                AddressModeV = AddressMode.ClampToEdge,
                AddressModeW = AddressMode.ClampToEdge,
                MagFilter = FilterMode.Linear,
                MinFilter = FilterMode.Linear,
                MipmapFilter = MipmapFilterMode.Nearest,
                Compare = CompareFunction.LessEqual,
                LodMinClamp = 0,
                LodMaxClamp = 100,
            };
            Sampler sampler = device.CreateSampler(descriptor_sampler)!;

            return new TextureGroup(gputex, gputexview, sampler);
        }

        public TextureGroup UploadToGpuFromData(Image image, Sampler? sampler = null)
        {
            if (!image.IsUploadedToGPU)
            {
                image.GPUCreateTexture(this);
                image.GPUWriteTexture(this);
            }
            TextureViewDescriptor textureViewDescriptor = new()
            {
                Label = "TextureView_" + image.name,
                Format = applicationpreferredFormat,
                Dimension = TextureViewDimension.D2,
                BaseMipLevel = 0,
                MipLevelCount = 1,
                BaseArrayLayer = 0,
                ArrayLayerCount = 1,
                Aspect = TextureAspect.All,
            };
            TextureView gputexview = image.texture!.CreateView(textureViewDescriptor)!;

            return new TextureGroup(image.texture!, gputexview, sampler ?? defaultSampler);
        }
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct SystemUniform
    {
        public Matrix4x4 viewProjectionMatrix;
        public Matrix4x4 projectionMatrix;
        public Matrix4x4 viewMatrix;

        public Matrix4x4 inverseViewProjectionMatrix;
        public Matrix4x4 inverseProjectionMatrix;
        public Matrix4x4 inverseViewMatrix;

        public float Time;
        public float DeltaTime;
        public UInt32 frame;
    }
}