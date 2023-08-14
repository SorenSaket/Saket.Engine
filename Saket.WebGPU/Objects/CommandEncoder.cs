using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.WebGPU.Objects
{
    /// <summary>
    ///  Used to encode commands to be issued to the GPU.
    /// </summary>
    public class CommandEncoder
    {
        public nint Handle => handle;

        protected readonly nint handle;

        internal CommandEncoder(nint handle)
        {
            this.handle = handle;
        }
    }
}
