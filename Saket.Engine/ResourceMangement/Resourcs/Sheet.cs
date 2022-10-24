using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine
{
    public class Sheet
    {
        public string[] names;
        public SheetElement[] rects;
        public float scale = 1f;

        public Sheet()
        {
            names = Array.Empty<string>();
            rects = Array.Empty<SheetElement>();    
            scale = 1f;
        }

        public Sheet(int columns, int rows, float scale = 1)
        {
            names = new string[columns * rows];
            rects = new SheetElement[columns * rows];

            float w = 1f / (columns);
            float h = 1f / rows;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    names[x + y * columns] = $"{x} : {y}";
                    rects[x + y * columns] = new SheetElement(((float)x / columns), ((float)y / rows), w, h);
                }
            }

            this.scale = scale;
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct SheetElement
    {
        public float x;
        public float y;
        public float w;
        public float h;

        public SheetElement(float x, float y, float w, float h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }
    }
}
