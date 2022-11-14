using Newtonsoft.Json.Linq;
using Saket.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Filetypes.Font.OpenFontFormat
{
    public class OFFReader
    {
        public long Position { get; set; }
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
            Position = 0;
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
            Position += length;
#if DEBUG
            if (Position > buffer.Length)
            {
                throw new IndexOutOfRangeException($"Read {Position - buffer.Length} bytes past underlying buffer.");
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
                    value = p[Position];
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
                    value = (sbyte)p[Position];
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
                        p[Position] << 8 | 
                        p[Position + 1]);
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
                       p[Position] << 8 |
                       p[Position + 1]);
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
                        p[Position] << 16 |
                        p[Position + 1] << 8 |
                        p[Position + 2]
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
                    value = (int)(
                      p[Position] << 16 |
                      p[Position + 1] << 8 |
                      p[Position + 2]
                      );
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
                        p[Position] << 24 |
                        p[Position + 1] << 16 |
                        p[Position + 2] << 8 |
                        p[Position + 3]  
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
                    value = (int)(
                         p[Position] << 24 |
                         p[Position + 1] << 16 |
                         p[Position + 2] << 8 |
                         p[Position + 3]
                         );
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
                    value = (short)(p[Position] << 8 | p[Position + 1]);
                    Advance(2);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadUFWORD (ref ushort value)
        {
            unsafe
            {
                fixed (byte* p = buffer)
                {
                    value = (ushort)(p[Position] << 8 | p[Position + 1]);
                    Advance(2);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadF2DOT14(ref float value){
            unsafe
            {
                fixed (byte* p = buffer)
                {
                    value = (((short)(p[Position] << 8 | p[Position + 1])) / 16384f);
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
                    value = (long)(
                        p[Position ] << 56 |
                        p[Position + 1] << 48 |
                        p[Position + 2] << 40 |
                        p[Position + 3] << 32 |
                        p[Position + 4] << 24 |
                        p[Position + 5] << 16 |
                        p[Position + 6] << 8 |
                        p[Position + 7]);
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
                        p[Position ] << 24|
                        p[Position + 1]  << 16|
                        p[Position + 2]  << 8|
                        p[Position + 3] ));
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
                       p[Position] << 8 |
                       p[Position + 1]);
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
                          p[Position] << 24 |
                          p[Position + 1] << 16 |
                          p[Position + 2] << 8 |
                          p[Position + 3]
                          );
                    Advance(4);
                }
            }
        }

    }
}
