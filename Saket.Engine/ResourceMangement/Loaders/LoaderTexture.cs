using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using StbImageSharp;

namespace Saket.Engine.Resources.Loaders
{
    public class LoaderTexture : ResourceLoader<Texture>
    {
        public LoaderTexture() : base()
        {
            
        }

        public override Texture Load(string textureName, ResourceManager resourceManager)
        {
            string path = "texture_" + textureName + ".png";

            {
                // Load fragment shader code
                if (resourceManager.TryGetStream(path, out Stream stream))
                {
                    StbImage.stbi_set_flip_vertically_on_load(1);
                    ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                    
                    return new Texture(image);
                }
            }

            throw new Exception("Failed to load image");
        }
    }
}
