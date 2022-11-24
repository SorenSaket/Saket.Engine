using Saket.Engine.Graphics.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Graphics.MSDF
{
    public struct EdgePoint
    {
        public static readonly EdgePoint Default = new EdgePoint
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
