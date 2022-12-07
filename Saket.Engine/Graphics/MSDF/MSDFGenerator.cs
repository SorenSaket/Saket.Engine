using Saket.Engine.Graphics.Shapes;
using Saket.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Graphics.MSDF
{
    public class MSDFGenerator
    {
        float[] ContourSd;
        int[] Windings;
        EdgeColor[] EdgeColors;


        public MSDFGenerator() 
        {
            Windings    = Array.Empty<int>();
            ContourSd   = Array.Empty<float>();
        }


        // 1. edgecoloring
        // 2. sdf
        // 3. error correction

        public void GenerateSDF(Color[] output, int outputWidth, Shape shape, float range, Vector2 scale, Vector2 translation)
        {
         

            // resize temp data to fit
            if (Windings.Length < shape.Count)
            {
                Array.Resize(ref Windings, shape.Count);
            }
            if(ContourSd.Length  < shape.Count)
            {
                Array.Resize(ref ContourSd, shape.Count);
            }

            // Get windings
            for (int i = 0; i < shape.Count; i++)
            {
                Windings[i] = shape[i].Winding();
            }

            int height = output.Length / outputWidth;
            for (var y = 0; y < height; ++y) 
            {
                var row = shape.InverseYAxis ? height - y - 1 : y;
                for (var x = 0; x < outputWidth; ++x)
                {
                    var context = new InstanceContext
                    {
                        P = new Vector2(x + .5f, y + .5f) / scale - translation,
                        NegDist = -SignedDistance.Infinite.Distance,
                        PosDist = SignedDistance.Infinite.Distance,
                        Winding = 0
                    };

                    //output[x+y*outputWidth] = ComputePixel(ref context);
                }
            }

        }
        /*
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Color ComputePixel(ref InstanceContext ctx)
        {
            var sr = EdgePoint.Default;
            var sg = EdgePoint.Default;
            var sb = EdgePoint.Default;
            var d = Math.Abs(SignedDistance.Infinite.Distance);

            for (var i = 0; i < shape.Count; ++i)
            {
                var r = EdgePoint.Default;
                var g = EdgePoint.Default;
                var b = EdgePoint.Default;

                MsdfScanContourEdges(shape[i], ctx.P, ref r, ref g, ref b);

                if (r.MinDistance < sr.MinDistance)
                    sr = r;
                if (g.MinDistance < sg.MinDistance)
                    sg = g;
                if (b.MinDistance < sb.MinDistance)
                    sb = b;

                var medMinDistance = Math.Abs(Arithmetic.Median(r.MinDistance.Distance,
                    g.MinDistance.Distance,
                    b.MinDistance.Distance));

                if (medMinDistance < d)
                {
                    d = medMinDistance;
                    ctx.Winding = -Windings[i];
                }

                r.NearEdge?.DistanceToPseudoDistance(ref r.MinDistance, ctx.P, r.NearT);
                g.NearEdge?.DistanceToPseudoDistance(ref g.MinDistance, ctx.P, g.NearT);
                b.NearEdge?.DistanceToPseudoDistance(ref b.MinDistance, ctx.P, b.NearT);
                medMinDistance = Arithmetic.Median(r.MinDistance.Distance, g.MinDistance.Distance,
                    b.MinDistance.Distance);
                ContourSd[i].R = r.MinDistance.Distance;
                ContourSd[i].G = g.MinDistance.Distance;
                ContourSd[i].B = b.MinDistance.Distance;
                ContourSd[i].Med = medMinDistance;
                ctx.UpdateDistance(Windings[i], medMinDistance);
            }

            sr.NearEdge?.DistanceToPseudoDistance(ref sr.MinDistance, ctx.P, sr.NearT);
            sg.NearEdge?.DistanceToPseudoDistance(ref sg.MinDistance, ctx.P, sg.NearT);
            sb.NearEdge?.DistanceToPseudoDistance(ref sb.MinDistance, ctx.P, sb.NearT);

            var msd = ComputeSignedDistance(Infinite, ref ctx);

            if (Arithmetic.Median(sr.MinDistance.Distance, sg.MinDistance.Distance, sb.MinDistance.Distance) ==  msd.Med)
            {
                msd.R = sr.MinDistance.Distance;
                msd.G = sg.MinDistance.Distance;
                msd.B = sb.MinDistance.Distance;
            }

            return new FloatRgb
            {
                R = (float)(msd.R / Range + .5),
                G = (float)(msd.G / Range + .5),
                B = (float)(msd.B / Range + .5)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        float ComputeSignedDistance(float sd, ref InstanceContext context)
        {
            if (context.PosDist >= 0 && Math.Abs(context.PosDist) <= Math.Abs(context.NegDist))
            {
                sd.Dist = context.PosDist;
                context.Winding = 1;
                for (var i = 0; i < Shape.Count; ++i)
                    if (Windings[i] > 0 && ContourSd[i].Dist > sd.Dist &&
                        Math.Abs(ContourSd[i].Dist) < Math.Abs(context.NegDist))
                        sd = ContourSd[i];
            }
            else if (context.NegDist <= 0 && Math.Abs(context.NegDist) <= Math.Abs(context.PosDist))
            {
                sd.Dist = context.NegDist;
                context.Winding = -1;
                for (var i = 0; i < Shape.Count; ++i)
                    if (Windings[i] < 0 && ContourSd[i].Dist < sd.Dist &&
                        Math.Abs(ContourSd[i].Dist) < Math.Abs(context.PosDist))
                        sd = ContourSd[i];
            }

            for (var i = 0; i < Shape.Count; ++i)
                if (Windings[i] != context.Winding && Math.Abs(ContourSd[i].Dist) < Math.Abs(sd.Dist))
                    sd = ContourSd[i];
            return sd;
        }
        */

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Iterates all edges in a <paramref name="contour"/>. Gets the singed distance to the edge from <paramref name="p"/>. Sets the edgepoints info to the lowest edgde found in the contour.
        /// </remarks>
        /// <param name="contour"></param>
        /// <param name="p"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        private void MsdfScanContourEdges(Contour contour, Vector2 p, ref EdgePoint r, ref EdgePoint g, ref EdgePoint b)
        {
            for (int i = 0; i < contour.Count; i++)
            {
                SignedDistance distance = contour[i].SignedDistance(p, out float t);

                if ((EdgeColors[i] & EdgeColor.Red) != 0 && distance < r.MinDistance)
                {
                    r.MinDistance = distance;
                    r.NearEdge = i;
                    r.NearT = t;
                }

                if ((EdgeColors[i] & EdgeColor.Green) != 0 && distance < g.MinDistance)
                {
                    g.MinDistance = distance;
                    g.NearEdge = i;
                    g.NearT = t;
                }

                if ((EdgeColors[i] & EdgeColor.Blue) != 0 && distance < b.MinDistance)
                {
                    b.MinDistance = distance;
                    b.NearEdge = i;
                    b.NearT = t;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aDir"></param>
        /// <param name="bDir"></param>
        /// <param name="crossThreshold"></param>
        /// <returns></returns>
        private static bool IsCorner(Vector2 aDir, Vector2 bDir, double crossThreshold)
        {
            return
                // Dot products is less than one when the vectors are pointing away from each other
                // Dot product is 0 when perpendicular
                // return true if they're pointing in opposite directions or perpendicular
                Vector2.Dot(aDir, bDir) <= 0 ||
                // The cross product is zero when the vectors are pointing in the same or opposite direction. 
                // if they're off by more than crossThreshold return true.
                Math.Abs(Extensions_Vector2.Cross(aDir, bDir)) > crossThreshold;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="seed"></param>
        /// <param name="banned"></param>
        private static void SwitchColor(ref EdgeColor color, ref ulong seed, EdgeColor banned = EdgeColor.Black)
        {
            // combined is the banned colors that is also present in color
            EdgeColor combined = color & banned;

            if (combined == EdgeColor.Red || combined == EdgeColor.Green || combined == EdgeColor.Blue)
            {
                // Exclusive Or
                // The color gets inverted
                color = combined ^ EdgeColor.White;
                return;
            }

            // Choose random color
            if (color == EdgeColor.Black || color == EdgeColor.White)
            {
                Span<EdgeColor> start = stackalloc[] { EdgeColor.Cyan, EdgeColor.Magenta, EdgeColor.Yellow };
                color = start[(int)(seed % 3)];
                seed /= 3;
                return;
            }


            var shifted = (int)color << (int)(1 + (seed & 1));
            color = (EdgeColor)((shifted | (shifted >> 3)) & (int)EdgeColor.White);
            seed >>= 1;
        }
        private struct Simple
        {
            private readonly Shape _shape;
            private readonly float _crossThreshold;
            private readonly List<int> _corners;
            private ulong _seed;

            internal Simple(Shape shape, float angleThreshold, ulong seed = 0)
            {
                _seed = seed;
                _shape = shape;
                _corners = new List<int>();
                _crossThreshold = MathF.Sin(angleThreshold);
            }

            private void IdentifyCorners(Contour contour)
            {
                _corners.Clear();
                if (contour.Count <= 0) return;
                var prevDirection = contour[contour.Count - 1].Direction(1);
                var index = 0;
                foreach (var edge in contour)
                {
                    if (IsCorner(Vector2.Normalize( prevDirection), Vector2.Normalize(edge.Direction(0)), _crossThreshold))
                        _corners.Add(index++);
                    prevDirection = edge.Direction(1);
                }
            }

            private void ProcessSmoothContour(Contour contour)
            {/*
                foreach (var edge in contour)
                    edge.Color = EdgeColor.White;*/
            }

            internal void Process()
            {
                foreach (var contour in _shape)
                {
                    IdentifyCorners(contour);
                    switch (_corners.Count)
                    {
                        case 0:
                            ProcessSmoothContour(contour);
                            break;
                        case 1:
                            ProcessTearDropContour(contour);
                            break;
                        default:
                            ProcessMultiCornerContour(contour);
                            break;
                    }
                }
            }

            private unsafe void ProcessTearDropContour(Contour contour)
            {
                var colors = stackalloc[] { EdgeColor.White, EdgeColor.White, EdgeColor.Black };
                ProcessTearDropContourComputeColor(colors);
                if (contour.Count >= 3)
                {/*
                    var m = contour.Count;
                    for (var i = 0; i < m; ++i)
                        contour[(_corners[0] + i) % m].Color =
                            (colors + 1)[(int)Math.Floor(3 + 2.875 * i / (m - 1) - 1.4375 + .5) - 3];*/
                }
                else if (contour.Count >= 1)
                    ProcessTearDropContourSplitAndColor(contour, colors);
            }

            private unsafe void ProcessTearDropContourComputeColor(EdgeColor* colors)
            {
                SwitchColor(ref colors[0], ref _seed);
                colors[2] = colors[0];
                SwitchColor(ref colors[2], ref _seed);
            }

            private unsafe void ProcessTearDropContourSplitAndColor(Contour contour, EdgeColor* colors)
            {
                throw new NotImplementedException();
                /*
                var corner = _corners[0];
                var parts = new EdgeSegment[] { null, null, null, null, null, null, null };
                contour[0].SplitInThirds(out parts[0 + 3 * corner], out parts[1 + 3 * corner],
                    out parts[2 + 3 * corner]);
                if (contour.Count >= 2)
                {
                    contour[1].SplitInThirds(out parts[3 - 3 * corner],
                        out parts[4 - 3 * corner],
                        out parts[5 - 3 * corner]);
                    parts[0].Color = parts[1].Color = colors[0];
                    parts[2].Color = parts[3].Color = colors[1];
                    parts[4].Color = parts[5].Color = colors[2];
                }
                else
                {
                    parts[0].Color = colors[0];
                    parts[1].Color = colors[1];
                    parts[2].Color = colors[2];
                }

                contour.Clear();
                for (var i = 0; parts[i] != null; ++i)
                    contour.Add(parts[i]);*/
            }

            private void ProcessMultiCornerContour(Contour contour)
            {
                var cornerCount = _corners.Count;
                var spline = 0;
                var start = _corners[0];
                var m = contour.Count;
                var color = EdgeColor.White;
                SwitchColor(ref color, ref _seed);
                var initialColor = color;
                for (var i = 0; i < m; ++i)
                {
                    var index = (start + i) % m;
                    if (spline + 1 < cornerCount && _corners[spline + 1] == index)
                    {
                        ++spline;
                        SwitchColor(ref color, ref _seed, spline == cornerCount - 1 ? initialColor : 0);
                    }

                    //contour[index].Color = color;
                }
            }
        }

        protected struct InstanceContext
        {
            internal Vector2 P;
            internal float NegDist, PosDist;
            internal int Winding;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal void UpdateDistance(int winding, float minDistance)
            {
                if (winding > 0 && minDistance >= 0 && Math.Abs(minDistance) < Math.Abs(PosDist))
                    PosDist = minDistance;
                if (winding < 0 && minDistance <= 0 && Math.Abs(minDistance) < Math.Abs(NegDist))
                    NegDist = minDistance;
            }
        }
    }
}
