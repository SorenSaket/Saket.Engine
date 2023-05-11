using System;
using System.Collections.Generic;

namespace Saket.Engine.Math.Geometry
{
    /// <summary>
	/// Vector shape representation.
	/// </summary>
    public class Shape : List<Spline2D>
    {
        ///  <summary> Specifies whether the shape uses bottom-to-top (false) or top-to-bottom (true) Y coordinates. </summary>
        public bool InverseYAxis = false;

        public Shape(IEnumerable<Spline2D> collection) : base(collection)
        {
        }

        /// <summary>
        /// Normalizes the shape geometry for distance field generation.
        /// If a contour only consists of one spline split in into thirds.
        /// </summary>
        public void Normalize()
        {
            throw new NotImplementedException();
            /*
            foreach (var contour in this)
            {
                if (contour.Count == 1)
                {
                    var parts = new EdgeSegment[3];
                    contour[0].SplitInThirds(out parts[0], out parts[1], out parts[2]);
                    contour.Clear();
                    contour.Add(parts[0]);
                    contour.Add(parts[1]);
                    contour.Add(parts[2]);
                }
            }*/
        }

        /// Performs basic checks to determine if the object represents a valid shape.
        public bool Validate()
        {
            throw new NotImplementedException();
            /*
            foreach (var contour in this)
                if (contour.Count > 0)
                {
                    var corner = contour[contour.Count - 1].Point(1);
                    foreach (var edge in contour)
                    {
                        if (edge == null)
                            return false;
                        if (edge.Point(0) != corner)
                            return false;
                        corner = edge.Point(1);
                    }
                }

            return true;*/
        }

        /// <summary>
        /// Computes the shape's bounding box.
        /// </summary>
        /// <param name="box"></param>
        public void Bounds(float[] box)
        {
            throw new NotImplementedException();
            /*
            foreach (var contour in this)
                contour.Bounds(box);*/
        }
    }
}