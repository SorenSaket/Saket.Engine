using Saket.Engine.Math.Types;
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
            emptySpaces = new List<Tile>(128);
        }

        List<Tile> emptySpaces;

        /// <summary>
        /// 
        /// note this modifies the position of the tiles but not the ordering.
        /// </summary>
        /// <param name="tiles"></param>
        public bool TryPack(Span<Tile> tiles, float width, float height, SettingsPacker settings = default)
        {
            /// Pure function. Always adds the bigger split v first
            bool TryFitAndSplit(ref Tile t, Tile slot, in Span<Tile> splits, out int splitCount)
            {
                float freeWidth = slot.Width - t.Width;
                float freeHeight = slot.Height - t.Height;

                // It doesn't fit
                if (freeWidth < 0 || freeHeight < 0)
                {
                    splitCount = 0;
                    return false;
                }

                t.X = slot.X;
                t.Y = slot.Y;

                // It fits exactly 
                if (freeWidth == 0 && freeHeight == 0)
                {
                    splitCount = 0;
                    return true;
                }

                // It fits exactly on one side
                if (freeWidth > 0 && freeHeight == 0)
                {
                    splits[0] = new Tile(freeWidth, slot.Height, slot.X + t.Width, slot.Y);
                    splitCount = 1;
                    return true;
                }

                if (freeWidth == 0 && freeHeight > 0)
                {
                    splits[0] = new Tile(slot.Width, freeHeight, slot.X, slot.Y + t.Height);
                    splitCount = 1;
                    return true;
                }

                // 
                if (freeWidth < freeHeight)
                {
                    // Bigger split
                    splits[0] = new Tile(slot.Width, freeHeight, slot.X, slot.Y + t.Height);
                    // Smaller Split
                    splits[1] = new Tile(freeWidth, t.Height, slot.X + t.Width, slot.Y);
                    splitCount = 2;
                    return true;
                }

                // Bigger split
                splits[0] = new Tile(freeWidth, slot.Height, slot.X + t.Width, slot.Y);
                // Smaller Split
                splits[1] = new Tile(t.Width, freeHeight, slot.X, slot.Y + t.Height);

                splitCount = 2;
                return true;
            }

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
            emptySpaces.Add(new Tile(width- settings.margin * 2, height - settings.margin*2, settings.margin, settings.margin));


            Span<Tile> splits = stackalloc Tile[2];

            //Iterate all tiles placing them on the canvas and splitting up EmptySpaces each time
            for (int i = 0; i < tiles.Length; i++)
            {
                // ref local to the tile
                ref Tile t = ref tiles[i];
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