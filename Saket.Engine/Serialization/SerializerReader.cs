using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Saket.Engine.Serialization
{
    public unsafe ref struct SerializerReader
    {
        /// <summary> The number of bytes avaliable to the reader </summary>
        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => capacity;
        }

        /// <summary> The data that has been read by the reader </summary>
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

        /// <summary> The absolute position of reader in bytes based on underlying array</summary>
        public int AbsolutePosition { get => absolutePosition; set => absolutePosition = value; }

        /// <summary> 
        /// Underlying array 
        /// </summary>
        byte[] data;
        /// <summary> 
        /// The current reader position relative to underlying array 
        /// </summary>
        int absolutePosition;
        /// <summary>
        /// The length of bytes read
        /// In effect this is the furthest (absolutePosition-offset) read from
        /// </summary>
        int count;
        /// <summary>
        /// The starting point for the reader. 
        /// The read should not be able to read bytes before this index
        /// </summary>
        readonly int offset;

        readonly int capacity;

        public SerializerReader(ArraySegment<byte> target, int offset = 0)
        {
            this.data = target.Array;
            this.offset = this.absolutePosition = offset + target.Offset;
            this.count = 0;
            this.capacity = target.Count;
        }

        public SerializerReader(byte[] target, int offset = 0)
        {
            this.data = target;
            this.offset = this.absolutePosition = offset;
            this.count = 0;
            this.capacity = target.Length;
        }

        // ---- Primitive Serialization ---- 
        public T Read<T>(SerializerWriter.ForPrimitives unused = default) where T : unmanaged
        {
            unsafe
            {
                fixed (byte* p = data)
                {
                    int position = absolutePosition;
                    absolutePosition += Marshal.SizeOf<T>();
                    return (T)Marshal.PtrToStructure(new IntPtr(p + position), typeof(T))!;
                }
            }
        }

        public T[] ReadArray<T>(SerializerWriter.ForPrimitives unused = default)
            where T : unmanaged
        {
            int length = Read<int>();
            // Allocate new array
            T[] result = new T[length];
            int byteCount = length * Marshal.SizeOf<T>();
            Buffer.BlockCopy(data, absolutePosition, result, 0, byteCount);
            absolutePosition += byteCount;
            return result;
        }

        // ---- Serializable Serialization ----
        public T Read<T>(SerializerWriter.ForSerializable unused = default) where T : ISerializable, new()
        {
            var obj = new T();
            obj.Deserialize(this);
            return obj;
        }
        public T[] ReadArray<T>(SerializerWriter.ForSerializable unused = default) where T : ISerializable, new()
        {
            int length = Read<int>();
            // Allocate new array
            T[] result = new T[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = Read<T>();
            }
            return result;
        }


        // ---- String Serialization ----
        public string Read(ref string s, bool oneByteChars = false)
        {
            throw new NotImplementedException();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArraySegment<byte> Read(int length)
        {
            var p = absolutePosition;
            absolutePosition += length;
            return new ArraySegment<byte>(data, p, length);
        }

    }
}
