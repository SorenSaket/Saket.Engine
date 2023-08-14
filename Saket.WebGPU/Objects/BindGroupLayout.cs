using Saket.WebGPU.Native;

namespace Saket.WebGPU.Objects
{
    /// <summary>
    /// A GPUBindGroupLayout defines the interface between a set of resources bound in a GPUBindGroup and their accessibility in shader stages.
    /// </summary>
    public class BindGroupLayout
    {
        public nint Handle => handle;

        protected readonly nint handle;

        internal BindGroupLayout(nint handle)
        {
            this.handle = handle;
        }
    }
}
