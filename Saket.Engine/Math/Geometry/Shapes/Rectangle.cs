using System;
using System.Numerics;

namespace Saket.Engine.Math.Geometry.Shapes
{
    public struct Rectangle : ISDF2D
    {
        public Vector2 Position;
        public Vector2 Size;
        public float Rotation;


        public float Area()
        {
            return Size.X * Size.Y;
        }

        public float SignedDistance(Vector2 point)
        {
            throw new NotImplementedException();
            /*
            float l = length(b - a);

            Vector2 d = (b - a) / l;
            Vector2 q = (point - (a + b) * 0.5);
            q = mat2(d.x, -d.y, d.y, d.x) * q;
            q = abs(q) - vec2(l, th) * 0.5;
            return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0);*/
        }
    }
}
