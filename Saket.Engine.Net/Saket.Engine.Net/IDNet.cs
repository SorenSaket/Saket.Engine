using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Net
{
    public struct IDNet
    {
        public UInt32 ID {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set; 
        }

        public IDNet(uint iD)
        {
            ID = iD;
        }

        public static implicit operator UInt32(IDNet d) => d.ID;
        public static implicit operator IDNet(UInt32 b) => new IDNet(b);
    }
}
