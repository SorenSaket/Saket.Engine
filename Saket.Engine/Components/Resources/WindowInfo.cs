using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine
{
    public class WindowInfo
    {
        public int width;
        public int height;
        public Vector2 Size => new Vector2(width, height);
        public float AspectRatio => (float)width / (float)height;
        public WindowInfo()
        {
        }
        public WindowInfo(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }
}
