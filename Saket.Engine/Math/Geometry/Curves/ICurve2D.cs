using System;
using System.Numerics;

namespace Saket.Engine.Math.Geometry
{
    /// <summary>
    /// A curve in two dimentions
    /// </summary>
    public interface ICurve2D : IShape
    {
        /// <summary>
        /// Returns a point on the spline at specfic t value
        /// </summary>
        /// <param name="t">Percentage along the spline 0-1</param>
        /// <returns>Point on spline</returns>
        public Vector2 Evaluate(float t);

        /// <summary>
        /// Returns the tanget at specfic t value
        /// </summary>
        /// <param name="t">Percentage along the spline 0-1</param>
        /// <returns></returns>
        public Vector2 Direction(float t);

        /// <summary>
        /// Returns the signed distance from curve to point
        /// </summary>
        /// <param name="point"></param>
        /// <param name="t">The t value for the closest point on the spline.</param>
        /// <returns></returns>
        public SignedDistance SignedDistance(Vector2 point, out float t);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
         SignedDistance ISDF2D.GetSignedDistance(Vector2 point) => SignedDistance(point, out _);

        /// <summary>
        /// Converts a previously retrieved signed distance from origin to pseudo-distance.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="point"></param>
        /// <param name="t"></param>
        public void DistanceToPseudoDistance(ref SignedDistance distance, Vector2 point, float t)
        {
            if (t < 0)
            {
                var dir = Vector2.Normalize(Direction(0));
                var aq = point - Evaluate(0);
                var ts = Vector2.Dot(aq, dir);
                if (ts < 0)
                {
                    var pseudoDistance = Extensions_Vector2.Cross(aq, dir);
                    if (MathF.Abs(pseudoDistance) <= MathF.Abs(distance.Distance))
                    {
                        distance.Distance = pseudoDistance;
                        distance.Dot = 0;
                    }
                }
            }
            else if (t > 1)
            {
                var dir = Vector2.Normalize(Direction(1));
                var bq = point - Evaluate(1);
                var ts = Vector2.Dot(bq, dir);
                if (ts > 0)
                {
                    var pseudoDistance = Extensions_Vector2.Cross(bq, dir);
                    if (MathF.Abs(pseudoDistance) <= MathF.Abs(distance.Distance))
                    {
                        distance.Distance = pseudoDistance;
                        distance.Dot = 0;
                    }
                }
            }
        }

    }
}
