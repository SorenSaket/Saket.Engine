using Saket.Engine.Math.Types;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Saket.Engine.Graphics
{
    /// <summary>
    /// Interger based AABB
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Tile
    {
        public float X, Y;
        public float Width, Height;


        public Vector2 Position => new Vector2(X, Y);
        public Vector2 Size => new Vector2(Width, Height);

        public Tile(float width, float height, float x = 0, float y = 0)
        {
            Width = width;
            Height = height;
            X = x;
            Y = y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Area (){ return Width * Height; }
    }
}