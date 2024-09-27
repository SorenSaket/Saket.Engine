using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saket.Engine.Types;

namespace Saket.Engine.Graphics.SDF
{
    /// <summary>
    /// Describes a point along a spline.
    /// </summary>
    public struct SplinePoint
    {
        public static readonly SplinePoint Default = new SplinePoint
        {
            MinDistance = SignedDistance.Infinite,
            NearEdge = -1,
            NearT = 0
        };

        /// <summary>
        /// 
        /// </summary>
        public SignedDistance MinDistance;
        /// <summary>
        /// Index of edge that is closet to the point
        /// </summary>
        public int NearEdge;
        /// <summary>
        /// The T value for the nearest point on a curve
        /// </summary>
        public float NearT;
    }
}
