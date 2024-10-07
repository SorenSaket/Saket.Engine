using System.Numerics;

namespace Saket.Engine.Geometry;

/// <summary>
/// Represents an Axis Aligned Bounding Box
/// </summary>
public struct BoundingBox2D
{
    public static readonly BoundingBox2D Null = new(new Vector2(float.PositiveInfinity), new Vector2(float.NegativeInfinity));
    public readonly Vector2 Size => max - min;

    public Vector2 min;
    public Vector2 max;



    public BoundingBox2D()
    {
    }

    public BoundingBox2D(Vector2 min, Vector2 max)
    {
        this.min = min;
        this.max = max;
    }

    public BoundingBox2D(float minX, float minY, float maxX, float maxY)
    {
        min = new Vector2(minX, minY);
        max = new Vector2(maxX, maxY);
    }

    public void AddPoint(Vector2 p)
    {
        if (p.X < min.X)
            min.X = p.X;
        if (p.Y < min.Y)
            min.Y = p.Y;
        if (p.X > max.X)
            max.X = p.X;
        if (p.Y > max.Y)
            max.Y = p.Y;
    }

    public void Combine(BoundingBox2D b)
    {
        AddPoint(b.min);
        AddPoint(b.max);
    }
}
