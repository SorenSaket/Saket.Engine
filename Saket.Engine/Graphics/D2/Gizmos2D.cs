using Saket.Engine.GeometryD2.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Graphics.D2;

public static class Gizmos2D
{
    public static List<Vertex2D> CreateHandle(Rectangle rect, float border)
    { 
        return Triangulate.TriangulateShape(
            rect,
            new Engine.Vector.ShapeStyle(Color.White, Color.Gray, Engine.Vector.BorderType.Center, border));
    }


        public static List<Vertex2D> GenerateQuads(IEnumerable<Vector2> points, Vector2 halfSize, params ReadOnlySpan<Color> color)
    {
        List<Vertex2D> vertices = [];
        int c = 0;
        foreach (var point in points)
        {
         
            vertices.AddRange(Quad(point, halfSize, color[c%color.Length] ));
            c++;
        }
        return vertices;
    }


    public static Vertex2D[] Quad(Vector2 position, Vector2 halfSize, Color color)
    {
        Vertex2D[] vertices = [
            new Vertex2D(new Vector2(position.X-halfSize.X, position.Y-halfSize.Y), new Vector2(0,0), color), // BL
            new Vertex2D(new Vector2(position.X-halfSize.X, position.Y+halfSize.Y), new Vector2(0,1), color), // TL
            new Vertex2D(new Vector2(position.X+halfSize.X, position.Y-halfSize.Y), new Vector2(1,0), color), // BR
            new Vertex2D(new Vector2(position.X-halfSize.X, position.Y+halfSize.Y), new Vector2(0,1), color), // TL
            new Vertex2D(new Vector2(position.X+halfSize.X, position.Y+halfSize.Y), new Vector2(1,1), color), // TR
            new Vertex2D(new Vector2(position.X+halfSize.X, position.Y-halfSize.Y), new Vector2(1,0), color), // BR
        ];
        return vertices;
    }

