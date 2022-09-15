using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine
{
	public static class Extensions_Math
	{
		public static void Clamp(this ref float value, in float min, in float max)
		{
			value = value < min ? min : value > max ? max : value;
		}
	}
}