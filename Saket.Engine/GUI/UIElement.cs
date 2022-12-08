using Saket.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Saket.UI

{


    [StructLayout(LayoutKind.Sequential)]
    public struct Button
    {
        public int text;
        public int box;
    }

    public struct StyleSheet
    {
        public List<Style> styles;

        public StyleSheet Add(Style style)
        {
            styles.Add(style);
            return this;
        }
    }

    public struct Style
    {
        public Selector selector;
        public Styles styles;

        public Style(Selector selector, Styles styles)
        {
            this.selector = selector;
            this.styles = styles;
        }
    }
    // Consider messing with Pack
    // https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.structlayoutattribute.pack?view=net-6.0
    // this could ensure that no padding is added
    // probably destroys possibility of interop
    // "Occasionally, the field is used to reduce memory requirements by producing a tighter packing size.
    // However, this usage requires careful consideration of actual hardware constraints, and may actually degrade performance."

    public struct Styles
    {
        public string Width
        {
            set => Utils.ParseMeasurement(value, out width, out mw);
        }
        public string Height
        {
            set => Utils.ParseMeasurement(value, out height, out mh);
        }



        public int width;
        

        public int height;

        public uint color;

        public uint background_color;


        public Measurement mw;
        public Measurement mh;

    }

    public struct Selector
    {
        public string id;
        public string[] classes;
        public Selector(string id = null, string[] classes = null)
        {
            this.id = id;
            this.classes = classes;
        }
    }

    public struct Element_Text
    {
        public int text;
    }
 
    [StructLayout(LayoutKind.Auto)]
    public struct Widget
    {
        public int parent;
        public int id; // Index into string array
        public int classes;

        public Widget(int parent = -1, int id = -1, int classes = -1) 
        {
            this.parent = parent;
            this.id = id;
            this.classes = classes;
        }
    }

    public struct ElementValue
    {
        public Measurement Measurement { get; set; }
        public float Value { get; set; }
    }

    public enum Measurement : byte
    {
        Pixels = 0,
        Percentage = 1,
        Stretch = 2,
    }

    [Flags]
    public enum ElementFlags : uint
    {
        Direction = 0,
    }

    public struct Primitive
    {
        float x, y, w, h;
        uint data;
        uint clipTarget;
    }
}
