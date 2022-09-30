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
        public Vector2 position;
        public float layer;

        public float rx;
        //public float ry;
        //public float rz;
        public float sx;
        public float sy;

        public Transform2D()
        {
            this.position = new Vector2();
            this.layer = 0;
            this.rx = 0;
            this.sx = 1;
            this.sy = 1;
        }
        public Transform2D(float x, float y, float z = 0, float rx = 0, float sx = 1, float sy = 1)
        {
            this.position = new Vector2(x, y);
            this.layer = z;
            this.rx = rx;
            this.sx = sx;
            this.sy = sy;
        }

        public override string? ToString()
        {
            return $"{position.X}, {position.Y}, {layer}, {rx}, {sx}, {sy}";
        }
        //public float sz;
    }
}
