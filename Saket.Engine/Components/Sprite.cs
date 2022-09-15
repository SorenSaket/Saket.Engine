using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
namespace Saket.Engine
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Sprite
    {
        public UInt32 tex;
        public UInt32 spr;
        public UInt32 color;
        public Sprite(UInt32 tex, UInt32 spr, UInt32 color)
        {
            this.tex = tex;
            this.spr = spr;
            this.color = color;
        }
    }
}
