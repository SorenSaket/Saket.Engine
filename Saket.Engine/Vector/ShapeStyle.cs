using Saket.Engine.Graphics;

namespace Saket.Engine.Vector;


public enum BorderType
{
    Undefined,
    Inner,
    Center,
    Outer
}

public struct ShapeStyle
{
    public Color Fill;
    public Color Border;
    public BorderType borderType;
    public float borderRadius;

    public ShapeStyle()
    {
        Fill = Color.White;
        Border = Color.White;
        borderType = BorderType.Center;
        borderRadius = 0f;
    }

    public ShapeStyle(Color fill, Color border, BorderType borderType = BorderType.Center, float borderRadius = 0)
    {
        Fill = fill;
        Border = border;
        this.borderType = borderType;
        this.borderRadius = borderRadius;
    }
}
