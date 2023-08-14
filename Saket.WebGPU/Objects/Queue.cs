using Saket.WebGPU.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.WebGPU.Objects
{
    public class Queue
    {
        public nint Handle => handle;
        private readonly nint handle;

        internal Queue(nint handle)
        {
            this.handle = handle;
        }
        /// <inheritdoc cref="Native.wgpu.QueueSubmit"> </inheritdoc>
        public void Sumbit(ReadOnlySpan<CommandBuffer> commandBuffers)
        {
            unsafe
            {
                var ptr_commandBuffers = stackalloc nint[commandBuffers.Length];
                for (int i = 0; i < commandBuffers.Length; i++)
                {
                    ptr_commandBuffers[i] = commandBuffers[i].Handle;
                }
                wgpu.QueueSubmit(handle, (uint)commandBuffers.Length, (nint)ptr_commandBuffers);
            }
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void WriteBuffer(Buffer buffer, ulong bufferOffset, void* data, nuint length)
        {
           wgpu.QueueWriteBuffer(handle, buffer.Handle, bufferOffset,data, length);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBuffer(Buffer buffer, ulong bufferOffset, ReadOnlySpan<byte> data)
        {
            unsafe
            {
                fixed(void* ptr = data)
                {
                    wgpu.QueueWriteBuffer(handle, buffer.Handle, bufferOffset, ptr, (nuint)data.Length);
                }
            }
        }
        //public void WriteTexture()
    }
}