    public static Vertex2D[] QuadMinMax(Vector2 min, Vector2 max, Color color)
    {
        Vertex2D[] vertices = [
            new Vertex2D(new Vector2(min.X, min.Y), new Vector2(0,0), color), // BL
            new Vertex2D(new Vector2(min.X, max.Y), new Vector2(0,1), color), // TL
            new Vertex2D(new Vector2(max.X, min.Y), new Vector2(1,0), color), // BR
            new Vertex2D(new Vector2(min.X, max.Y), new Vector2(0,1), color), // TL
            new Vertex2D(new Vector2(max.X, max.Y), new Vector2(1,1), color), // TR
            new Vertex2D(new Vector2(max.X, min.Y), new Vector2(1,0), color), // BR
        ];
        return vertices;
    }
    public static Vertex2D[] RectHollow(Rectangle rect, float lineThickness, Color color)
    {


        // Ensure lineThickness does not exceed half the size of the rectangle
        float halfWidth = rect.Size.X / 2f;
        float halfHeight = rect.Size.Y / 2f;
        float maxThickness = MathF.Min(halfWidth, halfHeight);
        if (lineThickness > maxThickness)
            lineThickness = maxThickness;

        // Compute half sizes for outer and inner rectangles
        Vector2 halfSizeOuter = new Vector2(halfWidth, halfHeight);
        Vector2 halfSizeInner = new Vector2(halfWidth - lineThickness, halfHeight - lineThickness);

        // Ensure inner sizes are not negative
        if (halfSizeInner.X < 0f) halfSizeInner.X = 0f;
        if (halfSizeInner.Y < 0f) halfSizeInner.Y = 0f;

        // Define the outer rectangle corners (local coordinates)
        Vector2[] outerCorners = new Vector2[]
        {
            new Vector2(-halfSizeOuter.X, -halfSizeOuter.Y), // p0: Top Left
            new Vector2(halfSizeOuter.X, -halfSizeOuter.Y),  // p1: Top Right
            new Vector2(halfSizeOuter.X, halfSizeOuter.Y),   // p2: Bottom Right
            new Vector2(-halfSizeOuter.X, halfSizeOuter.Y),  // p3: Bottom Left
        };

        // Define the inner rectangle corners (local coordinates)
        Vector2[] innerCorners = new Vector2[]
        {
            new Vector2(-halfSizeInner.X, -halfSizeInner.Y), // p4: Inner Top Left
            new Vector2(halfSizeInner.X, -halfSizeInner.Y),  // p5: Inner Top Right
            new Vector2(halfSizeInner.X, halfSizeInner.Y),   // p6: Inner Bottom Right
            new Vector2(-halfSizeInner.X, halfSizeInner.Y),  // p7: Inner Bottom Left
        };

        // Apply rotation and translation to all corners
        float cosTheta = MathF.Cos(rect.Rotation);
        float sinTheta = MathF.Sin(rect.Rotation);

        // Function to rotate and translate a point
        Vector2 Transform(Vector2 point)
        {
            // Rotate
            float xNew = point.X * cosTheta - point.Y * sinTheta;
            float yNew = point.X * sinTheta + point.Y * cosTheta;

            // Translate
            return new Vector2(xNew + rect.Position.X, yNew + rect.Position.Y);
        }

        // Transform all corners
        for (int i = 0; i < 4; i++)
        {
            outerCorners[i] = Transform(outerCorners[i]);
            innerCorners[i] = Transform(innerCorners[i]);
        }

        // Compute UVs (assuming UVs range from 0 to 1 across the rectangle before rotation)
        // Since rotation complicates UV mapping, we'll map UVs based on the unrotated positions
        // relative to the rectangle center

        // For UV mapping, use the local coordinates before rotation and map them from 0 to 1
        Vector2 uvMin = new Vector2(-halfWidth, -halfHeight);
        Vector2 uvMax = new Vector2(halfWidth, halfHeight);

        Vector2 ComputeUV(Vector2 localPoint)
        {
            float u = (localPoint.X - uvMin.X) / (uvMax.X - uvMin.X);
            float v = (localPoint.Y - uvMin.Y) / (uvMax.Y - uvMin.Y);
            return new Vector2(u, v);
        }

        // Collect all points and UVs
        Vector2[] points = new Vector2[8];
        Vector2[] uvs = new Vector2[8];

        for (int i = 0; i < 4; i++)
        {
            // Store transformed outer corners
            points[i] = outerCorners[i];

            // UVs based on local coordinates before rotation
            uvs[i] = ComputeUV(outerCorners[i] - rect.Position);
        }

        for (int i = 0; i < 4; i++)
        {
            // Store transformed inner corners
            points[i + 4] = innerCorners[i];

            // UVs based on local coordinates before rotation
            uvs[i + 4] = ComputeUV(innerCorners[i] - rect.Position);
        }

        // Define triangles with indices into the points array, ensuring clockwise winding order
        int[][] triangles = new int[][]
        {
            // Top Side
            new int[] { 0, 4, 1 }, // Triangle 1
            new int[] { 1, 4, 5 }, // Triangle 2

            // Right Side
            new int[] { 1, 5, 2 }, // Triangle 3
            new int[] { 2, 5, 6 }, // Triangle 4

            // Bottom Side
            new int[] { 2, 6, 3 }, // Triangle 5
            new int[] { 3, 6, 7 }, // Triangle 6

            // Left Side
            new int[] { 3, 7, 0 }, // Triangle 7
            new int[] { 0, 7, 4 }, // Triangle 8
        };

        // Create the vertex list
        List<Vertex2D> vertexList = new List<Vertex2D>();

        foreach (var tri in triangles)
        {
            // For each triangle, add the vertices in the correct order
            for (int i = 0; i < 3; i++)
            {
                int idx = tri[i];
                vertexList.Add(new Vertex2D
                {
                    pos = points[idx],
                    uv = uvs[idx],
                    col = color
                });
            }
        }

        return vertexList.ToArray();
    }

