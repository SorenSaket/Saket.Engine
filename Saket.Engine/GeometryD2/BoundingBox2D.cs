using Saket.Engine.GeometryD2.Shapes;
using Saket.Serialization;
using System;
using System.Diagnostics;
using System.Numerics;

namespace Saket.Engine.GeometryD2;

/// <summary>
/// Represents an Axis Aligned Bounding Box
/// </summary>
public struct BoundingBox2D : ISerializable
{
    public static readonly BoundingBox2D Null = new() { Min = new Vector2(float.PositiveInfinity), Max = new Vector2(float.NegativeInfinity) };
    public readonly Vector2 Size => Max - Min;

    public readonly float Top => Max.Y;
    public readonly float Bottom => Min.Y;
    public readonly float Left => Min.X;
    public readonly float Right => Max.X;


    public Vector2 Min;
    public Vector2 Max;



    public BoundingBox2D()
    {
        this = Null;
    }
    
    
    public BoundingBox2D(params ReadOnlySpan<Vector2> points)
    {
        this = Null;
        for (int i = 0; i < points.Length; i++)
        {
            AddPoint(points[i]);
        }
    }
    
    public BoundingBox2D(Vector2 min, Vector2 max)
    {
        this.Min = Vector2.Min(min, max);
        this.Max = Vector2.Max(min, max);
    }

    public BoundingBox2D(float minX, float minY, float maxX, float maxY)
    {
        var a = new Vector2(minX, minY);
        var b = new Vector2(maxX, maxY);
        this.Min = Vector2.Min(a, b);
        this.Max = Vector2.Max(a, b);
    }

    public readonly Rectangle ToRect()
    {
        return new Rectangle(this);
    }

    public void AddPoint(Vector2 p)
    {
        if (p.X < Min.X)
            Min.X = p.X;
        if (p.Y < Min.Y)
            Min.Y = p.Y;
        if (p.X > Max.X)
            Max.X = p.X;
        if (p.Y > Max.Y)
            Max.Y = p.Y;
    }

    public void Combine(BoundingBox2D b)
    {
        AddPoint(b.Min);
        AddPoint(b.Max);
    }

    public bool IsWithin(Vector2 point)
    {
        float minX = Math.Min(Min.X, Max.X);
        float maxX = Math.Max(Min.X, Max.X);
        float minY = Math.Min(Min.Y, Max.Y);
        float maxY = Math.Max(Min.Y, Max.Y);

        return (point.X >= minX && point.X <= maxX) &&
               (point.Y >= minY && point.Y <= maxY);
    }

    public void Serialize(ISerializer serializer)
    {
        serializer.LoadBytes(16);
        serializer.Serialize(ref Min);
        serializer.Serialize(ref Max);
    }
}