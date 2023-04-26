using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine
{
    public static class Mathf
    {
		public static readonly float sqrt2 = MathF.Sqrt(2f);
		public const float RadToDeg = (180f / MathF.PI) ;
		public const float DegToRad = (MathF.PI / 180f);

        /// <summary>
        /// Returns 1 for non-negative values and -1 for negative values. Returns 1 in case of 0.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NonZeroSign(double n)
        {
            return n >= 0 ? 1 : -1;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * Clamp01(t);
        }

        // Interpolates between /a/ and /b/ by /t/ without clamping the interpolant.


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LerpUnclamped(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 LerpUnclamped(Vector2 a, Vector2 b, float t)
        {
            return new Vector2(LerpUnclamped(a.X, b.X, t), LerpUnclamped(a.Y, b.Y, t));
        }

        // Same as ::ref::Lerp but makes sure the values interpolate correctly when they wrap around 360 degrees.


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LerpAngle(float a, float b, float t)
        {
            float delta = Repeat((b - a), 360);
            if (delta > 180)
                delta -= 360;
            return a + delta * Clamp01(t);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LerpAngleRad(float a, float b, float t)
        {
            float delta = Repeat((b - a), MathF.PI*2f);
            if (delta > MathF.PI)
                delta -= MathF.PI*2f;
            return a + delta * Clamp01(t);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp01(float value)
        {
            if (value < 0F)
                return 0F;
            else if (value > 1F)
                return 1F;
            else
                return value;
        }
        // Clamps a value between a minimum float and maximum float value.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Repeat(float t, float length)
        {
            return Clamp(t - MathF.Floor(t / length) * length, 0.0f, length);
        }

        /// <summary>
        /// Returns the middle out of three values
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Median(float a, float b, float c)
        {
            return MathF.Max(MathF.Min(a, b), MathF.Min(MathF.Max(a, b), c));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(params float[] values)
        {
            return values.Min();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(params float[] values)
        {
            return values.Max();
        }
    }
}
