using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Graphics.Shapes
{
    public struct QuadraticBezier : ICurve2D
    {
        /// <summary>
        /// Starting point
        /// </summary>
        public Vector2 start;
        /// <summary>
        /// Control point
        /// </summary>
        public Vector2 control;
        /// <summary>
        /// End point
        /// </summary>
        public Vector2 end;

        public QuadraticBezier(Vector2 a, Vector2 b, Vector2 c)
        {
            // corrent invalid control point
            if (c == a || c == b)
                c = 0.5f * (a + b);
            this.start = a;
            this.end = b;
            this.control = c;
        }

        public Vector2 Evaluate(float t)
        {
            return Vector2.Lerp(Vector2.Lerp(start, control, t), Vector2.Lerp(control, end, t), t);
        }

        public Vector2 Direction(float t)
        {
            return Vector2.Lerp(control - start, end - control, t);
        }

        public SignedDistance SignedDistance(Vector2 origin, out float t)
        {
            // todo check validity of code
            // Figure out how it works


            Vector2 qa = start - origin;
            Vector2 ab = control - start;
            Vector2 br = start + end - control - control;
            
            int intersectionCount = Solver.SolveCubic(
                Vector2.Dot(br, br), 
                3f * Vector2.Dot(ab, br), 
                2f * Vector2.Dot(ab, ab) + Vector2.Dot(qa, br),
                Vector2.Dot(qa, ab),
                out float t1,
                out float t2,
                out float t3
                );


            float minDistance = Mathh.NonZeroSign(Extensions_Vector2.Cross(ab, qa)) * qa.Length(); // distance from A


            t = -Vector2.Dot(qa, ab) / Vector2.Dot(ab, ab);
            {
                float distance = Mathh.NonZeroSign(Extensions_Vector2.Cross(end - control, end - origin)) *
                               (end - origin).Length(); // distance from B
                if (Math.Abs(distance) < Math.Abs(minDistance))
                {
                    minDistance = distance;
                    t = Vector2.Dot(origin - control, end - control) /
                            Vector2.Dot(end - control, end - control);
                }
            }

            Span<float> solutions = stackalloc float[3] { t1, t2, t3 };
            
            for (var i = 0; i < intersectionCount; ++i)
            {
                if (solutions[i] > 0 && solutions[i] < 1f)
                {
                    Vector2 endpoint = start + 2f * solutions[i] * ab + solutions[i] * solutions[i] * br;
                    float distance = Mathh.NonZeroSign(Extensions_Vector2.Cross(end - start, endpoint - origin)) *
                                   (endpoint - origin).Length();
                    if (Math.Abs(distance) <= Math.Abs(minDistance))
                    {
                        minDistance = distance;
                        t = solutions[i];
                    }
                }
            }
               

            if (t >= 0f && t <= 1f)
                return new SignedDistance(minDistance, 0);
            if (t < 0.5f)
                return new SignedDistance(minDistance, Math.Abs(Vector2.Dot(Vector2.Normalize(ab), Vector2.Normalize(qa))));
            return new SignedDistance(minDistance,
                Math.Abs(Vector2.Dot(Vector2.Normalize(end - control), Vector2.Normalize(end - origin))));
        }

        public Bounds Bounds()
        {
            throw new NotImplementedException();
            /*
            PointBounds(a, box);
            PointBounds(b, box);
            var bot = c - a - (b - c);
            if (bot.X != 0)
            {
                var param = (c.X - a.X) / bot.X;
                if (param > 0 && param < 1)
                    PointBounds(Point(param), box);
            }

            if (bot.Y != 0)
            {
                var param = (c.Y - a.Y) / bot.Y;
                if (param > 0 && param < 1)
                    PointBounds(Point(param), box);
            }*/
        }



        
        public void SplitInThirds(out QuadraticBezier part1, out QuadraticBezier part2, out QuadraticBezier part3)
        {
            throw new NotImplementedException();
            /*
            part1 = new QuadraticBezier(a, Arithmetic.Mix(a, c, 1.0 / 3.0), Point(1.0 / 3.0));

            part2 = new QuadraticBezier(Color, Point(1.0 / 3.0),
                Arithmetic.Mix(Arithmetic.Mix(a, c, 5.0 / 9.0), Arithmetic.Mix(c, b, 4.0 / 9.0), 0.5),
                Point(2 / 3.0));
            part3 = new QuadraticBezier(Color, Point(2.0 / 3.0), Arithmetic.Mix(c, b, 2.0 / 3.0), b);
        */
        }
    }
}