    public static Vertex2D[] GenerateHollowSquare(Vector2 min, Vector2 max, float lineThickness, Color color)
    {
        float width = max.X - min.X;
        float height = max.Y - min.Y;

        // Ensure lineThickness is positive and does not exceed half of the min dimension
        float maxThickness = MathF.Min(width, height) / 2f;
        if (lineThickness > maxThickness)
            lineThickness = maxThickness;

        // Define the outer and inner rectangle corners
        // Outer corners (p0 to p3)
        Vector2 p0 = new Vector2(min.X, min.Y); // Outer Top Left
        Vector2 p1 = new Vector2(max.X, min.Y); // Outer Top Right
        Vector2 p2 = new Vector2(max.X, max.Y); // Outer Bottom Right
        Vector2 p3 = new Vector2(min.X, max.Y); // Outer Bottom Left

        // Inner corners (p4 to p7)
        Vector2 p4 = new Vector2(min.X + lineThickness, min.Y + lineThickness); // Inner Top Left
        Vector2 p5 = new Vector2(max.X - lineThickness, min.Y + lineThickness); // Inner Top Right
        Vector2 p6 = new Vector2(max.X - lineThickness, max.Y - lineThickness); // Inner Bottom Right
        Vector2 p7 = new Vector2(min.X + lineThickness, max.Y - lineThickness); // Inner Bottom Left

        // Collect all unique vertices
        Vector2[] points = new Vector2[] { p0, p1, p2, p3, p4, p5, p6, p7 };

        // Compute UVs
        Vector2[] uvs = new Vector2[8];
        for (int i = 0; i < 8; i++)
        {
            float u = (points[i].X - min.X) / width;
            float v = (points[i].Y - min.Y) / height;
            uvs[i] = new Vector2(u, v);
        }

        // Define triangles with indices into the points array, ensuring clockwise winding order
        int[][] triangles = new int[][]
        {
            // Top Side
            new int[] { 0, 4, 1 }, // Triangle 1
            new int[] { 1, 4, 5 }, // Triangle 2

            // Right Side
            new int[] { 1, 5, 2 }, // Triangle 3
            new int[] { 2, 5, 6 }, // Triangle 4

            // Bottom Side
            new int[] { 2, 6, 3 }, // Triangle 5
            new int[] { 3, 6, 7 }, // Triangle 6

            // Left Side
            new int[] { 3, 7, 0 }, // Triangle 7
            new int[] { 0, 7, 4 }, // Triangle 8
        };

        // Create the vertex list
        List<Vertex2D> vertexList = new List<Vertex2D>();

        foreach (var tri in triangles)
        {
            // For each triangle, add the vertices in the correct order
            for (int i = 0; i < 3; i++)
            {
                int idx = tri[i];
                vertexList.Add(new Vertex2D
                {
                    pos = points[idx],
                    uv = uvs[idx],
                    col = color
                });
            }
        }

        return vertexList.ToArray();
    }


