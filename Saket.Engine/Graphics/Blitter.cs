using Saket.Engine.GeometryD2.Shapes;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Saket.Engine.Graphics;

public class Blitter
{

    public struct BlitOp
    {
        public  byte[] sourceData;
        public  int sourceWidth;
        public  int sourceHeight;
        public  Rectangle sourceRect;

        public  byte[] targetData;
        public  int targetWidth;
        public  int targetHeight;
        public Rectangle targetRect;

        public List<int>? includedSourceIndicies = null;
        public int bytesPerPixel = 4;
        public BlendMode blendMode = BlendMode.Normal;
        public Func<SampleOp, Color> Sampler = Sample_NearestNeighbor;

        public BlitOp(){}
        public BlitOp(byte[] sourceData, int sourceWidth, int sourceHeight, Rectangle sourceRect,
            byte[] targetData, int targetWidth, int targetHeight, Rectangle targetRect)
        {
            this.sourceData = sourceData;
            this.sourceWidth = sourceWidth;
            this.sourceHeight = sourceHeight;
            this.sourceRect = sourceRect;

            this.targetData = targetData;
            this.targetWidth = targetWidth;
            this.targetHeight = targetHeight;
            this.targetRect = targetRect;
        }
        public BlitOp(ImageTexture source, ImageTexture target)
        {
            sourceData = source.Data;
            sourceWidth = source.Width;
            sourceHeight = source.Height;
            sourceRect = new Rectangle(new Vector2(sourceWidth, sourceHeight) / 2f, new Vector2(sourceWidth, sourceHeight));

            targetData = target.Data;
            targetWidth = target.Width;
            targetHeight = target.Height;
            targetRect = new Rectangle(new Vector2(targetWidth, targetHeight) / 2f, new Vector2(targetWidth, targetHeight));
        }
    }

    public static void Blit(BlitOp op) // Assuming RGBA format by default
    {
        // Create transformation matrices
        Matrix3x2 sourceTransform = op.sourceRect.CreateTransformMatrix();
        Matrix3x2 targetInverseTransform = op.targetRect.CreateInverseTransformMatrix();

        var bounds_target = op.targetRect.GetBounds();

        // Clamp to target image bo unds
        int startX = Math.Max((int)Math.Floor(bounds_target.Min.X), 0);
        int endX = Math.Min((int)Math.Ceiling(bounds_target.Max.X), op.targetWidth - 1);
        int startY = Math.Max((int)Math.Floor(bounds_target.Min.Y), 0);
        int endY = Math.Min((int)Math.Ceiling(bounds_target.Max.Y), op.targetHeight - 1);

        // Iterate over the pixels within the bounding box
        for (int y_t = startY; y_t <= endY; y_t++)
        {
            for (int x_t = startX; x_t <= endX; x_t++)
            {
                Vector2 targetPixel = new Vector2(x_t, y_t);

                // Transform the pixel to the rectangle's local space
                Vector2 localPos = Vector2.Transform(targetPixel, targetInverseTransform);

                // Check if the local position is within the rectangle
                if (Math.Abs(localPos.X) <= 1 && Math.Abs(localPos.Y) <= 1)
                {
                    // Map local position from target to source space
                    // First, map from [-1, 1] to source rectangle's local space
                    Vector2 sourceLocalPos = localPos;

                    // Transform local position to source pixel position
                    Vector2 sourcePixel = Vector2.Transform(sourceLocalPos, sourceTransform);

                    // Prepare the SampleOp struct
                    SampleOp sampleOp = new SampleOp
                    {
                        targetX = sourcePixel.X,
                        targetY = sourcePixel.Y,
                        sourceData = op.sourceData,
                        sourceWidth = op.sourceWidth,
                        sourceHeight = op.sourceHeight,
                    };

                    // Get the integer rounded source pixel positions for boundary checking
                    int x_s_int = (int)Math.Round(sourcePixel.X);
                    int y_s_int = (int)Math.Round(sourcePixel.Y);

                    // Check bounds in the source image
                    if (x_s_int >= 0 && x_s_int < op.sourceWidth && y_s_int >= 0 && y_s_int < op.sourceHeight)
                    {
                        int sourceIndex = (y_s_int * op.sourceWidth + x_s_int) * op.bytesPerPixel;

                        // If we are excluding a pixel or is alpha = 0
                        if (op.includedSourceIndicies != null && !op.includedSourceIndicies.Contains((y_s_int * op.sourceWidth + x_s_int)))
                            continue;

                        int targetIndex = (y_t * op.targetWidth + x_t) * op.bytesPerPixel;


                        // Sample the pixel using the provided Sampler function
                        Color sampledColor = op.Sampler(sampleOp);
                        // Copy sampled color to target image
                        op.targetData[targetIndex + 0] = sampledColor.R; // Red
                        op.targetData[targetIndex + 1] = sampledColor.G; // Green
                        op.targetData[targetIndex + 2] = sampledColor.B; // Blue
                        op.targetData[targetIndex + 3] = sampledColor.A; // Alpha
                    }
                }
            }
        }
    }

