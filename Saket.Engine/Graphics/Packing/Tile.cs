using Saket.Engine.Math.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Graphics.Packing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Tile
    {
        public int X, Y;
        public int Width, Height;


        public Int2 Position => new Int2(X, Y);
        public Int2 Size => new Int2(Width, Height);

        public Tile(int width, int height, int x = 0, int y = 0)
        {
            Width = width;
            Height = height;
            X = x;
            Y = y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Area (){ return Width * Height; }
    }
}