    public static List<Vertex2D> GenerateLineMesh(
        List<Vector2> points,
        float lineWidth,
        bool roundCorners,
        int cornerSections)
    {
        if (points == null || points.Count < 2)
        {
            return []; // Need at least two points
        }

        List<Vertex2D> vertexList = new List<Vertex2D>();
        float halfWidth = lineWidth / 2f;

        // Store the left and right positions at each point
        List<Vector2> leftPositions = new List<Vector2>();
        List<Vector2> rightPositions = new List<Vector2>();

        // Process each point to calculate offsets
        for (int i = 0; i < points.Count; i++)
        {
            Vector2 p = points[i];
            Vector2 dirPrev, dirNext;

            if (i == 0)
            {
                dirNext = Vector2.Normalize(points[i + 1] - p);
                dirPrev = dirNext;
            }
            else if (i == points.Count - 1)
            {
                dirPrev = Vector2.Normalize(p - points[i - 1]);
                dirNext = dirPrev;
            }
            else
            {
                dirPrev = Vector2.Normalize(p - points[i - 1]);
                dirNext = Vector2.Normalize(points[i + 1] - p);
            }

            Vector2 normalPrev = new Vector2(-dirPrev.Y, dirPrev.X);
            Vector2 normalNext = new Vector2(-dirNext.Y, dirNext.X);

            if (roundCorners && i > 0 && i < points.Count - 1)
            {
                // Compute angle between directions
                float dot = Vector2.Dot(dirPrev, dirNext);
                dot = Math.Clamp(dot, -1.0f, 1.0f); // Ensure dot is in valid range
                float angle = (float)Math.Acos(dot);

                if (Math.Abs(angle) > 0.01f) // If angle is significant
                {
                    // Generate rounded corner
                    int segments = Math.Max(1, cornerSections);
                    for (int j = 0; j <= segments; j++)
                    {
                        float t = (float)j / segments;
                        // Spherical linear interpolation between normals
                        Vector2 normal = Slerp(normalPrev, normalNext, t);
                        Vector2 offsetA = normal * halfWidth;
                        leftPositions.Add(p + offsetA);
                        rightPositions.Add(p - offsetA);
                    }
                    continue; // Skip adding the current point's offsets again
                }
            }

            // Calculate miter vector for the corner
            Vector2 miter = Vector2.Normalize(normalPrev + normalNext);
            float miterLength = halfWidth / Vector2.Dot(miter, normalPrev);

            // Limit the miter length to avoid spikes at sharp angles
            float maxMiterLength = 4 * halfWidth; // Adjust as needed
            if (miterLength > maxMiterLength)
            {
                miterLength = maxMiterLength;
            }

            Vector2 offset = miter * miterLength;

            leftPositions.Add(p + offset);
            rightPositions.Add(p - offset);
        }

        // Generate the triangle list
        int index = 0;
        for (int i = 0; i < leftPositions.Count - 1; i++)
        {
            // First triangle
            vertexList.Add(new Vertex2D { pos = leftPositions[i], uv = new Vector2(0, 0), col = 0xffffffff });
            vertexList.Add(new Vertex2D { pos = rightPositions[i], uv = new Vector2(0, 1), col = 0xffffffff });
            vertexList.Add(new Vertex2D { pos = leftPositions[i + 1], uv = new Vector2(1, 0), col = 0xffffffff });

            // Second triangle
            vertexList.Add(new Vertex2D { pos = leftPositions[i + 1], uv = new Vector2(1, 0), col = 0xffffffff });
            vertexList.Add(new Vertex2D { pos = rightPositions[i], uv = new Vector2(0, 1), col = 0xffffffff });
            vertexList.Add(new Vertex2D { pos = rightPositions[i + 1], uv = new Vector2(1, 1), col = 0xffffffff });

            index += 6;
        }

        return vertexList;
    }


