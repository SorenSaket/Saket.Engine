using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine
{
    [StructLayout(LayoutKind.Sequential)]
    public struct UInt24
    {
        private Byte _b0;
        private Byte _b1;
        private Byte _b2;

        public UInt24(UInt32 value)
        {
            _b0 = (byte)((value) & 0xFF);
            _b1 = (byte)((value >> 8) & 0xFF);
            _b2 = (byte)((value >> 16) & 0xFF);
        }
    }
}
