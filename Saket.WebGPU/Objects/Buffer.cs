using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.WebGPU.Objects
{
    /// <summary>
    /// A GPUBuffer represents a block of memory that can be used in GPU operations. Data is stored in linear layout, meaning that each byte of the allocation can be addressed by its offset from the start of the GPUBuffer, subject to alignment restrictions depending on the operation. Some GPUBuffers can be mapped which makes the block of memory accessible via an ArrayBuffer called its mapping.
    /// </summary>
    public class Buffer
    {
        public nint Handle => handle;

        protected readonly nint handle;

        internal Buffer(nint handle)
        {
            this.handle = handle;
        }
    }
}
