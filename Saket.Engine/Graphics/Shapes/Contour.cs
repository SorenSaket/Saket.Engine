using System;
using System.Numerics;
using System.Collections.Generic;

namespace Saket.Engine.Graphics.Shapes
{
    /// <summary>
    /// A single continuous contour of a shape.
    /// </summary>
    public class Contour : List<QuadraticBezier>
    {
        /// Computes the bounding box of the contour.
        public void Bounds(float[] box)
        {
            //foreach (var edge in this) edge.Bounds(box);
        }

        /// Computes the winding of the contour. Returns 1 if positive, -1 if negative.
        public int Winding()
        {
            if (Count == 0)
                return 0;
            double total;
            switch (Count)
            {
                case 1:
                    total = WindingSingle();
                    break;
                case 2:
                    total = WindingDouble();
                    break;
                default:
                    total = WindingMultiple();
                    break;
            }

            return Math.Sign(total);
        }

        private float WindingMultiple()
        {
            throw new NotImplementedException();
            var total = 0.0f;
            /*var prev = this[Count - 1].Point(0);
            foreach (var edge in this)
            {
                var cur = edge.Point(0);
                total += Shoelace(prev, cur);
                prev = cur;
            }
            */
            return total;
        }

        private float WindingDouble()
            
        {
            throw new NotImplementedException();
            /*
            Vector2 a = this[0].Point(0),
                b = this[0].Point(.5f),
                c = this[1].Point(0),
                d = this[1].Point(.5f);
            return Shoelace(a, b) + Shoelace(b, c) + Shoelace(c, d) + Shoelace(d, a);*/
        }

        private float WindingSingle()
        {
            throw new NotImplementedException();
           /*Vector2 a = this[0].Point(0),
                b = this[0].Point(1.0f / 3.0f),
                c = this[0].Point(2.0f / 3.0f);
            return Shoelace(a, b) + Shoelace(b, c) + Shoelace(c, a);*/
        }

        private static float Shoelace(Vector2 a, Vector2 b)
        {
            return (b.X - a.X) * (a.Y + b.Y);
        }
    }
}