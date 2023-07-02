using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Saket.Engine.Resources.Databases
{
    public class DatabaseEmbedded : Database
    {
        public HashSet<string> assets;

        private Assembly assembly;

        public override HashSet<string> AvaliableResources => assets;

        public DatabaseEmbedded(Assembly assembly)
        {
            this.assembly = assembly;
            assets = GetAllResourceNames();
            
        }

        public override Stream? TryGetStream(string name)
        {
            return assembly.GetManifestResourceStream(name);
        }

        private HashSet<string> GetAllResourceNames()
        {
            return assembly.GetManifestResourceNames().ToHashSet() ?? new HashSet<string>();
        }

        public override void Initialize()
        {
            
        }
    }
}