    /*
     
                    op.Sampler.Invoke(new SampleOp()
                    {
                        sourceData = op.sourceData,
                        sourceWidth = op.sourceWidth,
                        sourceHeight = op.sourceHeight,
                        scaleX = bounds_target.Size.X,
                        scaleY = bounds_target.Size.Y,
                        targetX
                    });

     
     
     */

    /// <summary>
    /// Struct representing the scaling operation parameters.
    /// </summary>
    public struct SampleOp
    {
        /// <summary>
        /// The target X position for scaling.
        /// </summary>
        public float targetX;

        /// <summary>
        /// The target Y position for scaling.
        /// </summary>
        public float targetY;

        /// <summary>
        /// Byte array of the source image data (pixel data).
        /// Each pixel is assumed to be 4 bytes (RGBA).
        /// </summary>
        public byte[] sourceData;

        /// <summary>
        /// The width of the source image.
        /// </summary>
        public int sourceWidth;

        /// <summary>
        /// The height of the source image.
        /// </summary>
        public int sourceHeight;

    }

    /// <summary>
    /// Nearest Neighbor Scaling.
    /// </summary>
    public static Color Sample_NearestNeighbor(SampleOp op)
    {
        int XSample = (int)(op.targetX);
        int YSample = (int)(op.targetY);

        return SamplePixel(op.sourceData, op.sourceWidth, op.sourceHeight, XSample, YSample);
    }
    public static Color Sample_Bilinear(SampleOp op)
    {
        float srcX = op.targetX ;
        float srcY = op.targetY ;

        int x1 = (int)Math.Floor(srcX);
        int y1 = (int)Math.Floor(srcY);
        int x2 = Math.Min(x1 + 1, op.sourceWidth - 1);
        int y2 = Math.Min(y1 + 1, op.sourceHeight - 1);

        float xLerp = srcX - x1;
        float yLerp = srcY - y1;

        Color c11 = SamplePixel(op.sourceData, op.sourceWidth, op.sourceHeight, x1, y1);
        Color c12 = SamplePixel(op.sourceData, op.sourceWidth, op.sourceHeight, x1, y2);
        Color c21 = SamplePixel(op.sourceData, op.sourceWidth, op.sourceHeight, x2, y1);
        Color c22 = SamplePixel(op.sourceData, op.sourceWidth, op.sourceHeight, x2, y2);

        Color top = Color.Lerp(c11, c21, xLerp);
        Color bottom = Color.Lerp(c12, c22, xLerp);

        return Color.Lerp(top, bottom, yLerp);
    }

    private static Color SamplePixel(byte[] data, int width, int height, int x, int y)
    {
        int bytesPerPixel = 4; // Assuming RGBA format

        // Clamp coordinates to image bounds
        x = Math.Clamp(x, 0, width - 1);
        y = Math.Clamp(y, 0, height - 1);

        int index = (y * width + x) * bytesPerPixel;
        return new Color(data[index], data[index + 1], data[index + 2], data[index + 3]);
    }
  
}
