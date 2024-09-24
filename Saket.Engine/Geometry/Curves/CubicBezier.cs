using System;
using System.Numerics;

namespace Saket.Engine.Geometry.Curves;

/// <summary>
/// Bezier curve from 4 points
/// </summary>
public struct CubicBezier : ICurve2D
{
    public Vector2 start;
    public Vector2 controlA;
    public Vector2 controlB;
    public Vector2 end;




    public BoundingBox2D Bounds()
    {
        throw new NotImplementedException();
    }

    public Vector2 Direction(float t)
    {
        Vector2 tangent = Mathf.LerpUnclamped(
            Mathf.LerpUnclamped(controlA - start, controlB - controlA, t),
            Mathf.LerpUnclamped(controlB - controlA, end - controlB, t),
            t);

        return tangent;
    }

    public Vector2 Evaluate(float t)
    {
        var t2 = t * t;
        var t3 = t2 * t;

        // Bernstein form
        //return (start * (-t3 + 3f * t2 - 3f * t + 1f)) +
        //       (controlA * (3f * t3 - 6f * t2 + 3f * t)) +
        //       (controlB * (-3f * t3 + 3f * t2)) +
        //       (end * t3);

        // Polynomial Coefficients
        return start +
                t * (-3f * start + 3f * controlA) +
                t2 * (3f * start - 6f * controlA + 3f * controlB) +
                t3 * (-start + 3f * controlA - 3f * controlB + end);

    }

    public SignedDistance SignedDistance(Vector2 point, out float t)
    {
        throw new NotImplementedException();
    }

}
