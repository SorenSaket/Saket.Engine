using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Saket.Engine.Serialization
{
    /// <summary>
    /// Writer struct able to serialize primities and ISerializables to byte array
    /// </summary>
    public unsafe class SerializerWriter
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
            get => new ArraySegment<byte>(data, 0, count);
        }
        /// <summary>  </summary>
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


        public SerializerWriter(int intialCapacity = 64)
        {
            this.data = new byte[intialCapacity];
            this.count = 0;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureCapacity(int requiredCapacity)
        {
            // Double in size every time
            int newCapacity = data.Length;
            // Double the capacity until theres enough
            while (requiredCapacity > newCapacity)
            {
                newCapacity *= 2;
            }
            if(newCapacity != data.Length)
                Array.Resize(ref data, newCapacity);
        }

        // ---- Primitive Serialization ----
        public void Write<T>(in T value) 
            where T : unmanaged
        {
            fixed (T* ptr = &value)
            {
                Write(ptr, SizeOf<T>());
            }
        }
        public void Write<T>(in T[] value) 
            where T : unmanaged
        {
            if (value == null || value.Length <= 0)
            {
                Write(0);
                return;
            }
               
            Write(value.Length);
            fixed (T* ptr = value)
            {
                Write(ptr, SizeOf<T>() * value.Length);
            }
        }
        public void Write<T>(in ArraySegment<T> value)
            where T : unmanaged
        {
            Write(value.Count);
            fixed (T* ptr = value.Array)
            {
                Write(ptr + value.Offset * SizeOf<T>(), SizeOf<T>() * value.Count);
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
            // 
            this.count = Math.Max(this.count, absolutePosition);
        }

        public void Reset()
        {
            absolutePosition = 0;
            count = 0;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SizeOf<T>()
        {
            Type outputType = typeof(T).IsEnum ? Enum.GetUnderlyingType(typeof(T)) : typeof(T);
            return Marshal.SizeOf(outputType);
        }
    }
}