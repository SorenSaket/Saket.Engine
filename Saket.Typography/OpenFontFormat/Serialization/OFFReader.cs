using System.Runtime.CompilerServices;

namespace Saket.Typography.OpenFontFormat.Serialization
{
    public class OFFReader
    {
        public long BufferPosition { get; set; }
        public Stream stream;
        public byte[] buffer = new byte[128];
        public bool IsReader => true;
        public OFFReader(Stream input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            stream = input;
        }


        /// <summary>
        /// Ensures that there is more than the number of bytes left in stream.
        /// Do this once before any read to ensure that you don't cross end of stream.
        /// </summary>
        public virtual bool LoadBytes(int numberOfBytes)
        {
            if (stream == null)
                return false;
            if (numberOfBytes <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfBytes));


            int newCapacity = buffer.Length;
            // Double the capacity until theres enough
            while (numberOfBytes >= newCapacity)
            {
                newCapacity *= 2;
            }

            if (newCapacity > buffer.Length)
            {
                // Array.Resize is not used since the values should not be copied
                // values can stay uninitialized since they will be overwritten by stream.Read()
                // old array will be collected by GC
                buffer = GC.AllocateUninitializedArray<byte>(newCapacity);
            }

            // Reset poition
            BufferPosition = 0;
            int bytesRead = 0;
            int n = 0;
            do
            {
                n = stream.Read(buffer, bytesRead, numberOfBytes - bytesRead);
                if (n == 0)
                {
                    return false;
                }
                bytesRead += n;
            } while (bytesRead < numberOfBytes);

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Advance(int length)
        {
            BufferPosition += length;
#if DEBUG
            if (BufferPosition > buffer.Length)
            {
                throw new IndexOutOfRangeException($"Read {BufferPosition - buffer.Length} bytes past underlying buffer.");
            }
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadUInt8(ref byte value)
        {
            unsafe
            {
                fixed (byte* p = buffer)
                {
                    value = p[BufferPosition];
                    Advance(1);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadInt8(ref sbyte value)
        {
            unsafe
            {
                fixed (byte* p = buffer)
                {
                    value = (sbyte)p[BufferPosition];
                    Advance(1);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadUInt16(ref ushort value)
        {
            unsafe
            {
                fixed (byte* p = buffer)
                {
                    value = (ushort)(
                        p[BufferPosition] << 8 |
                        p[BufferPosition + 1]);
                    Advance(2);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadInt16(ref short value)
        {
            unsafe
            {
                fixed (byte* p = buffer)
                {
                    value = (short)(
                       p[BufferPosition] << 8 |
                       p[BufferPosition + 1]);
                    Advance(2);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadUInt24(ref uint value)
        {
            unsafe
            {
                fixed (byte* p = buffer)
                {
                    value = (uint)(
                        p[BufferPosition] << 16 |
                        p[BufferPosition + 1] << 8 |
                        p[BufferPosition + 2]
                        );
                    Advance(3);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadInt24(ref int value)
        {
            unsafe
            {
                fixed (byte* p = buffer)
                {
                    value =
                      p[BufferPosition] << 16 |
                      p[BufferPosition + 1] << 8 |
                      p[BufferPosition + 2]
                      ;
                    Advance(3);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadUInt32(ref uint value)
        {
            unsafe
            {
                fixed (byte* p = buffer)
                {
                    value = (uint)(
                        p[BufferPosition] << 24 |
                        p[BufferPosition + 1] << 16 |
                        p[BufferPosition + 2] << 8 |
                        p[BufferPosition + 3]
                        );
                    Advance(4);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadInt32(ref int value)
        {
            unsafe
            {
                fixed (byte* p = buffer)
                {
                    value =
                         p[BufferPosition] << 24 |
                         p[BufferPosition + 1] << 16 |
                         p[BufferPosition + 2] << 8 |
                         p[BufferPosition + 3]
                         ;
                    Advance(4);
                }
            }

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadFWORD(ref short value)
        {
            unsafe
            {
                fixed (byte* p = buffer)
                {
                    value = (short)(p[BufferPosition] << 8 | p[BufferPosition + 1]);
                    Advance(2);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadUFWORD(ref ushort value)
        {
            unsafe
            {
                fixed (byte* p = buffer)
                {
                    value = (ushort)(p[BufferPosition] << 8 | p[BufferPosition + 1]);
                    Advance(2);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadF2DOT14(ref float value)
        {
            unsafe
            {
                fixed (byte* p = buffer)
                {
                    value = (short)(p[BufferPosition] << 8 | p[BufferPosition + 1]) / 16384f;
                    Advance(2);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadLONGDATETIME(ref long value)
        {
            unsafe
            {
                fixed (byte* p = buffer)
                {
                    value =
                        p[BufferPosition] << 56 |
                        p[BufferPosition + 1] << 48 |
                        p[BufferPosition + 2] << 40 |
                        p[BufferPosition + 3] << 32 |
                        p[BufferPosition + 4] << 24 |
                        p[BufferPosition + 5] << 16 |
                        p[BufferPosition + 6] << 8 |
                        p[BufferPosition + 7];
                    Advance(8);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadTag(ref Tag value)
        {
            unsafe
            {
                fixed (byte* p = buffer)
                {
                    value = new Tag((uint)(
                        p[BufferPosition] << 24 |
                        p[BufferPosition + 1] << 16 |
                        p[BufferPosition + 2] << 8 |
                        p[BufferPosition + 3]));
                    Advance(4);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadOffset16(ref ushort value)
        {
            unsafe
            {
                fixed (byte* p = buffer)
                {
                    value = (ushort)(
                       p[BufferPosition] << 8 |
                       p[BufferPosition + 1]);
                    Advance(2);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadOffset32(ref uint value)
        {
            unsafe
            {
                fixed (byte* p = buffer)
                {
                    value = (uint)(
                          p[BufferPosition] << 24 |
                          p[BufferPosition + 1] << 16 |
                          p[BufferPosition + 2] << 8 |
                          p[BufferPosition + 3]
                          );
                    Advance(4);
                }
            }
        }

    }
}
