using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Resources.Loaders
{
    public class LoaderSheet : ResourceLoader<Sheet>
    {
        public override Sheet Load(string name, ResourceManager resourceManager)
        {
            string basepath = "sheet_" + name + ".json";
            
            if (resourceManager.TryGetStream(basepath, out Stream? stream))
            {
                StreamReader reader = new StreamReader(stream);
                var sheet = JsonConvert.DeserializeObject<Sheet>(reader.ReadToEnd());
                if (sheet != null)
                {
                    return sheet;
                }        
            }

            throw new Exception("Failed to load sheet");
        }
    }
}
