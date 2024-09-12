
using Saket.Engine.Math.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Saket.Engine
{
    public static class Extensions_Vector2 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// The cross product is zero when the vectors are pointing in the same or opposite direction. <br/>
        /// 
        /// </remarks>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cross(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }


        /// <summary>
        /// Returns a vector with unit length that is orthogonal to this one
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetOrthonormal(Vector2 value)
        {
            var len = value.Length();
            return len == 0f ? new Vector2(0f, -1f) : new Vector2(value.Y / len, -value.X / len);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Floor(this Vector2 vector)
        {
            return new Vector2(MathF.Floor(vector.X), MathF.Floor(vector.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Round(this Vector2 vector)
        {
            return new Vector2(MathF.Round(vector.X), MathF.Round(vector.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int2 RoundToInt2(this Vector2 vector)
        {
            return new Int2((int)MathF.Round(vector.X), (int)MathF.Round(vector.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RectNormal(this Vector2 v)
        {
            if (MathF.Abs(v.X) > MathF.Abs(v.Y))
                return new Vector2(MathF.Sign(v.X), 0);
            return new Vector2(0, MathF.Sign(v.Y));
        }

        public static bool IsWithin(this Vector2 v, Vector2 min, Vector2 max)
        {
            return v.X >= min.X && v.X < max.X && 
                    v.Y >= min.Y && v.Y < max.Y;
        }
    }
}
