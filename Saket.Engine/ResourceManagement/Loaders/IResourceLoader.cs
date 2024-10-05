using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.ResourceManagement.Loaders
{
    public abstract class ResourceLoader
    {
        protected ResourceLoader(Type type)
        {
            TargetType = type;
        }

        public Type TargetType { get; set; }


    }

    public abstract class ResourceLoader<T> : ResourceLoader
    {
        protected ResourceLoader() : base(typeof(T))
        {

        }
         
        public abstract T Load(string name, ResourceManager resourceManager);
    }
}