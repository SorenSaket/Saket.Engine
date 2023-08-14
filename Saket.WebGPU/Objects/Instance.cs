using System.Runtime.CompilerServices;
using Saket.WebGPU.Native;

namespace Saket.WebGPU.Objects
{
    public class Instance
    {
        public nint Handle => handle;

        private readonly nint handle;

        public Instance()
        {
            handle = Native.wgpu.CreateInstance(new());
            if (handle == 0)
                throw new Exception("Could not initialize WebGPU");
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Surface CreateSurfaceFromWindowsHWND(IntPtr hinstance, IntPtr hwnd, string? label = default)
        {
            var a = new WGPUSurfaceDescriptorFromWindowsHWND()
            {
                chain = new WGPUChainedStruct()
                {
                    sType = WGPUSType.SurfaceDescriptorFromWindowsHWND,
                },
                hinstance = hinstance,
                hwnd = hwnd
            };
            fixed(char* ptr_label = label)
            {
                return new Surface(Native.wgpu.InstanceCreateSurface(handle, new WGPUSurfaceDescriptor()
                {
                    label = ptr_label,
                    nextInChain = &a.chain
                }));
            }
           
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ProcessEvents()
        {
            Native.wgpu.InstanceProcessEvents(handle);
        }

        /// <inheritdoc cref="Saket.WebGPU.Native.wgpu.InstanceRequestAdapter"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Adapter RequestAdapter(in WGPURequestAdapterOptions options)
        {
            nint a = Helper.RequestAdapter(handle, options);
            if (a == 0)
                throw new Exception("Could Not create Adapter");
            return new Adapter(a);
        }
    }
}