    public static void GenerateLineMesh(
        List<Vector2> points,
        float lineWidth,
        bool roundCorners,
        int cornerSections,
        out Vertex2D[] vertices,
        out int[] indices)
    {
        List<Vertex2D> vertexList = new List<Vertex2D>();
        List<int> indexList = new List<int>();

        if (points == null || points.Count < 2)
        {
            vertices = vertexList.ToArray();
            indices = indexList.ToArray();
            return;
        }

        float halfWidth = lineWidth / 2f;
        float totalLength = 0f;
        float[] segmentLengths = new float[points.Count - 1];

        // Calculate segment lengths and total length
        for (int i = 0; i < points.Count - 1; i++)
        {
            float segmentLength = (points[i + 1] - points[i]).Length();
            segmentLengths[i] = segmentLength;
            totalLength += segmentLength;
        }

        float cumulativeLength = 0f;

        for (int i = 0; i < points.Count; i++)
        {
            Vector2 p = points[i];

            Vector2 dirPrev = Vector2.Zero;
            Vector2 dirNext = Vector2.Zero;

            if (i == 0)
            {
                // Start point
                dirNext = Vector2.Normalize(points[i + 1] - p);
                dirPrev = dirNext;
            }
            else if (i == points.Count - 1)
            {
                // End point
                dirPrev = Vector2.Normalize(p - points[i - 1]);
                dirNext = dirPrev;
            }
            else
            {
                dirPrev = Vector2.Normalize(p - points[i - 1]);
                dirNext = Vector2.Normalize(points[i + 1] - p);
            }

            Vector2 normalPrev = new Vector2(-dirPrev.Y, dirPrev.X);
            Vector2 normalNext = new Vector2(-dirNext.Y, dirNext.X);

            Vector2 miter = Vector2.Normalize(normalPrev + normalNext);
            float miterLength = halfWidth / Vector2.Dot(miter, normalPrev);

            // Limit the miter length to avoid spikes at sharp angles
            float maxMiterLength = halfWidth * 4f;
            if (miterLength > maxMiterLength)
                miterLength = maxMiterLength;

            Vector2 offset = miter * miterLength;

            // UV coordinate along the length
            float u = cumulativeLength / totalLength;

            if (roundCorners && i > 0 && i < points.Count - 1)
            {
                // Calculate angle between segments
                float angle = (float)System.Math.Acos(Vector2.Dot(dirPrev, dirNext));
                if (float.IsNaN(angle) || angle == 0f)
                {
                    // Angle is zero, treat as straight line
                    GenerateSegment(vertexList, indexList, p, offset, u);
                }
                else
                {
                    // Generate rounded corner
                    int segments = System.Math.Max(1, cornerSections);
                    for (int j = 0; j <= segments; j++)
                    {
                        float t = (float)j / segments;
                        Vector2 dir = Slerp(dirPrev, dirNext, t);
                        Vector2 normal = new Vector2(-dir.Y, dir.X);
                        Vector2 offsetCorner = normal * halfWidth;

                        Vector2 leftPos = p + offsetCorner;
                        Vector2 rightPos = p - offsetCorner;

                        Vertex2D vLeft = new Vertex2D { pos = leftPos, uv = new Vector2(u, 0), col = 0xffffffff };
                        Vertex2D vRight = new Vertex2D { pos = rightPos, uv = new Vector2(u, 1), col = 0xffffffff };

                        vertexList.Add(vLeft);
                        vertexList.Add(vRight);

                        int idx = vertexList.Count - 2;

                        if (j > 0)
                        {
                            // Create indices
                            indexList.Add(idx - 2);
                            indexList.Add(idx);
                            indexList.Add(idx - 1);

                            indexList.Add(idx);
                            indexList.Add(idx + 1);
                            indexList.Add(idx - 1);
                        }
                    }
                }
            }
            else
            {
                // No corner rounding
                GenerateSegment(vertexList, indexList, p, offset, u);
            }

            if (i < points.Count - 1)
            {
                cumulativeLength += segmentLengths[i];
            }
        }

        vertices = vertexList.ToArray();
        indices = indexList.ToArray();
    }

    // Helper function to generate a line segment
    private static void GenerateSegment(List<Vertex2D> vertexList, List<int> indexList, Vector2 p, Vector2 offset, float u)
    {
        Vector2 leftPos = p + offset;
        Vector2 rightPos = p - offset;

        Vertex2D vLeft = new Vertex2D { pos = leftPos, uv = new Vector2(u, 0), col = 0xffffffff };
        Vertex2D vRight = new Vertex2D { pos = rightPos, uv = new Vector2(u, 1), col = 0xffffffff };

        vertexList.Add(vLeft);
        vertexList.Add(vRight);

        int idx = vertexList.Count - 2;

        if (vertexList.Count > 2)
        {
            // Create indices
            indexList.Add(idx - 2);
            indexList.Add(idx);
            indexList.Add(idx - 1);

            indexList.Add(idx);
            indexList.Add(idx + 1);
            indexList.Add(idx - 1);
        }
    }

