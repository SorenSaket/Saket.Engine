
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saket.Engine.Geometry2D.Curves;

namespace Saket.Engine.Geometry2D.Splines;

/// <summary>
/// A 2d spline with support for multiple different Curve types
/// </summary>
public class MultiSpline2D
{
    public List<CurveType> curveTypes;

    public List<Vector2> points;

}
