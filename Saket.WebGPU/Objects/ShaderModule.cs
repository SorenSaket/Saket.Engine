using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.WebGPU.Objects
{
    public class ShaderModule
    {
        public nint Handle => handle;

        protected readonly nint handle;

        internal ShaderModule(nint handle)
        {
            this.handle = handle;
        }
    }
}
