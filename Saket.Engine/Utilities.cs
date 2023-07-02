using System;
using System.Collections.Generic;

namespace Saket.Engine
{
    /// <summary>
    /// Utilities and extensions
    /// </summary>
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

        public static bool Unwrap<T>(this T nullable, out T value) where T : class
        {
            if (nullable == null)
            {
                value = null;
                return false;
            }
            else
            {
                value = nullable;
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


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="predicate"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool FirstOrFalse<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, out T value)
        {
            foreach (var item in enumerable)
            {
                if (predicate(item))
                {
                    value = item;
                    return true;
                }

            }
            // Value will be null if returns false
            // Ignore with !
            value = default(T)!;
            return false;
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

        public static int AddUnique<T>(this IList<T> list, T itemToAdd)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Equals(itemToAdd))
                {
                    return i;
                }
            }
            list.Add(itemToAdd); 
            return list.Count-1;
        }
    }
}