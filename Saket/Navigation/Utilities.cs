using System;
using System.Collections;
using System.Collections.Generic;

namespace Saket.Navigation.VectorField
{
    public static class Utilities
    {
        // TODO move to array exentions

        public static void FlattenUniform(ref float[,] a, float[,] b)
        {
            int width = a.GetLength(0);
            int height = a.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    a[x, y] += b[x, y];
                }
            }
        }


        // 48 x 32
        // 24 x 16
        //

        public static void FlattenJagged(ref float[,] a, float[,] b)
        {
            int width = a.GetLength(0);
            int height = a.GetLength(1);

            float ratioX = width / b.GetLength(0);
            float ratioY = height / b.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    a[x, y] += b[(int)( x/ ratioX), (int)(y/ ratioY)];
                }
            }
        }

        public static float[,] Flatten (this float[][,] maps)
		{
            int width = maps[0].GetLength(0);
            int height = maps[0].GetLength(1);
            float[,] r = new float[width,height];

			for (int i = 0; i < maps.Length; i++)
			{
                if (maps[i] == null)
                    continue;
                for (int y = 0; y < maps[0].GetLength(1); y++)
				{
                    for (int x = 0; x < maps[0].GetLength(0); x++)
                    {
                        r[x, y] += maps[i][x, y];
                    }
				}
			}
            return r;
        }
        public static float[,] Modify(this float[,] map, Func<float, float> func)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    map[x, y] = func(map[x, y]);
                }
            }
            return map;
        }
        public static void Modify(ref float[,] map, Func<float, float> func)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            for (int y = 0; y < height; y++)
               
            {
                for (int x = 0; x < width; x++)
                {
                    map[x, y] = func(map[x, y]);
                }
            }
        }
        public static void Modify(ref float[][,] maps, Func<float,float> func)
        {
            int width = maps[0].GetLength(0);
            int height = maps[0].GetLength(1);

            for (int i = 0; i < maps.Length; i++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        maps[i][x, y] = func(maps[i][x, y]);
                    }
                }
            }
        }
    }
}