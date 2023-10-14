using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.WebGPU.Objects
{
    public class RenderPipeline
    {
        public nint Handle => handle;
        readonly nint handle;

        internal RenderPipeline(nint handle)
        {
            this.handle = handle;
        }

    }
}
