using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine
{
	internal abstract class Resource
	{
		public string Name { get; set; }
        // https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/weak-references
        // https://docs.microsoft.com/en-us/dotnet/api/system.weakreference
        internal WeakReference<IResource> value;

		public Resource(string name)
		{
			Name = name;
			value = new WeakReference<IResource>(null, false);
        }
    }

	/*
	public struct ResourceHandle
	{
		private Resource resource;
		internal ResourceHandle(Resource resource)
		{
			this.resource = resource;
		}

		public T Get { 
			get {
				bool isloaded = resource.value.TryGetTarget(out var target);

                if (isloaded)
				{

				}
				else
				{

				}
				return ((T)resource.value.Target);
			}
		}
	}*/
}
