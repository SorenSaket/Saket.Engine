using Saket.WebGPU.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.WebGPU.Objects
{
    /// <summary>
    /// The GPUBindGroup interface of the WebGPU API is based on a GPUBindGroupLayout and defines a set of resources to be bound together in a group and how those resources are used in shader stages.
    /// </summary>
    public class BindGroup
    {
        public nint Handle => handle;

        protected readonly nint handle;

        internal BindGroup(nint handle)
        {
            this.handle = handle;
        }   
    }
}