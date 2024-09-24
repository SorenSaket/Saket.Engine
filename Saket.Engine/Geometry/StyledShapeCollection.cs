
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Saket.Engine.Geometry;

/// <summary>
	/// Vector shape representation.
	/// </summary>
public class StyledShapeCollection : List<IShape>, IShape
{
    public List<IShape> Shapes => this;
    public List<ShapeStyle> Styles;
    public StyledShapeCollection(List<IShape> shapes, List<ShapeStyle> styles) : base(shapes)
    {
        Styles = styles;
    }
    public StyledShapeCollection() : base()
    {
        Styles = new List<ShapeStyle>();
    }
    public StyledShapeCollection(List<IShape> collection) : base(collection)
    {
        Styles = new List<ShapeStyle>(collection.Count);
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
    public Vector4 Bounds()
    {
        throw new NotImplementedException();
        /*
        foreach (var contour in this)
            contour.Bounds(box);*/
    }

    public SignedDistance GetSignedDistance(System.Numerics.Vector2 point)
    {
        return SignedDistance.GetShortest(Shapes, point);
    }

    BoundingBox2D IBounds2D.Bounds()
    {
        throw new NotImplementedException();
    }
}