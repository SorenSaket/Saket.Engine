using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Numerics;

namespace Saket.Engine
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Transform2D
    {
        public Vector2 Position;
        public float layer;
        public float rx;
        //public float ry;
        //public float rz;
        public Vector2 Scale;

        public Transform2D()
        {
            this.Position = Vector2.Zero;
            this.layer = 0;
            this.rx = 0;
            this.Scale = Vector2.One;
        }
        public Transform2D(float x, float y, float z = 0, float rx = 0, float sx = 1f, float sy = 1f)
        {
            this.Position = new Vector2(x, y);
            this.layer = z;
            this.rx = rx;
            this.Scale = new Vector2(sx, sy);
        }

        public Transform2D(Vector2 position, float layer, float rotaiton, Vector2 scale)
        {
            this.Position = position;
            this.layer = layer;
            this.rx = rotaiton;
            this.Scale = scale;
        }

        public override string? ToString()
        {
            return $"{Position.X}, {Position.Y}, {layer}, {rx}, {Scale.X}, {Scale.Y}";
        }
    }
}
