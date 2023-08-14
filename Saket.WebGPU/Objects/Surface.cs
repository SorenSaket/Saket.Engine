using Saket.WebGPU.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.WebGPU.Objects
{
    public class Surface
    {
        public nint Handle => handle;
        public readonly nint handle;

        internal Surface(nint handle)
        {
            this.handle = handle;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public WGPUTextureFormat GetPreferredFormat(Adapter adapter)
        {
            return wgpu.SurfaceGetPreferredFormat(handle, adapter.Handle);
        }
    }
}