using Saket.Engine.Math;
using Saket.Engine.Math.Geometry;
using Saket.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Graphics.SDF
{

#if true
    /// <summary>
    /// Generates SDF textures from shapes.
    /// </summary>
    public class SDFGenerator
    {
        /// <summary>
        /// Describes different kinds of Signed Distance Fields
        /// </summary>
        public enum SDFType
        {
            /// <summary>Single Channel Signed Distance Fields </summary>
            SDF,
            /// <summary>3 Channel Signed Distance Field </summary>
            MDSF,
            /// <summary>4 Channel Signed Distance Field.  </summary>
            MTSDF
        }

        /// <summary>
        /// Setting for SDFGenerator
        /// </summary>
        public struct Settings
        {
            /// <summary>  </summary>
            public SDFType type = SDFType.MDSF;

            /// <summary> Whether or not to use the psudo distance </summary>
            /// <remarks> Using Psudo Distance usually yields better results</remarks>
            public bool usePsudoDistance = true;

            public bool inverseX = false;

            public bool inverseY = false;

            public float angleThreshold;

            public ulong seed = 0;
            public Settings()
            {

            }
            public Settings(SDFType type, bool usePsudoDistance)
            {
                this.type = type;
                this.usePsudoDistance = usePsudoDistance;
            }
        }

        /// <summary>
        /// A temporary variable storing each contours signed distance. 
        /// Used for intermidate calculations
        /// </summary>
        protected float[] SplineSignedDistances;

        /// <summary>
        /// A temporary variable. Windings for each spline in the shape
        /// </summary>
        protected int[] Windings;

        protected List<EdgeColor> EdgeColors;



        private List<int> corners;
        private ulong _seed;

        public SDFGenerator()
        {
            Windings = Array.Empty<int>();
            SplineSignedDistances = Array.Empty<float>();
            
        }


        // 1. Edge coloring
        //      1.a Every edge segment must belong to at least two channels
        //      1.b Two edges meeting at a corner must have only one channel in common
        //      1.c Two smoothly connected edge segments must have at least two channels in common.
        // 2. SDF
        //      2.a For each pixel calculate the sdf for each channel
        // 3. Error Correction
        //     3.a todo

        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        /// <param name="outputWidth"></param>
        /// <param name="shape"></param>
        /// <param name="range"></param>
        /// <param name="scale"></param>
        /// <param name="translation"></param>
        public void GenerateSDF(Settings settings, float[] output, int outputWidth, Shape shape, float range, Vector2 scale, Vector2 translation)
        {
            // resize temp data to fit
            if (Windings.Length < shape.Count)
            {
                Array.Resize(ref Windings, shape.Count);
            }
            if (SplineSignedDistances.Length < shape.Count)
            {
                Array.Resize(ref SplineSignedDistances, shape.Count);
            }
            // Get windings for each contour
            for (int i = 0; i < shape.Count; i++)
            {
                Windings[i] = shape[i].Winding();
            }

            // height in pixels
            int height = output.Length / outputWidth;


            if (settings.type == SDFType.SDF)
            {
                // iterate all pixels
                // Multithread here. Compute shader possibility
                for (var y = 0; y < height; ++y)
                {
                    int row = settings.inverseY ? height - y - 1 : y;
                    for (var x = 0; x < outputWidth; ++x)
                    {
                        int column = settings.inverseX ? outputWidth-x-1 : x;
                        
                        var context = new InstanceContext
                        {
                            P = new Vector2(x + .5f, y + .5f) / scale - translation,
                            NegDist = -SignedDistance.Infinite.Distance,
                            PosDist = SignedDistance.Infinite.Distance,
                            Winding = 0
                        };

                        // there must be some acceleration structure we can use here to speed up the generation
                        // ín chumskys implementation it iterates each contour and edge for each pixel.
                        // Maybe see Journal of Computer Graphics Techniques Vol. 6, No. 2, 2017
                        SignedDistance minDistance = SignedDistance.Infinite;

                        foreach (var spline in shape)
                        {
                            foreach (var curve in spline)
                            {
                                SignedDistance distance = curve.SignedDistance(context.P, out _);
                                // operator overloaded smaller than.
                                // If the absolute distance smaller than
                                if (distance < minDistance)
                                    minDistance = distance;
                            }
                        }
                        float v = minDistance.Distance / range;
                        output[column + row * outputWidth] = v;
                        /*
                        float v = ComputeSDFPixel(shape, ref context) / range + 0.5f;
                        output[column + row * outputWidth] = v;*/
                    }
                }
            }
            else
            {
                throw new NotImplementedException();

                // 1. Edge coloring
                Generate_EdgeColors(shape, EdgeColors);
            }
        }



        #region Edge Coloring
        // Edge coloring is the process of assigning each edge/curve of each spline on a shape a color.
        // The coloring should follow these rules
        // 1. Edge coloring
        //      1.a Every edge segment must belong to at least two channels
        //      1.b Two edges meeting at a corner must have only one channel in common
        //      1.c Two smoothly connected edge segments must have at least two channels in common.

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        private void Generate_EdgeColors(Shape shape, List<EdgeColor> edgecolors)
        {
            foreach (var spline in shape)
            {
                // TODO ADD threshhold
                spline.GetCorners(corners, 0);

                // A different method should be used in each case
                switch (corners.Count)
                {
                    case 0:
                        // Process Smooth Contour
                        // If the spline has no corners
                        foreach (var curve in spline)
                        {
                            edgecolors.Add(EdgeColor.White);
                        }
                        break;
                    case 1:
                        ProcessTearDropContour(spline);
                        break;
                    default:
                        ProcessMultiCornerContour(spline);
                        break;
                }
            }
        }


        private unsafe void ProcessTearDropContour(Spline2D contour)
        {
            throw new NotImplementedException();
#if false
			var colors = stackalloc[] { EdgeColor.White, EdgeColor.White, EdgeColor.Black };
			//ProcessTearDropContourComputeColor(colors);
			SwitchColor(ref colors[0], ref _seed);
			colors[2] = colors[0];
			SwitchColor(ref colors[2], ref _seed);
			if (contour.Curves >= 3)
			{/*
					var m = contour.Count;
					for (var i = 0; i < m; ++i)
						contour[(_corners[0] + i) % m].Color =
							(colors + 1)[(int)MathF.Floor(3 + 2.875 * i / (m - 1) - 1.4375 + .5) - 3];*/
			}
			else if (contour.Curves >= 1) 
			{
				//ProcessTearDropContourSplitAndColor(contour, colors);
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
					contour.Add(parts[i]);
			}
#endif
        }
        private void ProcessMultiCornerContour(Spline2D contour)
        {
            int cornerCount = corners.Count;
            var spline = 0;
            var start = corners[0];
            var m = contour.Curves;
            var color = EdgeColor.White;
            SwitchColor(ref color, ref _seed);
            var initialColor = color;
            for (var i = 0; i < m; ++i)
            {
                var index = (start + i) % m;
                if (spline + 1 < cornerCount && corners[spline + 1] == index)
                {
                    ++spline;
                    SwitchColor(ref color, ref _seed, spline == cornerCount - 1 ? initialColor : 0);
                }

                //contour[index].Color = color;
            }
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

        #endregion

        #region SDF
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected float ComputeSDFPixel(Shape shape, ref InstanceContext ctx)
        {
            int i = 0;
            foreach (var spline in shape)
            {
                // minDistance is the shortest distance to the spline
                SignedDistance minDistance = SignedDistance.Infinite;

                // Iterate all curves to find the nearest (smallest distance)
                foreach (var curve in spline)
                {
                    SignedDistance distance = curve.SignedDistance(ctx.P, out _);
                    // operator overloaded smaller than.
                    // If the absolute distance smaller than
                    if (distance < minDistance)
                        minDistance = distance;
                }

                // Update the spline min distance
                SplineSignedDistances[i] = minDistance.Distance;

                //
                ctx.UpdateDistance(Windings[i], minDistance.Distance);

                i++;
            }

            return (float)(ComputeSignedDistance(shape, float.PositiveInfinity, ref ctx));
        }

#if false
		/// <summary>
		/// Computes color of pixel in 
		/// </summary>
		/// <param name="ctx"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Color ComputePointSDFValue(Shape shape, Vector2 point)
		{
			var sr = CurvePoint.Default;
			var sg = CurvePoint.Default;
			var sb = CurvePoint.Default;
			
			// Distance to curve. Set to infinity
			float d = MathF.Abs(SignedDistance.Infinite.Distance);

			// Iterate the shapes contours
			foreach(var spline in shape)
			{
				var r = CurvePoint.Default;
				var g = CurvePoint.Default;
				var b = CurvePoint.Default;

				// Get closests
				MsdfScanContourEdges(spline, point, ref r, ref g, ref b);

				if (r.MinDistance < sr.MinDistance)
					sr = r;
				if (g.MinDistance < sg.MinDistance)
					sg = g;
				if (b.MinDistance < sb.MinDistance)
					sb = b;

				float medMinDistance = MathF.Abs(
					Mathf.Median(
						r.MinDistance.Distance,
						g.MinDistance.Distance,
						b.MinDistance.Distance)
					);

				if (medMinDistance < d)
				{
					d = medMinDistance;
					//ctx.Winding = -Windings[i];
				}

				spline[r.NearEdge].DistanceToPseudoDistance(ref r.MinDistance, point, r.NearT);
				spline[g.NearEdge].DistanceToPseudoDistance(ref g.MinDistance, point, g.NearT);
				spline[b.NearEdge].DistanceToPseudoDistance(ref b.MinDistance, point, b.NearT);


				medMinDistance = Mathf.Median(r.MinDistance.Distance, g.MinDistance.Distance,
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

			if (Mathf.Median(sr.MinDistance.Distance, sg.MinDistance.Distance, sb.MinDistance.Distance) ==  msd.Med)
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
		protected float ComputePsuedoSDFPixel(ref InstanceContext ctx)
		{
			var sd = SignedDistance.Infinite.Distance;
			for (var i = 0; i < Shape.Count; ++i)
			{
				var minDistance = SignedDistance.Infinite;
				EdgeSegment nearEdge = null;
				double nearParam = 0;
				foreach (var edge in Shape[i])
				{
					double param = 0;
					var distance = edge.SignedDistance(ctx.P, ref param);
					if (distance < minDistance)
					{
						minDistance = distance;
						nearEdge = edge;
						nearParam = param;
					}
				}

				if (MathF.Abs(minDistance.Distance) < MathF.Abs(sd))
				{
					sd = minDistance.Distance;
					ctx.Winding = -Windings[i];
				}

				nearEdge?.DistanceToPseudoDistance(ref minDistance, ctx.P, nearParam);
				ContourSd[i] = new SingleDistance { Dist = minDistance.Distance };
				ctx.UpdateDistance(Windings[i], minDistance.Distance);
			}

			return (float)(ComputeSd(Infinite, ref ctx).Dist / Range + 0.5);
		}
		
#endif

        /*
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		float ComputeSignedDistance(float sd, ref InstanceContext context)
		{
			if (context.PosDist >= 0 && MathF.Abs(context.PosDist) <= MathF.Abs(context.NegDist))
			{
				sd.Dist = context.PosDist;
				context.Winding = 1;
				for (var i = 0; i < Shape.Count; ++i)
					if (Windings[i] > 0 && ContourSd[i].Dist > sd.Dist &&
						MathF.Abs(ContourSd[i].Dist) < MathF.Abs(context.NegDist))
						sd = ContourSd[i];
			}
			else if (context.NegDist <= 0 && MathF.Abs(context.NegDist) <= MathF.Abs(context.PosDist))
			{
				sd.Dist = context.NegDist;
				context.Winding = -1;
				for (var i = 0; i < Shape.Count; ++i)
					if (Windings[i] < 0 && ContourSd[i].Dist < sd.Dist &&
						MathF.Abs(ContourSd[i].Dist) < MathF.Abs(context.PosDist))
						sd = ContourSd[i];
			}

			for (var i = 0; i < Shape.Count; ++i)
				if (Windings[i] != context.Winding && MathF.Abs(ContourSd[i].Dist) < MathF.Abs(sd.Dist))
					sd = ContourSd[i];
			return sd;
		}
		*/

        /// <summary>
        /// Gets the closests R,G,B edges to point <paramref name="p"/>.
        /// </summary>
        /// <remarks>
        /// Iterates all edges in a <paramref name="contour"/>. 
        /// Gets the singed distance to the edge from <paramref name="p"/>. 
        /// Sets the edgepoints info to the lowest edge found in the contour.
        /// </remarks>
        /// <param name="contour"></param>
        /// <param name="p"></param>
        /// <param name="r">The closes</param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        private void MsdfScanContourEdges(Spline2D contour, Vector2 p, ref SplinePoint r, ref SplinePoint g, ref SplinePoint b)
        {
            // Iterate all edges in coutour
            int index = 0;
            foreach (var curve in contour)
            {
                SignedDistance distance = curve.SignedDistance(p, out float t);

                if ((EdgeColors[index] & EdgeColor.Red) != 0 && distance < r.MinDistance)
                {
                    r.MinDistance = distance;
                    r.NearEdge = index;
                    r.NearT = t;
                }

                if ((EdgeColors[index] & EdgeColor.Green) != 0 && distance < g.MinDistance)
                {
                    g.MinDistance = distance;
                    g.NearEdge = index;
                    g.NearT = t;
                }

                if ((EdgeColors[index] & EdgeColor.Blue) != 0 && distance < b.MinDistance)
                {
                    b.MinDistance = distance;
                    b.NearEdge = index;
                    b.NearT = t;
                }
                index++;
            }
        }


        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="signedDistance"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected float ComputeSignedDistance(Shape shape, float signedDistance, ref InstanceContext context)
        {
            // This function is to add overlap support
           //

            //  if is an inside pixel
            if (context.PosDist >= 0 && MathF.Abs(context.PosDist) <= MathF.Abs(context.NegDist))
            {
                signedDistance = context.PosDist;
                context.Winding = 1;
                // Iterate all splines that are clockwise 
                // and that their signed distance is further away
                // and which are closer than the negative distance
                for (var i = 0; i < shape.Count; ++i)
                    if (Windings[i] > 0 && SplineSignedDistances[i] > signedDistance &&
                        MathF.Abs(SplineSignedDistances[i]) < MathF.Abs(context.NegDist))
                        signedDistance = SplineSignedDistances[i];
            }
            // Outside pixel
            else if (context.NegDist <= 0 && MathF.Abs(context.NegDist) <= MathF.Abs(context.PosDist))
            {
                signedDistance = context.NegDist;
                context.Winding = -1;
                for (var i = 0; i < shape.Count; ++i)
                    if (Windings[i] < 0 && SplineSignedDistances[i] < signedDistance &&
                        MathF.Abs(SplineSignedDistances[i]) < MathF.Abs(context.PosDist))
                        signedDistance = SplineSignedDistances[i];
            }



                for (var i = 0; i < shape.Count; ++i)
                {
                    if (Windings[i] != context.Winding &&
                        MathF.Abs(SplineSignedDistances[i]) < MathF.Abs(signedDistance))
                    {
                        signedDistance = SplineSignedDistances[i];
                    }
                }

                return signedDistance;
            
        }




        /// <summary>
        /// Context in which calculations are carried out
        /// </summary>
        protected ref struct InstanceContext
        {
            /// <summary> </summary>
            internal Vector2 P;

            /// <summary> </summary>
            internal float NegDist;

            /// <summary> </summary>
            internal float PosDist;

            /// <summary> </summary>
            internal int Winding;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal void UpdateDistance(int winding, float minDistance)
            {
                // if the clockwise/filled area
                if (winding > 0 && minDistance >= 0 && MathF.Abs(minDistance) < MathF.Abs(PosDist))
                    PosDist = minDistance;
                // if counter clockwise/ empty area
                if (winding < 0 && minDistance <= 0 && MathF.Abs(minDistance) < MathF.Abs(NegDist))
                    NegDist = minDistance;
            }
        }
    }
#endif

}