using Saket.Engine.Math.Geometry;
using System.Collections.Generic;
using System.Numerics;

namespace Saket.Engine.Typography
{
    public class Glyph : Shape
    {
        public float width;
        public float height;  


        public Glyph(IEnumerable<Spline2D> collection, int width, int height) : base(collection)
        {
            this.width= width;
            this.height= height;

        }
    }
}
