using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Typography.TrueType
{



    /// <summary>
    /// Any point the font scaler interpreter references is in one of two zones, that is one of two
    /// sets of points that potentially make up a glyph description.The first of these referenced
    /// zones is zone 1 (Z1) and always contains the glyph currently being interpreted.
    /// The second, zone 0 (Z0), is used for temporary storage of point coordinates that do not
    /// correspond to any actual points in the glyph in zone 1
    /// </summary>
    internal struct Zone
    {
        public PointF[] Current;
        public PointF[] Original;
        public TouchState[] TouchState;
        /// <summary>
        /// Whether this zone is twilight ie. used for intermediate points.
        /// </summary>
        public bool IsTwilight;

        public Zone(PointF[] points, bool isTwilight)
        {
            IsTwilight = isTwilight;
            Current = points;
            Original = (PointF[])points.Clone();
            TouchState = new TouchState[points.Length];
        }

        public Vector2 GetCurrent(int index) => Current[index].P;
        public Vector2 GetOriginal(int index) => Original[index].P;
    }
}
