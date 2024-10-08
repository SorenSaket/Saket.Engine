using Saket.Engine.Types;
using Saket.Serialization;
using System;
using System.Numerics;

namespace Saket.Engine.Geometry2D.Shapes;

public enum ShapeType
{
    Undefined,
    Circle,
    Rectangle
}

public class Shape : IShape, ISerializable
{
    public ShapeType ShapeType;
    /// <summary>
    /// The center
    /// </summary>
    public Vector2 Position;
    /// <summary>
    /// The size
    /// </summary>
    public Vector2 Size;
    /// <summary>
    /// Rotation angle in radias
    /// </summary>
    public float Rotation;

    public BoundingBox2D GetBounds()
    {
        throw new NotImplementedException();
    }

    public SignedDistance GetSignedDistance(Vector2 point)
    {
        throw new NotImplementedException();
    }

    public void Serialize(ISerializer serializer)
    {
        throw new NotImplementedException();
    }
}
