using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
namespace Saket.Engine
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Transform2D
    {
        public float x;
        public float y;
        public float z;

        public float rx;
        //public float ry;
        //public float rz;

        public float sx;
        public float sy;

        public Transform2D()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
            this.rx = 0;
            this.sx = 1;
            this.sy = 1;
        }
        public Transform2D(float x, float y, float z, float rx, float sx = 1, float sy = 1)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.rx = rx;
            this.sx = sx;
            this.sy = sy;
        }

        public override string? ToString()
        {
            return $"{x}, {y}, {z}, {rx}, {sx}, {sy}";
        }
        //public float sz;
    }
}
