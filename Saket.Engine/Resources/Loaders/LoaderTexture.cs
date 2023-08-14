using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using StbImageSharp;

namespace Saket.Engine.Resources.Loaders
{
    public class LoaderTexture : ResourceLoader<Image>
    {
        public LoaderTexture() : base()
        {
            
        }

        public override Image Load(string textureName, ResourceManager resourceManager)
        {
            string path = "texture_" + textureName + ".png";

            {
                // Load fragment shader code
                if (resourceManager.TryGetStream(path, out Stream stream))
                {
                    StbImage.stbi_set_flip_vertically_on_load(1);
                    ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                    
                    Image tex = new Image(image.Width, image.Height);
                    tex.data = image.Data;
                    return tex;
                }
            }

            throw new Exception("Failed to load image");
        }
    }
}
