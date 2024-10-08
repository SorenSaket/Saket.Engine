using Saket.Engine.GeometryD2.Shapes;
using Saket.Engine.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Graphics.D2;

public static class Triangulate
{
    public static List<Vertex2D> TriangulateShape(in Rectangle rect, in ShapeStyle style)
    {
        List<Vertex2D> vertices = new List<Vertex2D>();

        // Calculate half size
        Vector2 halfSize = rect.Size / 2;

        // Define local corners (before transformation)
        Vector2[] corners = new Vector2[4];
        corners[0] = new Vector2(-halfSize.X, -halfSize.Y); // Bottom-left
        corners[1] = new Vector2(halfSize.X, -halfSize.Y);  // Bottom-right
        corners[2] = new Vector2(halfSize.X, halfSize.Y);   // Top-right
        corners[3] = new Vector2(-halfSize.X, halfSize.Y);  // Top-left

        // Define UVs for the corners
        Vector2[] uvs = new Vector2[4];
        uvs[0] = new Vector2(0, 1); // Bottom-left
        uvs[1] = new Vector2(1, 1); // Bottom-right
        uvs[2] = new Vector2(1, 0); // Top-right
        uvs[3] = new Vector2(0, 0); // Top-left

        // Get the transformation matrix
        Matrix3x2 transform = rect.CreateTransformMatrix();

        // Transform corners to world space
        for (int i = 0; i < 4; i++)
        {
            corners[i] = Vector2.Transform(corners[i], transform);
        }

        // Convert colors to uint (assuming RGBA format)
        uint fillColor = style.Fill;

        // Create fill triangles in clockwise order if fill color is not transparent
        if (style.Fill.A != 0)
        {
            // First triangle: Top-right, Bottom-right, Bottom-left
            vertices.Add(new Vertex2D { pos = corners[2], uv = uvs[2], col = fillColor });
            vertices.Add(new Vertex2D { pos = corners[1], uv = uvs[1], col = fillColor });
            vertices.Add(new Vertex2D { pos = corners[0], uv = uvs[0], col = fillColor });

            // Second triangle: Bottom-left, Top-left, Top-right
            vertices.Add(new Vertex2D { pos = corners[0], uv = uvs[0], col = fillColor });
            vertices.Add(new Vertex2D { pos = corners[3], uv = uvs[3], col = fillColor });
            vertices.Add(new Vertex2D { pos = corners[2], uv = uvs[2], col = fillColor });
        }

        // Handle border
        if (style.Border.A != 0 && style.borderType != BorderType.Undefined)
        {
            // Treat borderRadius as border thickness
            float borderThickness = style.borderRadius;

            // Create inner and outer rectangles based on border type
            Vector2[] outerCorners = new Vector2[4];
            Vector2[] innerCorners = new Vector2[4];
            Vector2[] outerUVs = new Vector2[4];
            Vector2[] innerUVs = new Vector2[4];

            // Calculate offsets based on border type
            Vector2 innerOffset = Vector2.Zero;
            Vector2 outerOffset = Vector2.Zero;

            switch (style.borderType)
            {
                case BorderType.Inner:
                    innerOffset = new Vector2(-borderThickness, -borderThickness);
                    break;
                case BorderType.Center:
                    innerOffset = new Vector2(-borderThickness / 2, -borderThickness / 2);
                    outerOffset = new Vector2(borderThickness / 2, borderThickness / 2);
                    break;
                case BorderType.Outer:
                    outerOffset = new Vector2(borderThickness, borderThickness);
                    break;
            }

            // Calculate inner rectangle
            Vector2 halfInnerSize = halfSize + innerOffset;
            innerCorners[0] = new Vector2(-halfInnerSize.X, -halfInnerSize.Y); // Bottom-left
            innerCorners[1] = new Vector2(halfInnerSize.X, -halfInnerSize.Y);  // Bottom-right
            innerCorners[2] = new Vector2(halfInnerSize.X, halfInnerSize.Y);   // Top-right
            innerCorners[3] = new Vector2(-halfInnerSize.X, halfInnerSize.Y);  // Top-left

            // Calculate outer rectangle
            Vector2 halfOuterSize = halfSize + outerOffset;
            outerCorners[0] = new Vector2(-halfOuterSize.X, -halfOuterSize.Y); // Bottom-left
            outerCorners[1] = new Vector2(halfOuterSize.X, -halfOuterSize.Y);  // Bottom-right
            outerCorners[2] = new Vector2(halfOuterSize.X, halfOuterSize.Y);   // Top-right
            outerCorners[3] = new Vector2(-halfOuterSize.X, halfOuterSize.Y);  // Top-left

            // Define UVs for inner and outer corners
            // For simplicity, mapping UVs from 0 to 1 over the outer rectangle
            for (int i = 0; i < 4; i++)
            {
                innerUVs[i] = MapUV(innerCorners[i], halfOuterSize);
                outerUVs[i] = MapUV(outerCorners[i], halfOuterSize);
            }

            // Transform inner and outer corners to world space
            for (int i = 0; i < 4; i++)
            {
                innerCorners[i] = Vector2.Transform(innerCorners[i], transform);
                outerCorners[i] = Vector2.Transform(outerCorners[i], transform);
            }

            // Convert border color to uint
            uint borderColor = style.Border;

            // Create border triangles (as a quad strip)
            for (int i = 0; i < 4; i++)
            {
                int next = (i + 1) % 4;

                // First triangle of the quad
                vertices.Add(new Vertex2D { pos = outerCorners[i], uv = outerUVs[i], col = borderColor });
                vertices.Add(new Vertex2D { pos = outerCorners[next], uv = outerUVs[next], col = borderColor });
                vertices.Add(new Vertex2D { pos = innerCorners[i], uv = innerUVs[i], col = borderColor });

                // Second triangle of the quad
                vertices.Add(new Vertex2D { pos = innerCorners[i], uv = innerUVs[i], col = borderColor });
                vertices.Add(new Vertex2D { pos = outerCorners[next], uv = outerUVs[next], col = borderColor });
                vertices.Add(new Vertex2D { pos = innerCorners[next], uv = innerUVs[next], col = borderColor });
            }
        }

        return vertices;
    }


    // Helper method to map UVs
    private static Vector2 MapUV(Vector2 position, Vector2 halfOuterSize)
    {
        // Map position from (-halfOuterSize, +halfOuterSize) to (0,1)
        float u = (position.X + halfOuterSize.X) / (2 * halfOuterSize.X);
        float v = 1 - ((position.Y + halfOuterSize.Y) / (2 * halfOuterSize.Y)); // Invert Y for typical UV mapping
        return new Vector2(u, v);
    }

}
