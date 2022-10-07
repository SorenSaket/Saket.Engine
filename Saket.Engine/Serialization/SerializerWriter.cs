using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Saket.Engine.Serialization
{
    /// <summary>
    /// Writer struct able to serialize primities and ISerializables to byte array
    /// </summary>
    public unsafe ref struct SerializerWriter
    {
        /// <summary> The number of bytes avaliable to the writer </summary>
        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => data.Length;
        }
        /// <summary> The absolute position of writer in bytes based on underlying array</summary>
        public int AbsolutePosition {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get =>absolutePosition;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => absolutePosition = value; 
        }

        public int RelativePosition
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => absolutePosition-offset;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => absolutePosition = value+offset;
        }

        /// <summary> How many bytes the writer has written. </summary>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => count;
        }
        /// <summary> The data that has been written to the underlying array </summary>
        public ArraySegment<byte> Data
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new ArraySegment<byte>(data, offset, count);
        }

        public byte[] DataRaw 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => data;
        }



        /// <summary> 
        /// Underlying array 
        /// </summary>
        byte[] data;
        
        /// <summary> 
        /// The current writer position relative to underlying array 
        /// </summary>
        int absolutePosition;

        /// <summary>
        /// The length of bytes written
        /// In effect this is the furthest (absolutePosition-offset) written to
        /// </summary>
        int count;

        /// <summary>
        /// The starting point for the writer. 
        /// The writer should not be able to write to bytes before this index
        /// </summary>
        readonly int offset;

        readonly int capacity;

        /// <summary>
        /// Create Writer from Array Segment. The writer can 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="offset"></param>
        public SerializerWriter(ArraySegment<byte> target, int offset = 0)
        {
            this.data = target.Array!;
            this.offset = this.absolutePosition = offset + target.Offset;
            this.count = 0;
            this.capacity = target.Count;
        }

        public SerializerWriter(byte[] target, int offset = 0)
        {
            this.data = target;
            this.offset = this.absolutePosition = offset;
            this.count = 0;
            this.capacity = target.Length;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureCapacity(int requiredCapacity)
        {
            // Double in size every time
            int newCapacity = Capacity;
            // Double the capacity until theres enough
            while (requiredCapacity >= newCapacity)
            {
                newCapacity *= 2;
            }
            Array.Resize(ref data, newCapacity);
        }

        // ---- Primitive Serialization ----
        public void Write<T>(in T value) 
            where T : unmanaged
        {
            fixed (T* ptr = &value)
            {
                Write(ptr, sizeof(T));
            }
        }
        public void Write<T>(in T[] value) 
            where T : unmanaged
        {
            Write(value.Length);
            fixed (T* ptr = value)
            {
                Write(ptr, sizeof(T) * value.Length);
            }
        }
        public void Write<T>(in ArraySegment<T> value)
            where T : unmanaged
        {
            Write(value.Count);
            fixed (T* ptr = value.Array)
            {
                Write(ptr + value.Offset * sizeof(T), sizeof(T) * value.Count);
            }
        }

        // ---- Serializable Serialization ----
        public void WriteSerializable<T>(in T value) where T : ISerializable, new() 
        {
            value.Serialize(this);
        }
        public void WriteSerializable<T>(in T[] value) where T : ISerializable, new() 
        {
            Write(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                value[i].Serialize(this);
            }
        }

        // ---- String Serialization ----
        public void Write(string s, bool oneByteChars = false) 
        {
            Write(s.Length);
            fixed (char* native = s)
            {
                Write(native, s.Length * sizeof(char));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(void* value, int length)
        {
            EnsureCapacity(absolutePosition + length);
            Marshal.Copy(new IntPtr(value), data, absolutePosition, length);
            absolutePosition += length;
            this.count = Math.Max(this.count, absolutePosition-offset);
        }

    }
}