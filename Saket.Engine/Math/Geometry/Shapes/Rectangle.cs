using System;
using System.Numerics;

namespace Saket.Engine.Math.Geometry.Shapes
{
    public struct Rectangle : IShape
    {
        public Vector2 Position;
        public Vector2 Size;
        public float Rotation;

        public float Area()
        {
            return Size.X * Size.Y;
        }

        public BoundingBox2D Bounds()
        {
            throw new NotImplementedException();
        }

        public SignedDistance GetSignedDistance(Vector2 point)
        {
            throw new NotImplementedException();
        }
    }
}
