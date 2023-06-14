using Saket.Engine.Graphics.Packing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Graphics
{
    public class TextureAtlas
    {
        public Texture texture;
        public List<Tile> tiles;

        public TextureAtlas(Texture texture, int initialCapacity = 128)
        {
            this.texture = texture;
            this.tiles = new List<Tile>(initialCapacity);
        }

        public TextureAtlas(Texture texture, int columns, int rows)
        {
            this.texture = texture;
            this.tiles = new List<Tile>(columns*rows);
            float w = 1f / (columns);
            float h = 1f / rows;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    tiles.Add(new Tile( w, h, (float)x / columns, (float)y / rows ));
                }
            }
        }
    }
}