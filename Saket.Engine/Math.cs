using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine
{
    public static class Math
    {
        public static float RadToDeg(float radians)
        {
            return (180 / MathF.PI) * radians;
        }

        public static float DegToRad(float degrees)
        {
            return degrees * (MathF.PI/180);
        }
    }
}
