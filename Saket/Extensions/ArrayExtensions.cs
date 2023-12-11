namespace Saket
{
    public static class ArrayExtensions 
    {
        public static void Populate<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
        }
        public static void Populate<T>(this T[,] arr, T value)
        {
            for (int x = 0; x < arr.GetLength(0); x++)
            {
				for (int y = 0; y < arr.GetLength(1); y++)
				{
                    arr[x,y] = value;
                }
            }
        }



        public static float Min(this float[,] arr)
        {
            float min = float.PositiveInfinity;
            for (int x = 0; x < arr.GetLength(0); x++)
            {
                for (int y = 0; y < arr.GetLength(1); y++)
                {
                    if (arr[x, y] < min && !float.IsNegativeInfinity(arr[x, y]) && arr[x, y] != float.MinValue)
                        min = arr[x, y];
                }
            }
            return min;
        }

        public static float Max(this float[,] arr)
		{
            float max = float.NegativeInfinity;
            for (int x = 0; x < arr.GetLength(0); x++)
            {
                for (int y = 0; y < arr.GetLength(1); y++)
                {
                    if (arr[x, y] > max && !float.IsPositiveInfinity(arr[x, y]) && arr[x, y] != float.MaxValue)
                        max = arr[x, y];
                }
            }
            return max;
        }



        public static float MinNonInf(this float[] arr)
        {
            float min = float.PositiveInfinity;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] < min && !float.IsNegativeInfinity(arr[i]) && arr[i] != float.MinValue)
                    min = arr[i];
            }
            return min;
        }

        public static float MaxNonInf(this float[] arr)
        {
            float max = float.NegativeInfinity;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] > max && !float.IsPositiveInfinity(arr[i]) && arr[i] != float.MaxValue)
                    max = arr[i];
            }
            return max;
        }

    }
}
