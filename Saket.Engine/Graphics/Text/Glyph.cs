using Saket.Engine.Math.Geometry;
using System.Collections.Generic;
using System.Numerics;

namespace Saket.Engine.Graphics.Text
{
    public class Glyph
    {
        public float width;
        public float height;
        public StyledShapeCollection Shape;

        public Glyph(StyledShapeCollection shape, float width, float height)
        {
            Shape = shape;
            this.width = width;
            this.height = height;
        }
    }
}
