using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Navigation
{
    [System.Serializable]
    public class MapData : IDataProvider<MapData>
    {
        public MapData(int width, int height, float nodeSize)
        {
            NodeSize = nodeSize;
            Heights = new float[width, height];
            Normals = new Vector3[width, height];
            Flags = new MapFlags[width, height];
        }

        public float NodeSize { get; private set; }
        public float[,] Heights { get; set; }
        public Vector3[,] Normals { get; set; }
        public MapFlags[,] Flags { get; set; }

        public bool ValidPoint(int x, int y)
        {
            if (x < 0 || x >= Width)
                return false;
            if (y < 0 || y >= Height)
                return false;
            return true;
        }


        public int Width => Heights.GetLength(0);
        public int Height => Heights.GetLength(1);

        public bool Valid => Width * Height > 0;


        public MapData GetData()
        {
            return this;
        }

        public void RegenerateData()
        {
            throw new NotImplementedException();
        }

        public Action<MapData> OnDataChanged { get; set; }
        //
        //
    }
    // flags |= flag;// SetFlag
    // flags &= ~flag; // ClearFlag 
    [Flags]
    public enum MapFlags : int
    {
        None = 0,
        Blocking = 1 << 0,
        Steep = 1 << 1,
        Built = 1 << 2,
    };
}
