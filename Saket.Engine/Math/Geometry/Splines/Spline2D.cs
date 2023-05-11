using System;
using System.Numerics;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Saket.Engine.Math.Geometry
{
    /// <summary>
    /// A single continuous contour of a shape.
    /// Made of quadratic bezier splines.
    /// </summary>
    public class Spline2D : IEnumerable<ICurve2D>
    {
        /// <summary>
        /// The number of cures in this spline. There are 
        /// </summary>
        public int Curves => points.Count / 2;

        /// <summary>
        /// The list of points 
        /// </summary>
        public List<Vector2> points;

        public Spline2D( List<Vector2> points)
        {
            this.points = points;
        }

        public ICurve2D this[int i]
        {
            get { return GetCurve(i); }
        }

        public QuadraticBezier GetCurve(int index)
        {
            int si = index * 2;
            return new QuadraticBezier(points[si], points[si + 1], points[(si + 2)%(points.Count)]);
        }

        /// <summary>
        /// Computes the bounding box of the contour.
        /// </summary>
        /// <param name="box"></param>
        public BoundingBox2D Bounds()
        {
            throw new NotImplementedException();
            //foreach (var edge in this) edge.Bounds(box);
        }

        /// <summary>
        /// Computes the winding of the contour. Returns 1 if clockwise, -1 if counter clockwise.
        /// </summary>
        /// <returns>Either -1, 1 depending on winding. returns 0 in case winding cannot be found.</returns>
        public int Winding()
        {
            if (Curves == 0)
                return 0;

            switch (Curves)
            {
                case 1:
                    {
                        Vector2 a = this[0].Evaluate(0),
                        b = this[0].Evaluate(1.0f / 3.0f),
                        c = this[0].Evaluate(2.0f / 3.0f);
                        return MathF.Sign(Shoelace(a, b) + Shoelace(b, c) + Shoelace(c, a));
                    }
                case 2:
                    {
                        Vector2 a = this[0].Evaluate(0),
                         b = this[0].Evaluate(.5f),
                         c = this[1].Evaluate(0),
                         d = this[1].Evaluate(.5f);

                        return MathF.Sign(Shoelace(a, b) + Shoelace(b, c) + Shoelace(c, d) + Shoelace(d, a));
                    }
                default:
                    var total = 0.0f;
                    var prev = this[Curves - 1].Evaluate(0);
                    foreach (var edge in this)
                    {
                        var cur = edge.Evaluate(0);
                        total += Shoelace(prev, cur);
                        prev = cur;
                    }
                    return MathF.Sign(total);
            }
        }
        private static float Shoelace(Vector2 a, Vector2 b)
        {
            return (b.X - a.X) * (a.Y + b.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spline"></param>
        /// <param name="corners"></param>
        /// <param name="crossThreshold"></param>
        public void GetCorners(List<int> corners, float crossThreshold)
        {
            corners.Clear();

            // This works regardless if the curve is closed or not?
            // the first is always a corner if the spline is not closed

            var prevDirection = GetCurve(Curves - 1).Direction(1);

            for (int i = 0; i < Curves; i++)
            {
                var curve = GetCurve(i);

                if (IsCorner(Vector2.Normalize(prevDirection), Vector2.Normalize(curve.Direction(0)), crossThreshold))
                {
                    corners.Add(i);
                }

                prevDirection = curve.Direction(1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aDir"></param>
        /// <param name="bDir"></param>
        /// <param name="crossThreshold"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCorner(Vector2 aDir, Vector2 bDir, float crossThreshold)
        {
            return
                // Dot products is less than one when the vectors are pointing away from each other
                // Dot product is 0 when perpendicular
                // return true if they're pointing in opposite directions or perpendicular
                Vector2.Dot(aDir, bDir) <= 0 ||
                // The cross product is zero when the vectors are pointing in the same or opposite direction. 
                // if they're off by more than crossThreshold return true.
                MathF.Abs(Extensions_Vector2.Cross(aDir, bDir)) > crossThreshold;
        }

        public IEnumerator<ICurve2D> GetEnumerator()
        {
            return new SplineEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SplineEnumerator(this);
        }
    }


    public struct SplineEnumerator : IEnumerator, IEnumerator<ICurve2D>
    {
        public object Current => spline.GetCurve(position);
        ICurve2D IEnumerator<ICurve2D>.Current => spline.GetCurve(position);

        int position = -1;

        private Spline2D spline;

        public SplineEnumerator(Spline2D spline) : this()
        {
            this.spline = spline;
        }

        public bool MoveNext()
        {
            position++;
            return (position < spline.Curves);
        }

        public void Reset()
        {
            position = -1;
        }

        public void Dispose()
        {
            
        }
    }
}