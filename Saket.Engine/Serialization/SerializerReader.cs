using System;
using System.Runtime.CompilerServices;

namespace Saket.Engine.Serialization
{
    public unsafe struct SerializerReader
    {
        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => data.Length;
        }
        public byte[] Data
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => data;
        }

        byte[] data;
        int position;

        public SerializerReader(byte[] target, int offset = 0)
        {
            this.data = target;
            this.position = offset;
        }

        // ---- Primitive Serialization ---- 
        public void Serialize<T>(in T value, SerializerWriter.ForPrimitives unused = default)
            where T : unmanaged
        {
            fixed (T* ptr = &value)
            {
                Write(ptr, sizeof(T));
            }
        }
        public void Serialize<T>(in T[] value, SerializerWriter.ForPrimitives unused = default)
            where T : unmanaged
        {
            Serialize(value.Length);
            fixed (T* ptr = value)
            {
                Write(ptr, sizeof(T) * value.Length);
            }
        }

        // ---- Serializable Serialization ----
        public void Serialize<T>(ref T value, SerializerWriter.ForSerializable unused = default) where T : ISerializable, new()
        {
            value.Serialize(this);
        }
        public void Serialize<T>(ref T[] value, SerializerWriter.ForSerializable unused = default) where T : ISerializable, new()
        {
            Serialize(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                value[i].Serialize(this);
            }
        }

        // ---- String Serialization ----
        public void Serialize(ref string s, bool oneByteChars = false)
        {
            Serialize((uint)s.Length);
            fixed (char* native = s)
            {
                Write((byte*)native, s.Length * sizeof(char));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Read(void* value, int length)
        {
            EnsureCapacity(position + length);
            Marshal.Copy(new IntPtr(value), data, position, length);
            position += length;
        }

    }
}
