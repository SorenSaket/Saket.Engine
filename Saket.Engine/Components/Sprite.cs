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
        /// <summary> OpenGL Texture Handle </summary>
        public Int32 tex;
        /// <summary> Sheet Element Index </summary>
        public Int32 spr;
        /// <summary> Color Tint </summary>
        public UInt32 color;
        public Sprite(Int32 tex, Int32 spr, UInt32 color)
        {
            this.tex = tex;
            this.spr = spr;
            this.color = color;
        }
    }
}