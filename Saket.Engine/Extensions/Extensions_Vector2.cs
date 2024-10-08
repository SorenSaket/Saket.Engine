using Saket.Engine.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Saket.Engine;

public static class Extensions_Vector2 
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// The cross product is zero when the vectors are pointing in the same or opposite direction. <br/>
    /// 
    /// </remarks>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cross(Vector2 a, Vector2 b)
    {
        return a.X * b.Y - a.Y * b.X;
    }


    /// <summary>
    /// Returns a vector with unit length that is orthogonal to this one
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 GetOrthonormal(Vector2 value)
    {
        var len = value.Length();
        return len == 0f ? new Vector2(0f, -1f) : new Vector2(value.Y / len, -value.X / len);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Floor(this Vector2 vector)
    {
        return new Vector2(MathF.Floor(vector.X), MathF.Floor(vector.Y));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Round(this Vector2 vector)
    {
        return new Vector2(MathF.Round(vector.X), MathF.Round(vector.Y));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Ceiling(this Vector2 vector)
    {
        return new Vector2(MathF.Ceiling(vector.X), MathF.Ceiling(vector.Y));
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int2 RoundToInt2(this Vector2 vector)
    {
        return new Int2((int)MathF.Round(vector.X), (int)MathF.Round(vector.Y));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 RectNormal(this Vector2 v)
    {
        if (MathF.Abs(v.X) > MathF.Abs(v.Y))
            return new Vector2(MathF.Sign(v.X), 0);
        return new Vector2(0, MathF.Sign(v.Y));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Sign(this Vector2 v)
    {
        return new Vector2(MathF.Sign(v.X), MathF.Sign(v.Y));
    }

    public static bool IsWithin(this Vector2 v, Vector2 min, Vector2 max)
    {
        return v.X >= min.X && v.X < max.X && 
                v.Y >= min.Y && v.Y < max.Y;
    }

    public static bool IsCollinear(Vector2 a, Vector2 b, Vector2 c, float e = 0.0001f)
    {
        // Calculate the area of the triangle formed by the points
        // If the area is 0, the points are collinear
        float area = (a.X) * (b.Y - c.Y) + b.X * (c.Y - a.Y) + c.X * (a.Y - b.Y);
        return area < e;
    }



    public static (Vector2 min, Vector2 max) MinMax(Vector2 A, Vector2 B)
    {
        return (
               new Vector2(MathF.Min(A.X,B.X), MathF.Min(A.Y, B.Y)),
                new Vector2(MathF.Max(A.X, B.X), MathF.Max(A.Y, B.Y)));
    }


    public static float Min(this Vector2 value)
    {
        return MathF.Min(value.X, value.Y);
    }

    public static float Max(this Vector2 value)
    {
        return MathF.Max(value.X, value.Y);
    }
}