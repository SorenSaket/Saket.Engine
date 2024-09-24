using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Graphics._2D;

public static class LineRenderer
{
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