using Saket.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.GUI.Styling
{
    /// <summary>
    /// Contains all styling for all GUI elements. 
    /// a zero/default value means that it's unset. should inhert values from parent?
    /// </summary>
    public struct Style
    {
        // Todo research alternative to nullable
        // value orderings for packing? 

        [Flags]
        public enum StyleFlags : uint
        {
            /// <summary>
            /// 0 = horizontal, 1 = vertical
            /// </summary>
            Direction,
            /// <summary>
            /// 0 = parent directed, 1 = self directed
            /// </summary>
            SelfDirected,


        }

        public bool Vertical
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Flags.HasFlag(StyleFlags.Direction);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { Flags = value ? Flags |= StyleFlags.Direction : Flags &= ~StyleFlags.Direction;  }
        }

        public bool SelfDirected
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Flags.HasFlag(StyleFlags.SelfDirected);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { Flags = value ? Flags |= StyleFlags.SelfDirected : Flags &= ~StyleFlags.SelfDirected; }
        }

        public StyleFlags Flags;

        public ElementValue Width;
        public ElementValue Height;

        /// <summary>
        /// Padding
        /// </summary>
        public ElementValue innerSpacing;

        /// <summary>
        /// outer spacing / Margin
        /// </summary>
        public ElementValue outerSpacing;

        public Color color;
        public Color background_color;

        public int font;
        public int background_texture;

        public Style() 
        {
            color = Color.Black;
            background_color = Color.White;
        }

        public void Combine(Style style)
        {
            throw new NotImplementedException();
        }
    }
}
