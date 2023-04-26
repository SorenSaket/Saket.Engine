using System;

namespace Saket.Engine.Graphics.SDF
{
    /// <summary>
    ///  Edge color specifies which color channels an edge belongs to.
    /// </summary>
    [Flags]
    public enum EdgeColor : byte
    {
        Black   = 0,
        Red     = 1,
        Green   = 2,
        Blue    = 4,
        Yellow  = Red & Green,
        Magenta = Red & Blue,
        Cyan    = Green & Blue,
        White   = Red & Green & Blue,
    }
}