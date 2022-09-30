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
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct SheetElement
    {
        public float x;
        public float y;
        public float w;
        public float h;
    }
}
