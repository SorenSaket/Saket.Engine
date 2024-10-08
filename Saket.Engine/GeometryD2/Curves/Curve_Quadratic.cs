using Saket.Engine.Types;
using System;
using System.Numerics;

namespace Saket.Engine.GeometryD2.Curves;

/// <summary>
/// Bezier cruve from 3 points
/// </summary>
public struct Curve_Quadratic : ICurve2D
{
    /// <summary>
    /// Starting point
    /// </summary>
    public Vector2 Start;
    /// <summary>
    /// Control point
    /// </summary>
    public Vector2 Control;
    /// <summary>
    /// End point
    /// </summary>
    public Vector2 End;

    public Curve_Quadratic(Vector2 a, Vector2 b, Vector2 c)
    {
        Start = a;
        Control = b;
        End = c;
    }

    public Vector2 Evaluate(float t)
    {
        return Vector2.Lerp(Vector2.Lerp(Start, Control, t), Vector2.Lerp(Control, End, t), t);
    }

    public Vector2 Direction(float t)
    {
        return Vector2.Lerp(Control - Start, End - Control, t);
    }

    public SignedDistance SignedDistance(Vector2 origin, out float t)
    {
        // todo check validity of code
        // Figure out how it works

        Vector2 qa = Start - origin;
        Vector2 ab = Control - Start;
        Vector2 br = End - Control - ab;

        int intersectionCount = Solver.SolveCubic(
            Vector2.Dot(br, br),
            3f * Vector2.Dot(ab, br),
            2f * Vector2.Dot(ab, ab) + Vector2.Dot(qa, br),
            Vector2.Dot(qa, ab),
            out float t1,
            out float t2,
            out float t3
            );

        Vector2 epDir = Direction(0);
        float minDistance = Mathf.NonZeroSign(Extensions_Vector2.Cross(epDir, qa)) * qa.Length(); // distance from A

        t = -Vector2.Dot(qa, epDir) / Vector2.Dot(epDir, epDir);


        {
            epDir = Direction(1);
            float distance = (End - origin).Length(); // distance from B
            if (MathF.Abs(distance) < MathF.Abs(minDistance))
            {
                minDistance = Mathf.NonZeroSign(Extensions_Vector2.Cross(epDir, End - origin)) * distance;

                // taking a vector dot product of itself is equal to the square of its magnitude
                t = Vector2.Dot(origin - Control, epDir) / Vector2.Dot(epDir, epDir);
            }
        }

        Span<float> solutions = stackalloc float[3] { t1, t2, t3 };

        for (var i = 0; i < intersectionCount; ++i)
        {
            if (solutions[i] > 0 && solutions[i] < 1f)
            {
                Vector2 qe = qa + 2f * solutions[i] * ab + solutions[i] * solutions[i] * br;
                float distance = qe.Length();
                if (distance <= MathF.Abs(minDistance))
                {
                    minDistance = Mathf.NonZeroSign(Extensions_Vector2.Cross(ab + solutions[i] * br, qe)) * distance;
                    t = solutions[i];
                }
            }
        }


        if (t >= 0f && t <= 1f)
            return new SignedDistance(minDistance, 0);
        if (t < 0.5f)
            return new SignedDistance(minDistance, MathF.Abs(Vector2.Dot(Vector2.Normalize(Direction(0)), Vector2.Normalize(qa))));
        return new SignedDistance(minDistance, MathF.Abs(Vector2.Dot(Vector2.Normalize(Direction(1)), Vector2.Normalize(End - origin))));
    }

    //TODO doc
    public BoundingBox2D GetBounds()
    {
        BoundingBox2D bounds = BoundingBox2D.Null;
        bounds.AddPoint(Start);
        bounds.AddPoint(End);

        Vector2 bot = Control - Start - (End - Control);

        // null division guard
        if (bot.X != 0f)
        {
            var param = (Control.X - Start.X) / bot.X;

            if (param > 0f && param < 1f)
                bounds.AddPoint(Evaluate(param));
        }

        if (bot.Y != 0f)
        {
            var param = (Control.Y - Start.Y) / bot.Y;
            if (param > 0f && param < 1f)
                bounds.AddPoint(Evaluate(param));
        }

        return bounds;
    }

    public void SplitInThirds(out Curve_Quadratic part1, out Curve_Quadratic part2, out Curve_Quadratic part3)
    {
        throw new NotImplementedException();
        /*
        part1 = new QuadraticBezier(a, Arithmetic.Mix(a, c, 1.0 / 3.0), Point(1.0 / 3.0));

        part2 = new QuadraticBezier(Color, Point(1.0 / 3.0),
            Arithmetic.Mix(Arithmetic.Mix(a, c, 5.0 / 9.0), Arithmetic.Mix(c, b, 4.0 / 9.0), 0.5),
            Point(2 / 3.0));
        part3 = new QuadraticBezier(Color, Point(2.0 / 3.0), Arithmetic.Mix(c, b, 2.0 / 3.0), b);
    */
    }

}
