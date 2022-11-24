using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Graphics.MSDF
{
    public struct MultiDistance
    {
        public float R, G, B;
        public float Med;

        public float Dist
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Med;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Med = value;
        }
    }
}
