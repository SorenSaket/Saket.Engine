using Saket.Engine.Graphics.Packing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Graphics
{
    public class Atlas : List<Tile>
    {
        public Texture texture;

        public Atlas(Texture texture, int initialCapacity = 128) : base(initialCapacity)
        {
            this.texture = texture;
        }
    }
}
