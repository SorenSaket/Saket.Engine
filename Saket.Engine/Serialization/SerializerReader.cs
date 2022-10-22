using System;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Saket.Engine.Serialization
{
    public unsafe ref struct SerializerReader
    {
        /// <summary> The number of bytes avaliable to the reader </summary>
        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (maxAbsolutePosition > 0)
                    return maxAbsolutePosition - offset;
                else
                    return data.Length - offset;
            }
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

        public int RelativePosition
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => absolutePosition - offset;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => absolutePosition = value + offset;
        }
        /// <summary> The absolute position of reader in bytes based on underlying array</summary>
        public int AbsolutePosition
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => absolutePosition;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => absolutePosition = value;
        }


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

        readonly int maxAbsolutePosition;

        public SerializerReader(ArraySegment<byte> target)
        {
            this.data = target.Array;
            this.offset = this.absolutePosition = target.Offset;
            this.count = 0;
            this.maxAbsolutePosition = this.offset+target.Count;
        }

        public SerializerReader(ref byte[] target, int offset = 0)
        {
            this.data = target;
            this.offset = this.absolutePosition = offset;
            this.count = 0;
            this.maxAbsolutePosition = -1;
        }

        // ---- Primitive Serialization ---- 
        public T Read<T>() where T : unmanaged
        {
            unsafe
            {
                fixed (byte* p = data)
                {
                    int position = absolutePosition;
                    // This is nessary for enum support. One option is to remove direct enum support directly
                    // On the other hand supporting enums increases ease of use
                    if (typeof(T).IsEnum)
                    {
                        Type t = Enum.GetUnderlyingType(typeof(T));
                        Advance(Marshal.SizeOf(t));
                        return (T)Marshal.PtrToStructure(new IntPtr(p + position), t)!;
                    }
                    else
                    {
                        Advance(Marshal.SizeOf<T>());
                        return (T)Marshal.PtrToStructure(new IntPtr(p + position), typeof(T))!;
                    }
                }
            }
        }
        public T[] ReadArray<T>()
            where T : unmanaged
        {
            int length = Read<int>();
            if (length <= 0)
                return Array.Empty<T>();

            // Allocate new array
            // Todo make heap allocation free
            T[] result = new T[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = Read<T>();
            }
            return result;
        }

        // ---- Serializable Serialization ----
        public T ReadSerializable<T>() where T : ISerializable, new()
        {
            var obj = new T();
            obj.Deserialize(ref this);
            return obj;
        }
        public T[] ReadSerializableArray<T>() where T : ISerializable, new()
        {
            int length = Read<int>();

            if (length <= 0)
                return Array.Empty<T>();

            // Allocate new array
            // Todo make heap allocation free
            T[] result = new T[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = ReadSerializable<T>();
            }
            return result;
        }


        // ---- String Serialization ----
        public string ReadString()
        {
            int length = Read<int>();
            if (length == 0)
                return string.Empty;
            var segment = Read(length);
            return UTF8Encoding.UTF8.GetString(segment.Array, segment.Offset, segment.Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArraySegment<byte> Read(int length)
        {
            var p = absolutePosition;
            Advance(length);
            return new ArraySegment<byte>(data, p, length);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Advance(int length)
        {
            absolutePosition += length;
#if DEBUG
            if(maxAbsolutePosition > 0)
            {
                if (absolutePosition > maxAbsolutePosition)
                {
                    throw new IndexOutOfRangeException($"Read {absolutePosition - maxAbsolutePosition} bytes past underlying buffer");
                }
            }
            else
            {
                if (absolutePosition > data.Length)
                {
                    throw new IndexOutOfRangeException($"Read {absolutePosition - data.Length} bytes past underlying buffer");
                }
            }
#endif
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SizeOf<T>()
        {
            Type outputType = typeof(T).IsEnum ? Enum.GetUnderlyingType(typeof(T)) : typeof(T);
            return Marshal.SizeOf(outputType);
        }

    }
}
