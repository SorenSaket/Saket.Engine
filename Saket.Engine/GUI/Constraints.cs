using System.Numerics;

namespace Saket.Engine.GUI; 

public struct Constraints
{
    public float MinWidth;
    public float MaxWidth;
    public float MinHeight;
    public float MaxHeight;

    public Vector2 Min { get { return new(MinWidth, MinHeight); } set { MinWidth = value.X; MinHeight = value.Y; } }
    public Vector2 Max { get { return new(MaxWidth, MaxHeight); } set { MaxWidth = value.X; MaxHeight = value.Y; } }

    public Constraints(float minWidth, float maxWidth, float minHeight, float maxHeight)
    {
        this.MinWidth = minWidth;
        this.MaxWidth = maxWidth;
        this.MinHeight = minHeight;
        this.MaxHeight = maxHeight;
    }

    public Constraints(Vector2 min, Vector2 max)
    {
        this.MinWidth = min.X;
        this.MaxWidth = max.X;
        this.MinHeight = min.Y;
        this.MaxHeight = max.Y;
    }
}
