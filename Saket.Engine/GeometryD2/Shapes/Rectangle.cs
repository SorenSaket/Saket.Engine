using Saket.Engine.Types;
using Saket.Serialization;
using System;
using System.Numerics;

namespace Saket.Engine.GeometryD2.Shapes;

public struct Rectangle : IShape, ISerializable
{
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

    public float X
    {
        get { return Position.X; }
        set { Position.X = value; }
    }
    public float Y
    {
        get { return Position.Y; }
        set { Position.Y = value; }
    }
    public float Width
    {
        get { return Size.X; }
        set { Size.X = value; }
    }
    public float Height
    {
        get { return Size.Y; }
        set { Size.Y = value; }
    }

    public readonly Vector2 TopLeft
    {
        get
        {
            return Position+new Vector2(-Size.X, Size.Y)/2f;
        }
    }
    public readonly Vector2 TopRight
    {
        get
        {
            return Position + new Vector2(Size.X, Size.Y) / 2f;
        }
    }
    public readonly Vector2 BottomLeft
    {
        get
        {
            return Position + new Vector2(-Size.X, -Size.Y) / 2f;
        }
    }
    public readonly Vector2 BottomRight
    {
        get
        {
            return Position + new Vector2(Size.X, -Size.Y) / 2f;
        }
    }




    public Rectangle()
    {
        Position = Vector2.Zero;
        Size = Vector2.One;
        Rotation = 0;
    }
    public Rectangle(Vector2 position, Vector2 size, float rotation = 0)
    {
        Position = position;
        Size = size;
        Rotation = rotation;
    }

    public Rectangle(float position_X = 0, float position_Y = 0, float size_X =1, float size_Y = 1, float rotation = 0)
    {
        Position = new Vector2(position_X, position_Y);
        Size = new Vector2(size_X, size_Y);
        Rotation = rotation;
    }


    public Rectangle(BoundingBox2D box)
    {
        Position = (box.Min + box.Max) / 2f;
        Size = box.Max - box.Min;
        Rotation = 0;
    }

    public float Area()
    {
        return Size.X * Size.Y;
    }

    public BoundingBox2D GetBounds()
    {
        // Define the corners of the untransformed rectangle (from -0.5 to 0.5)
        Vector2[] corners = 
        [
            new (-0.5f, -0.5f),
            new (0.5f, -0.5f),
            new (0.5f, 0.5f),
            new (-0.5f, 0.5f)
        ];

        // Create the transformation matrix for the bounding box
        Matrix3x2 transform = this.CreateTransformMatrix();

        // Transform the corners
        for (int i = 0; i < corners.Length; i++)
            corners[i] = Vector2.Transform(corners[i], transform);

        // Find the bounding rectangle
        float minX = Extensions_Math.Min(corners[0].X, corners[1].X, corners[2].X, corners[3].X);
        float minY = Extensions_Math.Min(corners[0].Y, corners[1].Y, corners[2].Y, corners[3].Y);
        float maxX = Extensions_Math.Max(corners[0].X, corners[1].X, corners[2].X, corners[3].X);
        float maxY = Extensions_Math.Max(corners[0].Y, corners[1].Y, corners[2].Y, corners[3].Y);

        return new BoundingBox2D(new Vector2(minX,minY), new Vector2(maxX, maxY));
    }

    public SignedDistance GetSignedDistance(Vector2 point)
    { 
        // Get the inverse transformation matrix to convert point to local space
        Matrix3x2 transform = CreateTransformMatrix();
        Matrix3x2.Invert(transform, out Matrix3x2 inverseTransform);

        // Transform the point to the rectangle's local space
        Vector2 localPoint = Vector2.Transform(point, inverseTransform);

        // Since the rectangle is now axis-aligned and centered at (0,0) with size (1,1),
        // we can compute the distance accordingly.
        Vector2 halfSize = new Vector2(0.5f, 0.5f);

        // Calculate the distance from the point to the rectangle edges
        Vector2 d = Vector2.Abs(localPoint) - halfSize;

        // Compute the outside distance
        float outsideDistance = Vector2.Max(d, Vector2.Zero).Length();

        // Compute the inside distance
        float insideDistance = MathF.Min(MathF.Max(d.X, d.Y), 0.0f);

        // Return the combined distance
        return new SignedDistance(outsideDistance + insideDistance,0);
    }

    /// <summary>
    /// Creates a transformation matrix 
    /// </summary>
    public Matrix3x2 CreateTransformMatrix()
    {
        // Translate to origin, apply rotation and scaling, then translate back to position
        Matrix3x2 transform =
            Matrix3x2.CreateScale(Size) *      // Scale to bounding box size
            Matrix3x2.CreateRotation(Rotation) *        // Apply rotation
            Matrix3x2.CreateTranslation(Position);        // Move to position in image

        return transform;
    }   

    /// <summary>
    /// Creates an inverse transformation matrix for this rectangle
    /// </summary>
    public Matrix3x2 CreateInverseTransformMatrix()
    {
        Matrix3x2 transform = CreateTransformMatrix();
        Matrix3x2.Invert(transform, out Matrix3x2 inverseTransform);
        return inverseTransform;
    }

    public void Serialize(ISerializer serializer)
    {
        serializer.LoadBytes(20);
        serializer.Serialize(ref Position);
        serializer.Serialize(ref Size);
        serializer.Serialize(ref Rotation);
    }
}
