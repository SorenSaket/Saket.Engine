using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using StbImageSharp;
using Saket.Engine.Graphics;

namespace Saket.Engine.ResourceManagement.Loaders
{
    public class LoaderTexture : ResourceLoader<ImageTexture>
    {
        public LoaderTexture() : base()
        {
            
        }

        public override ImageTexture Load(string textureName, ResourceManager resourceManager)
        {
            string path = "texture_" + textureName + ".png";

            {
                // Load fragment shader code
                if (resourceManager.TryGetStream(path, out Stream stream))
                {
                    StbImage.stbi_set_flip_vertically_on_load(1);
                    ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                    
                    ImageTexture tex = new ImageTexture(image.Data, image.Width, image.Height);
                    return tex;
                }
            }

            throw new Exception("Failed to load image");
        }
    }
}
