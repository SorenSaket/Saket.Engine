using Saket.Engine.Geometry;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Saket.Engine.ResourceManagement.Loaders
{
    public class LoaderShape : ResourceLoader<StyledShapeCollection>
    {
        

        public override StyledShapeCollection Load(string name, ResourceManager resourceManager)
        {
            string path = name + ".svg";

            {
                // Load fragment shader code
                if (resourceManager.TryGetStream(path, out Stream stream))
                {
                
                    
                }
            }
            
            throw new Exception("Failed to load shape");
        }
    }
}
