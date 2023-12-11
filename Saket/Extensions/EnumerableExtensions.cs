using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Numerics;

namespace Saket
{
    public static class EnumerableExtensions
    {/*
        public static int RandomIndexByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector)
        {
            float totalWeight = sequence.Sum(weightSelector);
            // The weight we are after...
            float itemWeightIndex = UnityEngine.Random.value * totalWeight;
            
            float currentWeightIndex = 0;

			for (int i = 0; i < sequence.Count(); i++)
			{
                currentWeightIndex += weightSelector(sequence.ElementAt(i));
                if (currentWeightIndex >= itemWeightIndex)
                    return i;
            }

            return -1;
        }
        public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector)
        {
            float totalWeight = sequence.Sum(weightSelector);
            // The weight we are after...
            float itemWeightIndex = UnityEngine.Random.value * totalWeight;
            float currentWeightIndex = 0;

            foreach (var item in from weightedItem in sequence select new { Value = weightedItem, Weight = weightSelector(weightedItem) })
            {
                currentWeightIndex += item.Weight;

                // If we've hit or passed the weight we are after for this item then it's the one we want....
                if (currentWeightIndex >= itemWeightIndex)
                    return item.Value;

            }

            return default(T);
        }
        public static T RandomElement<T>(this IEnumerable<T> sequence)
        {
            return sequence.ElementAt(UnityEngine.Random.Range(0, sequence.Count()));
        }

        */

        public static Vector3 Average(this IEnumerable<Vector3> source)
		{
            Vector3 avg = new Vector3(0, 0, 0);
			for (int i = 0; i < source.Count(); i++)
			{
                avg += source.ElementAt(0);
			}
            return avg / source.Count();
        }

    }
}
