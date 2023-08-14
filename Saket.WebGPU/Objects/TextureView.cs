using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.WebGPU.Objects
{
    public class TextureView
    {
        public nint Handle => handle;
        public readonly nint handle;

        internal TextureView(nint handle)
        {
            this.handle = handle;
        }

    }
}