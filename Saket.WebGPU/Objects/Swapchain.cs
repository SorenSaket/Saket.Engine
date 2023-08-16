using Saket.WebGPU.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.WebGPU.Objects
{
    public class Swapchain
    {
        public nint Handle => handle;
        private readonly nint handle;

        internal Swapchain(nint handle)
        {
            this.handle = handle;
        }

        public TextureView GetCurrentTextureView()
        {
           return new TextureView(wgpu.SwapChainGetCurrentTextureView(handle));
        }

        public void Present()
        {
            wgpu.SwapChainPresent(handle);
        }
    }
}
