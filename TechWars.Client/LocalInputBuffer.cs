using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechWars.Client
{
	public class LocalInputBuffer<T> 
	{
		public LocalInputBuffer()
		{
			Queue = new();
		}

		public Queue<Tuple<ushort,T>> Queue { get; set; }
	}
}
