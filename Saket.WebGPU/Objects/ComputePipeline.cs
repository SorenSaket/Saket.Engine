using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.WebGPU.Objects
{
    public class ComputePipeline
    {
        public nint Handle => handle;

        protected readonly nint handle;

        internal ComputePipeline(nint handle)
        {
            this.handle = handle;
        }
    }
}
