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

        public readonly Vector2 Position    => new(X, Y);
        public readonly Vector2 Size        => new(Width, Height);

        public Tile(float width, float height, float x = 0, float y = 0)
        {
            Width = width;
            Height = height;
            X = x;
            Y = y;
        }
        // Why is this a method?
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float Area (){ return Width * Height; }
    }
}