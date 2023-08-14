using Saket.WebGPU.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.WebGPU.Objects
{
    /// <summary>
    /// The GPUAdapter interface of the WebGPU API represents a GPU adapter. From this you can request a GPUDevice, adapter info, features, and limits.
    /// </summary>
    public class Adapter
    {
        public nint Handle => handle;
        public readonly nint handle;

        internal Adapter(nint handle)
        {
            this.handle = handle;
        }

        ~Adapter() {
            wgpu.AdapterRelease(handle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <inheritdoc cref="Saket.WebGPU.Native.wgpu.AdapterGetProperties"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public WGPUAdapterProperties GetProperties()
        {
            throw new NotImplementedException();
            ///wgpu.AdapterGetProperties(adapter, ref properties);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetLimits(ref WGPULimits limits)
        {
            WGPUSupportedLimits a = new ()
            {
                limits = limits
            };
            bool value = wgpu.AdapterGetLimits(handle, ref a);
            limits = a.limits;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Device CreateDevice(ReadOnlySpan<WGPUFeatureName> requiredFeatures, WGPURequiredLimits? limits = null, wgpulabel label = default)
        {
            unsafe {
                var l = limits.GetValueOrDefault();

                fixed (byte* ptr_label = label.bytes)
                fixed (WGPUFeatureName* ptr_requiredFeatures = requiredFeatures)

                {
                    WGPUDeviceDescriptor descriptor = new()
                    {
                        requiredFeaturesCount = (uint)requiredFeatures.Length,
                        requiredFeatures = ptr_requiredFeatures,
                        requiredLimits = limits.HasValue ? &l : (WGPURequiredLimits*)0,
                        label = (char*)ptr_label
                    };
                    return new Device(Helper.RequestDevice(handle, descriptor));
                }
            
            }
       
        }
    }
}