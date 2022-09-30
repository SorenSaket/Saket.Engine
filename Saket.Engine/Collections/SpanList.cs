using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Collections
{
    public ref struct SpanRingBuffer<T>where T : unmanaged
    {
        public int Capacity => buffer.Length;
        public int Count => count;

        public T this[int i]
        {
            get => buffer[(start + i) % buffer.Length];
            set => buffer[(start + i) % buffer.Length] = value;
        }

        private int count;
        private int start;
        private int end;

        Span<T> buffer;

        public SpanRingBuffer(Span<T>  buffer)
        {
            this.buffer = buffer;
            count = start = end = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Enqueue(T value)
        {
            if (count == Capacity)
                throw new ArgumentOutOfRangeException("Ring buffer is full smh");
            count++;
            end = (end + 1) % buffer.Length;
            buffer[end] = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public T Dequeue()
        {
            if (count == 0)
                throw new ArgumentOutOfRangeException("Ring buffer is empty smh");
            count--;
            var el = start;
            start = (start + 1) % buffer.Length;
            return buffer[el];
        }
    }
}
