
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Math.Geometry
{
    /// <summary>
    /// A 2d spline with support for multiple different Curve types
    /// </summary>
    public class MultiSpline2D
    {
        public List<CurveType> curveTypes;

        public List<Vector2> points;

    }
}
