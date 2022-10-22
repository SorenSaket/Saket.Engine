
using System;
using System.Numerics;

namespace Saket.Engine
{
    public static class NumericsExtentions
    {
        public static Vector3 XYZ(this Vector4 vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        public static Vector3 Floor(this Vector3 vector)
        {
            return new Vector3(MathF.Floor(vector.X), MathF.Floor(vector.Y), MathF.Floor(vector.Z));
        }

        public static Vector2 Floor(this Vector2 vector)
        {
            return new Vector2(MathF.Floor(vector.X), MathF.Floor(vector.Y));
        }

        public static Vector2 Round(this Vector2 vector)
        {
            return new Vector2(MathF.Round(vector.X), MathF.Round(vector.Y));
        }



    }
}
