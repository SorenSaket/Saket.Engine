
using System;
using System.Numerics;

namespace Saket.Engine
{
    public static class Extensions_Vector3
    {

        // Math
        public static Vector3 Abs(this Vector3 v)
        {
            return new Vector3(MathF.Abs(v.X), MathF.Abs(v.Y), MathF.Abs(v.Z));
        }
        public static Vector3 Floor(this Vector3 v)
        {
            return new Vector3(MathF.Floor(v.X), MathF.Floor(v.Y), MathF.Floor(v.Z));
        }
        public static Vector3 Round(this Vector3 v)
        {
            return new Vector3(MathF.Round(v.X), MathF.Round(v.Y), MathF.Round(v.Z));
        }

        // To vector 2
        public static Vector2 XY(this Vector3 v)
        {
            return new Vector2(v.X, v.Y);
        }
        public static Vector2 XZ(this Vector3 v)
        {
            return new Vector2(v.X, v.Z);
        }

        // To Vector 3

        public static Vector3 ToVector3(this float f)
        {
            return new Vector3(f, f, f);
        }
        public static Vector3 ToVector3XY(this Vector2 v)
        {
            return new Vector3(v.X, v.Y, 0);
        }
        public static Vector3 ToVector3XZ(this Vector2 v)
        {
            return new Vector3(v.X, 0, v.Y);
        }

        public static Vector3 SelectXZ(this Vector3 f)
        {
            return new Vector3(f.X, 0, f.Z);
        }

    }
}