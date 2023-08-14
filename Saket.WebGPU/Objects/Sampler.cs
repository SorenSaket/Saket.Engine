using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.WebGPU.Objects
{
    public class Sampler
    {
        public nint Handle => handle;

        protected readonly nint handle;

        internal Sampler(nint handle)
        {
            this.handle = handle;
        }
    }
}
