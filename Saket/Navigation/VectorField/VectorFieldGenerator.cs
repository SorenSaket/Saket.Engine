using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Numerics;

namespace Saket.Navigation.VectorField;

public class VectorFieldGenerator : IDataProvider<Vector2[,]>
{
    public Action<Vector2[,]> OnDataChanged { get; set; }
 
    public IDataProvider<MapData> MapProvider { get; set; }
    public IDataProvider<float[,]> HeatMapProvider { get; set; }

    public Vector2[,] Field { get; private set; }
    public Vector2[,] GetData() => Field;


    public VectorFieldGenerator(IDataProvider<MapData> mapProvider, IDataProvider<float[,]> heatMapProvider)
		{
			MapProvider = mapProvider;
			HeatMapProvider = heatMapProvider;
		}

    public static Vector2[,] GenerateVectorField(float[,] heatmap)
    {
        // The size of the heightmap and heatmap
        int width = heatmap.GetLength(0);
        int height = heatmap.GetLength(1);
        const int convsize = 1;

        Vector2[,] field = new Vector2[width, height];

        for (int rx = 0; rx < width; rx++)
        {
            for (int ry = 0; ry < height; ry++)
            {
                // Do not generate a vector for walls
                //if (map.values[i] >= wallValue)
                //    continue; 
                Vector2 dir = Vector2.Zero;
                Vector2 invWallDir = Vector2.Zero;

                float minValue = float.MaxValue;

                for (int x = -convsize; x <= convsize; x++)
                {
                    for (int y = -convsize; y <= convsize; y++)
                    {
                        // Do not run on self
                        if (x == 0 && y == 0)
                            continue;

                        int cx = (rx + x);
                        int cy = (ry + y);

                        // Do not run out of bounds
                        if (cx >= width || cx < 0)
                            continue;
                        if (cy >= height || cy < 0)
                            continue;

                        if (heatmap[cx, cy] < minValue)
                        {
                            dir = new Vector2(x, y);
                            minValue = heatmap[cx, cy];
                        }
                    }
                }

                field[rx, ry] = Vector2.Normalize(dir);
            }
        }

        return field;
    }

    public static Vector2[,] GenerateVectorFieldA(float[,] heatmap)
    {
        // The size of the heightmap and heatmap
        int width = heatmap.GetLength(0);
        int height = heatmap.GetLength(1);
        const int convsize = 1;

        Vector2[,] field = new Vector2[width, height];

        for (int rx = 0; rx < width; rx++)
        {
            for (int ry = 0; ry < height; ry++)
            {
                // Do not generate a vector for walls
                //if (map.values[i] >= wallValue)
                //    continue; 
                Vector2 dir = Vector2.Zero;
                Vector2 invWallDir = Vector2.Zero;

                float minValue = float.MaxValue;

                for (int x = -convsize; x <= convsize; x++)
                {
                    for (int y = -convsize; y <= convsize; y++)
                    {
                        // Do not run on self
                        if (x == 0 && y == 0)
                            continue;

                        int cx = (rx + x);
                        int cy = (ry + y);

                        // Do not run out of bounds
                        if (cx >= width || cx < 0)
                            continue;
                        if (cy >= height || cy < 0)
                            continue;

                        if (heatmap[cx, cy] < minValue)
                        {
                            dir = new Vector2(x, y);
                            minValue = heatmap[cx, cy];
                        }

                        
                        if ((heatmap[cx, cy] == float.MaxValue || float.IsPositiveInfinity(heatmap[cx, cy])) && ((x==0) || (y == 0)))
                            invWallDir += new Vector2(x, y);
                        /*
                        dir -= new Vector2(x, y).normalized * (heatmap.values[ci]);*/
                    }
                }

                //dir.normalized;//
                //r.values[i] = negate(dir, invWallDir).normalized; 

                field[rx, ry] = Vector2.Normalize(Negate(dir, invWallDir));
            }
        }

        return field;
    }


    static Vector2 Negate(Vector2 val, Vector2 negation)
    {
        if (negation == Vector2.Zero)
            return val;

        if (negation.X != 0 && MathF.Sign(negation.X) == MathF.Sign(val.X))
            val.X = 0;

        if (negation.Y != 0 && MathF.Sign(negation.Y) == MathF.Sign(val.Y))
            val.Y = 0;

        return val;
    }


    public void RegenerateData()
	{
        if (HeatMapProvider == null || HeatMapProvider.GetData() == null)
            return;

        Field = GenerateVectorField(HeatMapProvider.GetData());
        OnDataChanged?.Invoke(Field);
    }
}