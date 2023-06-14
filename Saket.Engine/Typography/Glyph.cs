using Saket.Engine.Math.Geometry;
using System.Collections.Generic;
using System.Numerics;

namespace Saket.Engine.Typography
{
    public class Glyph
    {
        public float width;
        public float height;
        public Shape Shape;

        public Glyph(Shape shape, float width, float height)
        {
            this.Shape = shape;
            this.width = width;
            this.height= height;
        }
    }
}
