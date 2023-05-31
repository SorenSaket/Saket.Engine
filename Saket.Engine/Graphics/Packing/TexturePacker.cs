using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Saket.Engine.Math.Geometry.Shapes;

namespace Saket.Engine.Graphics.Packing
{
    public class TexturePacker
    {
        public struct SettingsPacker
        {
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


        public TexturePacker()
        {


        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tiles"></param>
        public void Pack(Rectangle[] tiles)
        {
            Span<int> sortedIndices = stackalloc int[tiles.Length];

            for (int i = 0; i < tiles.Length; i++)
            {
                sortedIndices[i] = i;
            }

            // Dont think caching area is worth it since its a simple computation
            sortedIndices.Sort((x,y) => tiles[x].Area().CompareTo(tiles[y].Area()));

            // the size of the exported bounding box
            Vector2 size = new Vector2(1f, 1f);

            Vector2 pointer = Vector2.Zero;

            for (int i = 0; i < tiles.Length; i++)
            {

            }
        }
    }
}