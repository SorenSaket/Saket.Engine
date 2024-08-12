
using Saket.Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Saket.Engine.GUI.Styling
{
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
        /// <summary>
        /// 0 = nowrap, 1 = wrap
        /// </summary>
        Wrap,
    }

    public enum AlignItems : byte
    {
        start,
        end,
        center,
        stretch
    }

    public enum AlignContent : byte
    {
        start,
        end,
        center,
        stretch,
        space_between,
        space_around
    }

    /// <summary>
    /// Contains all styling for all GUI elements. 
    /// a zero/default value means that it's unset. should inhert values from parent?
    /// </summary>
    public struct Style
    {
        // Todo research alternative to nullable
        // value orderings for packing? 

        #region Flag Accesors

        public bool Wrap
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Flags.HasFlag(StyleFlags.Wrap) ? true : false;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { Flags = value ? Flags |= StyleFlags.Wrap : Flags &= ~StyleFlags.Wrap; }
        }

        /// <summary>
        /// Which Direction to lay out children
        /// </summary>
        public Axis Axis
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Flags.HasFlag(StyleFlags.Direction) ? Axis.Horizontal : Axis.Vertical;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { Flags = value == Axis.Horizontal ? Flags |= StyleFlags.Direction : Flags &= ~StyleFlags.Direction;  }
        }

        public bool SelfDirected
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Flags.HasFlag(StyleFlags.SelfDirected);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { Flags = value ? Flags |= StyleFlags.SelfDirected : Flags &= ~StyleFlags.SelfDirected; }
        }

        #endregion


        public StyleFlags Flags;

        public AlignItems AlignItems;
        public AlignContent AlignContent;


        // 

        public ElementValue Width;
        public ElementValue Height;

        public ElementValue MinWidth;
        public ElementValue MinHeight;
        
        public ElementValue MaxWidth;
        public ElementValue MaxHeight;


        public ElementValue X;
        public ElementValue Y;

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
