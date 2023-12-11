using Saket.Engine.Math.Types;
using Saket.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Saket.Navigation.VectorField
{
    public enum Falloff
    {
        None = 0,
        Constant,
        Linear,
        Exponential
    }


    /// <summary>
    /// This provides a 2d
    /// </summary>
    public class HeatmapGenerator : IDataProvider<float[,]>
    {
        /// <summary>
        /// Callback for when the heatmap changes
        /// </summary>
        public Action<float[,]> OnDataChanged { get; set; }

        /// <summary>
        /// Where to source the mapdata from
        /// </summary>
        public IDataProvider<MapData> MapProvider { get; set; }
        
        /// <summary>
        /// Optional Post process additive heatmap modifieres
        /// </summary>
        public List<IDataProvider<float[,]>> Modifiers { get; private set; } = [];

        /// <summary>
        /// 
        /// </summary>
        public IDataListProvider<AffectorInstance> AffectorProvider { get; set; }

        // Cache of the heatmap
        // Delete ?
        public float[,] HeatMap { get; private set; }
        public float[,] GetData() => HeatMap;

        public void RegenerateData()
        {
            if( MapProvider == null || MapProvider.GetData() == null || !MapProvider.GetData().Valid)
			{
                Debug.WriteLine("Invalid MapProvider Data");
                return;
            }
            if (AffectorProvider == null || AffectorProvider.GetData() == null || AffectorProvider.GetData().Count <= 0)
            {
                Debug.WriteLine("Invalid AffectorProvider Data");
                return;
            }

            HeatMap = GenerateGlobalHeatMap(MapProvider.GetData(), AffectorProvider.GetData());
            OnDataChanged?.Invoke(HeatMap);
        }
        
        
        public HeatmapGenerator(IDataProvider<MapData> mapProvider, IDataListProvider<AffectorInstance> affectorProvider)
		{
			this.MapProvider = mapProvider;
			this.AffectorProvider = affectorProvider;
        }


		public void AddModifer(IDataProvider<float[,]> provider)
		{
            Modifiers.Add(provider);
            provider.OnDataChanged += (data) => RegenerateData();
            RegenerateData();
        }


		private float[,] GenerateGlobalHeatMap(MapData data, List<AffectorInstance> affectors)
        {
            int mainIndex = affectors.FindIndex((x) => x.Affector.Main);

            if (mainIndex == -1)
            {
                Debug.WriteLine("No main affector found");
            }
            if (affectors.Count((x) => x.Affector.Main) > 1)
                Debug.WriteLine("Multiple main affector found. This is not allowed");

            float[,] modMap = new float[data.Width, data.Height];

            if (affectors.Count > 0)
            {
                for (int i = 0; i < affectors.Count; i++)
                {
                    if(affectors[i].Valid && !affectors[i].Affector.Main && affectors[i].Heatmap != null)
                        Utilities.FlattenUniform(ref modMap, affectors[i].Heatmap as float[,]);
                    // float max = heatMaps[i].Cast<float>().Where((x) => !float.IsPositiveInfinity(x) && x != float.MaxValue).Max();
                    //Utilities.Modify(ref heatMaps[i], (x) => (x - max));
                    /* heatMaps[i] = heatMaps[i].Modify((x) =>
                     {
                         if (x != 0)
                             return (x - max);
                         return 0;
                     });*/
                }
            }

			for (int i = 0; i < Modifiers.Count; i++)
			{
                if(Modifiers[i].GetData() != null)
                    Utilities.FlattenJagged(ref modMap, Modifiers[i].GetData());
            }

            VectorFieldAffector mainAffector = affectors[mainIndex].Affector;

            return GenerateHeatmap(data,
                (int)MathF.Round(mainAffector.Position.X / data.NodeSize),
                (int)MathF.Round(mainAffector.Position.Y / data.NodeSize),
                (int)MathF.Round(mainAffector.Radius / data.NodeSize),
                mainAffector.Stength, GetFalloffFunction(mainAffector.Falloff, mainAffector.FalloffValue), mainAffector.Blocking, modMap).Modify((x) => { if (x == 0) return float.PositiveInfinity; return x; });
        }






        /*
        private void UpdateAffector(AffectorInstance affector)
		{
			if (affector.Valid) { 

                if (!affector.Affector.Main) // No reason to cache the main affector
                    affector.Cache[0] = GenerateAffectorHeatmap(mapEvaluator.MapData, affector.Affector);
                
                 Generate();
            }
        }*/
        public delegate float FalloffFunction(float iteration);
        public static FalloffFunction GetFalloffFunction(Falloff falloff, float value)
        {
            return falloff switch
            {
                Falloff.None => (x) => 1f,
                Falloff.Constant => (x) => value,
                Falloff.Linear => (x) => x * value,
                Falloff.Exponential => (x) => MathF.Pow(value, -x),
                _ => (x) => 1f,
            };
        }

        public static float[,] GenerateHeatmap(MapData mapData, int affectorX, int affectorY, int range = int.MaxValue, float stength = 1f,  FalloffFunction falloff = null, bool blocking = true, float[,] modMap = null, Vector2[,] avgSpeedMap = null)
        {
            // Default falloff function. returns 1.
            if (falloff == null)
                falloff = (q) => 1f;

            //if(modMap == null)

            // The size of the heightmap and heatmap
            int width = mapData.Width;
            int height = mapData.Height;
            float cornerMod = 1f;
            // The heatmap to return
            float[,] heatMap = new float[width, height];
            bool[,] closedMap = new bool[width, height];
            heatMap.Populate(float.PositiveInfinity);
            // TODO remove reliance on int2 type
            Queue<Int2> openList = new Queue<Int2>();

            openList.Enqueue(new Int2(affectorX, affectorY));
            heatMap[affectorX, affectorY] = 1;

            while (openList.Count > 0)
            {
                Int2 realPos = openList.Dequeue();
                int rx = realPos.X;
                int ry = realPos.Y;
                closedMap[rx, ry] = true;

                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        // Do not run on self or corners
                        if ((x == 0 && y == 0) || (x==1 && y== 1) || (x == 1 && y == -1) || (x == -1 && y == 1) || (x == -1 && y == -1))
                            continue;

                        int cx = (rx + x);
                        int cy = (ry + y);

                        // Do not run out of bounds
                        if (cx >= width || cx < 0)
                            continue;
                        if (cy >= height || cy < 0)
                            continue;
                        
                        // Cancel if closed
                        if (closedMap[cx, cy])
                            continue;
                        
                        float newValue = heatMap[rx, ry]+1;

                        if ((blocking && mapData.Flags[cx, cy].HasFlag(MapFlags.Blocking)) || mapData.Flags[cx, cy].HasFlag(MapFlags.Steep)) // If is impassible terrain
                        {
                            heatMap[cx, cy] = float.PositiveInfinity; // TODO health of building
                            continue;
                        }//
                        else
                        {
                            if (modMap != null)
                                newValue += modMap[cx, cy];
                           //newValue += cornerMod; // sqrt(2)

                            /*if (avgSpeedMap[cx, cy].magnitude > 0)
                            {
                                float dir = Vector2.Dot(new Vector2(x, y), avgSpeedMap[cx, cy]);
                                if (dir < 0)
                                    newValue += dir * -32;
                            }*/
                        }

                        // If this tile has not been set or lower value is possible
                        if (heatMap[cx, cy] == 0 || MathF.Abs(heatMap[cx, cy]) > MathF.Abs(newValue))
                        {
                            heatMap[cx, cy] = newValue;
                            openList.Enqueue(new Int2(cx,cy));
                            closedMap[cx, cy] = true;
                        }
                    }
                }
            }
            return heatMap;
        }




        public static void GenerateHeatmapStep(HeatmapCalculationState state)
        {
            Int2 realPos = state.openList.Dequeue();
            int rx = realPos.X;
            int ry = realPos.Y;
            state.closedMap[rx, ry] = true;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    // Do not run on self or corners
                    if ((x == 0 && y == 0) || (x == 1 && y == 1) || (x == 1 && y == -1) || (x == -1 && y == 1) || (x == -1 && y == -1))
                        continue;

                    int cx = (rx + x);
                    int cy = (ry + y);

                    // Do not run out of bounds
                    if (cx >= state.mapData.Width || cx < 0)
                        continue;
                    if (cy >= state.mapData.Height || cy < 0)
                        continue;

                    // Cancel if closed
                    if (state.closedMap[cx, cy])
                        continue;

                    float newValue = state.heatMap[rx, ry] + 1;

                    if ((state.mapData.Flags[cx, cy].HasFlag(MapFlags.Blocking)) || state.mapData.Flags[cx, cy].HasFlag(MapFlags.Steep)) // If is impassible terrain
                    {
                        state.heatMap[cx, cy] = float.PositiveInfinity; // TODO health of building
                        continue;
                    }


                    // If this tile has not been set or lower value is possible
                    if (state.heatMap[cx, cy] == 0 || MathF.Abs(state.heatMap[cx, cy]) > MathF.Abs(newValue))
                    {
                        state.heatMap[cx, cy] = newValue;
                        state.openList.Enqueue(new Int2(cx, cy));
                        state.closedMap[cx, cy] = true;
                    }
                }
            }
        }


        public static float[,] GenerateAffectorHeatmap(MapData data, Affector affector)
        {
            if (data == null || !data.Valid)
                throw new Exception("Invalid Data");

            return GenerateHeatmap(
                data,
               (int) MathF.Round(affector.Position.X / data.NodeSize),
               (int)MathF.Round(affector.Position.Y / data.NodeSize),
               (int)MathF.Round(affector.Radius / data.NodeSize),
                affector.Stength, 
                GetFalloffFunction(affector.Falloff, affector.FalloffValue), 
                affector.Blocking);
        }
	}


    public class HeatmapCalculationState
    {
        public MapData mapData;
        public float[,] heatMap;
        public bool[,] closedMap;
        public Queue<Int2> openList;
    }

}
