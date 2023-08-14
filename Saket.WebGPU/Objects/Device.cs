using Saket.WebGPU.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.WebGPU.Objects
{
    /// <summary>
    /// A GPUDevice encapsulates a device and exposes the functionality of that device. GPUDevice is the top-level interface through which WebGPU interfaces are created. 
    /// <seealso href="https://gpuweb.github.io/gpuweb/#device">Documentation</seealso>
    /// </summary>
    /// 
    public class Device
    {
        public nint Handle => handle;
        private readonly nint handle;

        internal Device(nint handle)
        {
            this.handle = handle;
            // Device Error callback
            unsafe
            {
                wgpu.DeviceSetUncapturedErrorCallback(handle, (WGPUErrorType type,
                  char* message,
                  void* userdata) => {

                      Debug.Write("WGPUDevice Error: " + new string((sbyte*)message,0,0, Encoding.UTF8));

                  }, null);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BindGroupLayout CreateBindGroupLayout(ReadOnlySpan<WGPUBindGroupLayoutEntry> entires, wgpulabel label = default)
        {
            unsafe
            {
                fixed (byte* ptr_label = label.bytes)
                fixed (WGPUBindGroupLayoutEntry* ptr_entires = entires)
                {
                    var descriptor = new WGPUBindGroupLayoutDescriptor()
                    {
                        entryCount = (uint)entires.Length,
                        entries = ptr_entires,
                        label = (char*)ptr_label
                    };

                    return new BindGroupLayout(wgpu.DeviceCreateBindGroupLayout(handle, descriptor));
                }
            }
         
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BindGroup CreateBindGroup(BindGroupLayout layout, Span<WGPUBindGroupEntry> entires, wgpulabel label = default)
        {
            unsafe
            {
                fixed (byte* ptr_label = label.bytes)
                fixed (WGPUBindGroupEntry* ptr_entires = entires)
                {
                    WGPUBindGroupDescriptor descriptor = new()
                    {
                        layout = layout.Handle,
                        label = (char*)ptr_label,
                        entries = ptr_entires,
                        entryCount = (uint)entires.Length,
                    };
                    return new BindGroup(wgpu.DeviceCreateBindGroup(handle, descriptor));
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Buffer CreateBuffer(in WGPUBufferDescriptor descriptor)
        {
            return new Buffer(wgpu.DeviceCreateBuffer(handle, descriptor));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CommandEncoder CreateCommandEncoder(in WGPUCommandEncoderDescriptor descriptor)
        {
            return new CommandEncoder(wgpu.DeviceCreateCommandEncoder(handle, descriptor));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ComputePipeline CreateComputePipeline(in WGPUComputePipelineDescriptor descriptor)
        {
            return new ComputePipeline(wgpu.DeviceCreateComputePipeline(handle, descriptor));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PipelineLayout CreatePipelineLayout(in WGPUPipelineLayoutDescriptor descriptor)
        {
            return new PipelineLayout(wgpu.DeviceCreatePipelineLayout(handle, descriptor));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public QuerySet CreateQuerySet(in WGPUQuerySetDescriptor descriptor)
        {
            return new QuerySet(wgpu.DeviceCreateQuerySet(handle, descriptor));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RenderBundleEncoder CreateRenderBundleEncoder(in WGPURenderBundleEncoderDescriptor descriptor)
        {
            return new RenderBundleEncoder(wgpu.DeviceCreateRenderBundleEncoder(handle, descriptor));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RenderPipeline CreateRenderPipeline(in WGPURenderPipelineDescriptor descriptor)
        {
            return new RenderPipeline(wgpu.DeviceCreateRenderPipeline(handle, descriptor));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Sampler CreateSampler(in WGPUSamplerDescriptor descriptor)
        {
            return new Sampler(wgpu.DeviceCreateSampler(handle, descriptor));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ShaderModule CreateShaderModule(in WGPUShaderModuleDescriptor descriptor)
        {
            return new ShaderModule(wgpu.DeviceCreateShaderModule(handle, descriptor));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Swapchain CreateSwapchain(Surface surface, in WGPUSwapChainDescriptor descriptor)
        {
            return new Swapchain(wgpu.DeviceCreateSwapChain(handle, surface.Handle, descriptor));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Texture CreateTexture(in WGPUTextureDescriptor descriptor)
        {
            return new Texture(wgpu.DeviceCreateTexture(handle, descriptor));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Queue GetQueue()
        {
            return new Queue(wgpu.DeviceGetQueue(handle));
        }
    }
}
