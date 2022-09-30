using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Saket.Engine.Serialization
{
    public unsafe ref struct SerializerWriter
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
        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => position;
        }

        byte[] data;
        int position;

        public SerializerWriter(byte[] target, int offset = 0)
        {
            this.data = target;
            this.position = offset;
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
        public void Serialize<T>(in T value, ForPrimitives unused = default) 
            where T : unmanaged
        {
            fixed (T* ptr = &value)
            {
                Write(ptr, sizeof(T));
            }
        }
        public void Serialize<T>(in T[] value, ForPrimitives unused = default) 
            where T : unmanaged
        {
            Serialize(value.Length);
            fixed (T* ptr = value)
            {
                Write(ptr, sizeof(T) * value.Length);
            }
        }

        // ---- Serializable Serialization ----
        public void Serialize<T>(ref T value, ForSerializable unused = default) where T : ISerializable, new() 
        {
            value.Serialize(this);
        }
        public void Serialize<T>(ref T[] value, ForSerializable unused = default) where T : ISerializable, new() 
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
        public void Write(void* value, int length)
        {
            EnsureCapacity(position + length);
            Marshal.Copy(new IntPtr(value), data, position, length);
            position += length;
        }

        #region Contraint Tags

        /// <summary>
        /// This empty struct exists to allow overloading WriteValue based on generic constraints.
        /// At the bytecode level, constraints aren't included in the method signature, so if multiple
        /// methods exist with the same signature, it causes a compile error because they would end up
        /// being emitted as the same method, even if the constraints are different.
        /// Adding an empty struct with a default value gives them different signatures in the bytecode,
        /// which then allows the compiler to do overload resolution based on the generic constraints
        /// without the user having to pass the struct in themselves.
        /// </summary>
        public struct ForPrimitives
        {

        }

        /// <summary>
        /// This empty struct exists to allow overloading WriteValue based on generic constraints.
        /// At the bytecode level, constraints aren't included in the method signature, so if multiple
        /// methods exist with the same signature, it causes a compile error because they would end up
        /// being emitted as the same method, even if the constraints are different.
        /// Adding an empty struct with a default value gives them different signatures in the bytecode,
        /// which then allows the compiler to do overload resolution based on the generic constraints
        /// without the user having to pass the struct in themselves.
        /// </summary>
        public struct ForSerializable
        {

        }

        #endregion
    }
}
