using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public static T? FirstOrNull<T>(this IEnumerable<T> enumerable, Predicate<T> predicate)
		{
			foreach (var item in enumerable)
			{
				if(predicate(item))
					return item;
			}
			return default(T);
		}
    }
}
