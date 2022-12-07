using System;
using System.Runtime.CompilerServices;

namespace Saket.Engine
{

    public static class Solver
    {
        // https://www.realtimerendering.com/resources/GraphicsGems/gems/Roots3And4.c


        // Tests to find where the equation intersects the x axis

        // Functions returns the number of intersections
        // -1: for infinite intersections
        // 0: no intersections



        /// <summary>
        /// Solves equation in the form ax + b 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>The number of intersections</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SolveLinear(float a, float b, out float intersection)
        {
            intersection = 0;
            // In case the equation is parrallel with x axis 
            if (MathF.Abs(a) < 1e-14)
            {
                // Always intersects with x axis
                if (b == 0)
                    return -1;
                // Never intersects with x axis
                return 0;
            }
            // find intersection
            intersection = -b / a;
            return 1;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SolveQuadratic(float a, float b, float c, out float intersection1, out float intersection2)
        {
            // If the equation has no curvature it effectivly becomes a linear equation
            if (MathF.Abs(a) < 1e-14)
            {
                int r = SolveLinear(b, c, out intersection1);
                intersection2 = 0;
                return r;
            }

            var discriminant = b * b - 4 * a * c;

            // Two intersections
            if (discriminant > 0)
            {
                discriminant = MathF.Sqrt(discriminant);
                intersection1 = (-b + discriminant) / (2 * a);
                intersection2 = (-b - discriminant) / (2 * a);
                return 2;
            }

            if (discriminant == 0)
            {
                intersection1 = -b / (2 * a);
                intersection2 = 0;
                return 1;
            }

            intersection1 = intersection2 = 0;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SolveCubicNormed(float a, float b, float c, out float intersection1, out float intersection2, out float intersection3)
        {
            float a2 = a * a;
            float q = (a2 - 3f * b) / 9f;
            float r = (a * (2f * a2 - 9f * b) + 27f * c) / 54f;
            float r2 = r * r;
            float q3 = q * q * q;
            
            
            if (r2 < q3)
            {
                float t = r / MathF.Sqrt(q3);
                if (t < -1) t = -1;
                if (t > 1) t = 1;
                t = MathF.Acos(t);
                a /= 3;
                q = -2 * MathF.Sqrt(q);
                intersection1 = q * MathF.Cos(t / 3) - a;
                intersection2 = q * MathF.Cos((t + 2 * MathF.PI) / 3) - a;
                intersection3 = q * MathF.Cos((t - 2 * MathF.PI) / 3) - a;
                return 3;
            }

            float aa = -MathF.Pow(MathF.Abs(r) + MathF.Sqrt(r2 - q3), 1f / 3.0f);
            if (r < 0) aa = -aa;
            float bb = aa == 0 ? 0 : q / aa;
            a /= 3;
            intersection1 = aa + bb - a;
            intersection2 = -0.5f * (aa + bb) - a;
            intersection3 = 0.5f * MathF.Sqrt(3.0f) * (aa - bb);
            return MathF.Abs(c) < 1e-14 ? 2 : 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SolveCubic(float a, float b, float c, float d, out float intersection1, out float intersection2, out float intersection3)
        {
            if(MathF.Abs(a) < 1e-14)
            {
                var r = SolveQuadratic(b,c,d, out intersection1, out intersection2);
                intersection3 = 0;
                return r; 
            }
            else
            {
                return SolveCubicNormed(b / a, c / a, d / a, out intersection1, out intersection2, out intersection3);
            }
        }
    }
}
