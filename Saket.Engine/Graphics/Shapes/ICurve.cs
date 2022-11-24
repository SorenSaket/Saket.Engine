using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Graphics.Shapes
{
    public interface ICurve2D
    {
        /// <summary>
        /// Returns a point on the curve at specfic t value
        /// </summary>
        /// <param name="t">Percentage along the curve 0-1</param>
        /// <returns>Point on curve</returns>
        public Vector2 Evaluate(float t);

        /// <summary>
        /// Returns the tanget at specfic t value
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector2 Direction(float t);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="t">The t value for the closest point on the curve.</param>
        /// <returns></returns>
        public SignedDistance SignedDistance(Vector2 point, out float t);
        
        /// <summary>
		/// Converts a previously retrieved signed distance from origin to pseudo-distance.
		/// </summary>
		/// <param name="distance"></param>
		/// <param name="origin"></param>
		/// <param name="param"></param>
		public void DistanceToPseudoDistance(ref SignedDistance distance, Vector2 origin, float param)
        {
            if (param < 0)
            {
                var dir = Vector2.Normalize(Direction(0));
                var aq = origin - Evaluate(0);
                var ts = Vector2.Dot(aq, dir);
                if (ts < 0)
                {
                    var pseudoDistance = Extensions_Vector2.Cross(aq, dir);
                    if (Math.Abs(pseudoDistance) <= Math.Abs(distance.Distance))
                    {
                        distance.Distance = pseudoDistance;
                        distance.Dot = 0;
                    }
                }
            }
            else if (param > 1)
            {
                var dir = Vector2.Normalize(Direction(1));
                var bq = origin - Evaluate(1);
                var ts = Vector2.Dot(bq, dir);
                if (ts > 0)
                {
                    var pseudoDistance = Extensions_Vector2.Cross(bq, dir);
                    if (Math.Abs(pseudoDistance) <= Math.Abs(distance.Distance))
                    {
                        distance.Distance = pseudoDistance;
                        distance.Dot = 0;
                    }
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns>AABB of the extremidies of the curve</returns>
        public Bounds Bounds();
    }
}
