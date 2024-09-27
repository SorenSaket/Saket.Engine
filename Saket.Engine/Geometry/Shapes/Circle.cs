using Saket.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Geometry.Shapes;

public struct Circle : IShape
{
    public Vector2 position;
    public float radius;
    public Circle(float x, float y, float radius)
    {
        this.position = new Vector2(x,y);
        this.radius = radius;
    }
    public Circle(Vector2 position, float radius)
    {
        this.position = position;
        this.radius = radius;
    }

    public SignedDistance GetSignedDistance(Vector2 point)
    {
        return new SignedDistance((point-position).Length()-radius, 0.5f);
    }

    public BoundingBox2D Bounds()
    {
        throw new NotImplementedException();
    }
}
