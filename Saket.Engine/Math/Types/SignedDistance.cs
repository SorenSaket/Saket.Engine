using System;
using System.Runtime.CompilerServices;

namespace Saket.Engine.Math
{
    /// <summary>
    /// Represents a signed distance and alignment, which together can be compared to uniquely determine the closest edge segment.
    /// </summary>
    public struct SignedDistance
    {
        public static readonly SignedDistance Infinite = new SignedDistance(float.NegativeInfinity, 1);

        public float Distance;

        /// <summary>
        /// Dot is zero when the t value is outside of the range of 0..1
        /// 
        /// </summary>
        public float Dot;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SignedDistance(float dist, float d)
        {
            Distance = dist;
            Dot = d;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(SignedDistance a, SignedDistance b)
        {
            return MathF.Abs(a.Distance) < MathF.Abs(b.Distance) ||
                   MathF.Abs(a.Distance) == MathF.Abs(b.Distance) && a.Dot < b.Dot;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(SignedDistance a, SignedDistance b)
        {
            return MathF.Abs(a.Distance) > MathF.Abs(b.Distance) ||
                   MathF.Abs(a.Distance) == MathF.Abs(b.Distance) && a.Dot > b.Dot;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(SignedDistance a, SignedDistance b)
        {
            return MathF.Abs(a.Distance) < MathF.Abs(b.Distance) ||
                   MathF.Abs(a.Distance) == MathF.Abs(b.Distance) && a.Dot <= b.Dot;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(SignedDistance a, SignedDistance b)
        {
            return MathF.Abs(a.Distance) > MathF.Abs(b.Distance) ||
                   MathF.Abs(a.Distance) == MathF.Abs(b.Distance) && a.Dot >= b.Dot;
        }
    }
}