using Saket.Engine.GeometryD2.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Saket.Engine.Graphics.Packing
{
    public class Packer
    {
        public struct SettingsPacker
        {
            /// <summary>
            /// The padding to use in between tiles
            /// </summary>
            public float padding;

            /// <summary>
            /// The space around 
            /// </summary>
            public float margin;

            public Rotation rotation;

            public SettingsPacker()
            {
                padding = 0;
                margin = 0;
                rotation = Rotation.NoRotate;
            }

            public enum Rotation
            {
                /// <summary>
                /// The Packer doesn't flip nor rotate the tiles
                /// </summary>
                NoRotate,
                /// <summary>
                /// The packer will only rotate in 90 degree increments
                /// </summary>
                Rotate90,
                /// <summary>
                /// The packer will rotate arbitrarily
                /// </summary>
                Rotate
            }

            public enum ExportSize
            {
                KeepPowerOfTwo,
                Minimum,
            }

        }

        public Packer()
        {
            emptySpaces = new List<Rectangle>(128);
        }

        List<Rectangle> emptySpaces;
        /// <summary>
        /// Pure function. Always adds the bigger split v first.
        /// a span Rectangle of count 2 must be provided.
        /// </summary>
        /// <param name="rectToFit"></param>
        /// <param name="emptySpace"></param>
        /// <param name="splits">The empty spaces left over.</param>
        /// <param name="splitCount">the number of empty spaces left over.</param>
        /// <returns></returns>
        bool TryFitAndSplit(ref Rectangle rectToFit, Rectangle emptySpace, in Span<Rectangle> splits, out int splitCount)
        {
            float freeWidth = emptySpace.Width - rectToFit.Width;
            float freeHeight = emptySpace.Height - rectToFit.Height;

            float halfFreeWidth = freeWidth / 2f;
            float halfFreeHeight = freeHeight / 2f;
            // It doesn't fit
            if (freeWidth < 0 || freeHeight < 0)
            {
                splitCount = 0;
                return false;
            }

            rectToFit.X = emptySpace.X - freeWidth / 2f;
            rectToFit.Y = emptySpace.Y - freeHeight / 2f;

            // It fits exactly 
            if (freeWidth == 0 && freeHeight == 0)
            {
                splitCount = 0;
                return true;
            }

            // It fits exactly on one side
            // Fits exacltly on Y
            if (freeWidth > 0 && freeHeight == 0)
            {
                splits[0] = new Rectangle(emptySpace.X + rectToFit.Width/2f, emptySpace.Y, freeWidth, emptySpace.Height);
                splitCount = 1;
                return true;
            }
            // Fits exacltly on X
            if (freeWidth == 0 && freeHeight > 0)
            {
                splits[0] = new Rectangle(emptySpace.X, emptySpace.Y + rectToFit.Height / 2f, emptySpace.Width, freeHeight);
                splitCount = 1;
                return true;
            }


            // ┌─────┐
            // │     │
            // ├─┬───┤
            // └─┴───┘
            // If bigger split is Y
            if (freeWidth < freeHeight)
            {
                // Bigger split
                splits[0] = new Rectangle(emptySpace.X, emptySpace.Y + rectToFit.Height / 2f, emptySpace.Width, freeHeight);
                // Smaller Split
                splits[1] = new Rectangle(emptySpace.X + rectToFit.Width / 2f, emptySpace.Y - (halfFreeHeight), freeWidth, rectToFit.Height);
                splitCount = 2;
                return true;
            }

            // ┌─┬───┐
            // │ │   │
            // ├─┤   │
            // └─┴───┘
            // Bigger split
            splits[0] = new Rectangle(emptySpace.X + rectToFit.Width/2f,   emptySpace.Y, freeWidth, emptySpace.Height);
            // Smaller Split
            splits[1] = new Rectangle(emptySpace.X - (halfFreeWidth), emptySpace.Y + rectToFit.Height/2f, rectToFit.Width, freeHeight);

            splitCount = 2;
            return true;
        }
        /// <summary>
        /// 
        /// note this modifies the position of the tiles but not the ordering.
        /// </summary>
        /// <param name="tiles"></param>
        public bool TryPack(Span<Rectangle> tiles, float width, float height, SettingsPacker settings = default)
        {

           

            // Create a span of the indicies
            //Span<int> sortedIndices = stackalloc int[tiles.Length];
            //for (int i = 0; i < tiles.Length; i++)
            //{
            //sortedIndices[i] = i;
            // }

            // Sort the indicies based on the area of the tiles
            // Dont think caching area is worth it since its a simple computation
            // TODO make sorting method that only sorts sortedIndices based on tiles
            //MemoryExtensions.Sort(tiles, sortedIndices, (x, y) => y.Height.CompareTo(x.Height));

            emptySpaces.Clear();
            // Add the first area that fills the entire canvas
            emptySpaces.Add(new Rectangle(width/2f, height/2f, width - settings.margin * 2, height - settings.margin * 2));


            Span<Rectangle> splits = stackalloc Rectangle[2];

            //Iterate all tiles placing them on the canvas and splitting up EmptySpaces each time
            for (int i = 0; i < tiles.Length; i++)
            {
                // ref local to the tile
                ref Rectangle t = ref tiles[i];
                bool success = false;
                // Get an empty space which the tile fits into
                for (int k = emptySpaces.Count-1; k >= 0; k--)
                {
                    success = TryFitAndSplit(ref t, emptySpaces[k], splits, out var count);

                    if (success)
                    {
                        emptySpaces.RemoveAt(k);
                        for (int s = 0; s < count; s++)
                        {
                            emptySpaces.Add(splits[s]);
                        }
                        break;
                    }
                }

                if (!success)
                    return false;
            }

            return true;
        }

    }
}