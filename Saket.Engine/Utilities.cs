using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Saket.Engine
{
    public static class Utilities
    {
        public static bool Unwrap<T>(this Nullable<T> nullable, out T value) where T : struct
        {
            if (nullable == null)
            {
                value = default(T);
                return false;
            }
            else
            {
                value = nullable.Value;
                return true;
            }
        }

		public static Nullable<T> FirstOrNull<T>(this IEnumerable<T> enumerable, Predicate<T> predicate) where T : struct
        {
			foreach (var item in enumerable)
			{
				if(predicate(item))
					return item;
			}
			return null;
		}


        public static bool IndexOf<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, out int index) where T : struct
        {
            int i = 0;
            foreach (var item in enumerable)
            {
                if (predicate(item))
                {
                    index = i;
                    return true;
                }
                i++;
            }
            index = -1;
            return false;
        }

    }
}
