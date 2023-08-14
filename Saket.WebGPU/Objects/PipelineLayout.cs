using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.WebGPU.Objects
{
    public class PipelineLayout
    {
        public nint Handle => handle;

        protected readonly nint handle;

        internal PipelineLayout(nint handle)
        {
            this.handle = handle;
        }
    }
}
