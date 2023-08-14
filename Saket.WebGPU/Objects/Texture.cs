using Saket.WebGPU.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.WebGPU.Objects
{
    public class Texture
    {
        public nint Handle => handle;

        protected readonly nint handle;

        internal Texture(nint handle)
        {
            this.handle = handle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextureView CreateView(in WGPUTextureViewDescriptor descriptor)
        {
            return new TextureView(wgpu.TextureCreateView(handle, descriptor));
        }
    }
}