    // Helper function to perform spherical linear interpolation between two vectors
    private static Vector2 Slerp(Vector2 from, Vector2 to, float t)
    {
        float dot = Vector2.Dot(from, to);
        dot = System.Math.Clamp(dot, -1.0f, 1.0f);
        float theta = (float)System.Math.Acos(dot) * t;
        Vector2 relativeVec = Vector2.Normalize(to - from * dot);
        return from * (float)System.Math.Cos(theta) + relativeVec * (float)System.Math.Sin(theta);
    }
    public static List<Vertex2D> GenerateLineMesh(List<Vector2> points, float lineWidth, float cornerRadius, int cornerSegments)
    {
        List<Vertex2D> vertices = [];

        if (points == null || points.Count < 2)
        {
            throw new ArgumentException("Need at least two points to form a line");
        }

        if(lineWidth <= 0)
            throw new ArgumentException("Linewidth must be positive");

        float halfWidth = lineWidth * 0.5f;

        // For each point on the line
        for (int i = 0; i < points.Count - 1; i++)
        {
            // From - to
            Vector2 p0 = points[i];
            Vector2 p1 = points[i + 1];

            // Direction
            Vector2 dir = Vector2.Normalize(p1 - p0);
            // The perpendicular 
            Vector2 perp = new Vector2(-dir.Y, dir.X) * halfWidth;

            // Add vertices for the current segment
            vertices.Add(new Vertex2D(p0 + perp, new Vector2(0, 0)));
            vertices.Add(new Vertex2D(p0 - perp, new Vector2(0, 1)));
            vertices.Add(new Vertex2D(p1 + perp, new Vector2(1, 0)));
            vertices.Add(new Vertex2D(p1 - perp, new Vector2(1, 1)));

            // If not the first or last segment, handle corner rounding
            if (i < points.Count - 2 && cornerRadius > 0)
            {
                Vector2 p2 = points[i + 2];
                Vector2 dirNext = Vector2.Normalize(p2 - p1);
                Vector2 perpNext = new Vector2(-dirNext.Y, dirNext.X) * halfWidth;

                // Calculate the angle between the segments
                float angle = SignedAngle(dir, dirNext);

                if (MathF.Abs(angle) > 1e-2) // Avoid processing straight lines
                {
                    // Generate a rounded corner using segments
                    int segments = (int)MathF.Max(2, cornerSegments); // At least 2 segments
                    float step = angle / (segments - 1);

                    for (int j = 1; j < segments - 1; j++)
                    {
                        float segmentAngle = j * step;
                        Vector2 roundedDir = Vector2.Normalize(RotateVector(dir, segmentAngle));
                        Vector2 cornerPerp = new Vector2(-roundedDir.Y, roundedDir.X) * halfWidth;

                        // Calculate UV for corner rounding
                        float uvX = InverseLerp(0, segments - 1, j);

                        vertices.Add(new Vertex2D(p1 + cornerPerp, new Vector2(uvX, 0)));
                        vertices.Add(new Vertex2D(p1 - cornerPerp, new Vector2(uvX, 1)));
                    }
                }
            }
        }

        return vertices;
    }

    public static float InverseLerp(float a, float b, float value)
    {
        if (a != b) // Avoid division by zero
        {
            return Mathf.Clamp01((value - a) / (b - a));
        }
        else
        {
            return 0f; // If a and b are the same, we can't interpolate, so return 0
        }
    }

    public static float SignedAngle(Vector2 from, Vector2 to)
    {
        // Normalize the vectors
        from = Vector2.Normalize(from);
        to =    Vector2.Normalize(to);

        // Calculate the angle between the two vectors
        float angle = MathF.Acos(Vector2.Dot(from, to)) * Mathf.RadToDeg;

        // Calculate the sign of the angle
        float crossProduct = from.X * to.Y - from.Y * to.X;
        return (crossProduct < 0) ? -angle : angle;
    }


    // Helper function to rotate a vector by a given angle in degrees
    private static Vector2 RotateVector(Vector2 v, float angleDeg)
    {
        float rad = angleDeg * Mathf.DegToRad;
        float cos = MathF.Cos(rad);
        float sin = MathF.Sin(rad);
        return new Vector2(v.X * cos - v.Y * sin, v.X * sin + v.Y * cos);
    }
}