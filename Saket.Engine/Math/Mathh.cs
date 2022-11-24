using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine
{
    public static class Mathh
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
        public static float LerpUnclamped(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        // Same as ::ref::Lerp but makes sure the values interpolate correctly when they wrap around 360 degrees.
        public static float LerpAngle(float a, float b, float t)
        {
            float delta = Repeat((b - a), 360);
            if (delta > 180)
                delta -= 360;
            return a + delta * Clamp01(t);
        }
        public static float LerpAngleRad(float a, float b, float t)
        {
            float delta = Repeat((b - a), MathF.PI*2f);
            if (delta > MathF.PI)
                delta -= MathF.PI*2f;
            return a + delta * Clamp01(t);
        }
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
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }
        public static float Repeat(float t, float length)
        {
            return Clamp(t - MathF.Floor(t / length) * length, 0.0f, length);
        }
    }
}
