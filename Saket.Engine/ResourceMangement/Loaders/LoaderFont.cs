using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saket.Engine.Filetypes.Font.TrueType;
using Saket.Engine.Resources.Loaders;
using Saket.Engine.Typography;


namespace Saket.Engine.Resources.Loaders
{
    public class LoaderFont : ResourceLoader<Font>
    {
        public override Font Load(string name, ResourceManager resourceManager)
        {
            if(resourceManager.TryGetStream(name, out var stream))
            {
                //var r = OpenFont.LoadFromStream(stream);

            }

            throw new Exception("Failed to load font");
        }
    }
}
