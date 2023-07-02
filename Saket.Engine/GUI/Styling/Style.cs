using Saket.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.GUI.Styling
{
    [Flags]
    public enum ElementFlags : uint
    {
        Direction = 0,
        SelfDirected
    }

    // Todo research alternative to nullable
    // value orderings for packing? 
 
    public struct Style
    {
        public bool Vertical
        {
            get => Flags.HasFlag(ElementFlags.Direction);
            set { Flags = value ? Flags |= ElementFlags.Direction : Flags &= ~ElementFlags.Direction;  }
        }
        public bool SelfDirected
        {
            get => Flags.HasFlag(ElementFlags.SelfDirected);
            set { Flags = value ? Flags |= ElementFlags.SelfDirected : Flags &= ~ElementFlags.SelfDirected; }
        }


        public ElementFlags Flags;

        public ElementValue? width;
        
        public ElementValue? height;

        /// <summary>
        /// Padding
        /// </summary>
        public ElementValue? innerSpacing;

        /// <summary>
        /// outer spacing / Margin
        /// </summary>
        public ElementValue? outerSpacing;

        public Color? color;
        public Color? background_color;

        public void Combine(Style style)
        {
            throw new NotImplementedException();
        }
    }
}
