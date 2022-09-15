using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Networking
{
	public static class NetworkCommon
	{
		private const float MaxGameSequence = ushort.MaxValue;
		private const float HalfMaxGameSequence = MaxGameSequence / 2f;

		/// <summary>
		/// The difference/distance between the two ticks
		/// </summary>
		/// <param name="a">Newer tick</param>
		/// <param name="b">Later tick</param>
		/// <param name="halfMax">The difference/distance between the two ticks</param>
		/// <returns></returns>
		public static int SeqDiff(int a, int b)
        {
            return (int)MathF.Ceiling( Diff(a, b, HalfMaxGameSequence));
        }
		
		public static ushort TickAdvance(ushort tick, int value = 1)
		{
			return (ushort)((tick + value) % ushort.MaxValue);
		}

		private static float Diff(float a, float b, float halfMax)
        {
            return (a - b + halfMax * 3f) % (halfMax * 2f) - (halfMax);
        }
    }
}