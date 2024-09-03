using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using WebGpuSharp;

namespace Saket.Engine.Graphics
{
    public class TextureGroup
    {
        public Texture texture;
        public TextureView view;
        public Sampler sampler;

        public TextureGroup(Texture texture, TextureView view, Sampler sampler)
        {
            this.texture = texture;
            this.view = view;
            this.sampler = sampler;
        }
    }
}
