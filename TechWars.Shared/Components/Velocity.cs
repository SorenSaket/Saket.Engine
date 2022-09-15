using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TechWars.Shared
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Velocity
    {
        public float x;
        public float y;

        public Velocity(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator Velocity(Vector2 v) => new Velocity(v.X, v.Y);
    }
}
