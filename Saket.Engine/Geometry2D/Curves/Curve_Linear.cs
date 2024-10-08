using Saket.Engine.Types;
using System;
using System.Numerics;

namespace Saket.Engine.Geometry2D.Curves;

/// <summary>
/// Bezier curve from 4 points
/// </summary>
public struct Curve_Linear : ICurve2D
{
    public Vector2 start;
    public Vector2 end;


    public BoundingBox2D GetBounds()
    {
        throw new NotImplementedException();
    }

    public Vector2 Direction(float t)
    {
        throw new NotImplementedException();
    }

    public Vector2 Evaluate(float t)
    {
        throw new NotImplementedException();

    }

    public SignedDistance SignedDistance(Vector2 point, out float t)
    {
        throw new NotImplementedException();
    }

}