
using System.Numerics;

namespace Saket.Engine
{
    public static class NumericsExtentions
    {
        public static Vector3 XYZ(this Vector4 vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }


    }
}
