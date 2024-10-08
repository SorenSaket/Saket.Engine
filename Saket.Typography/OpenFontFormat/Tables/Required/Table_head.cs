﻿using Saket.Typography.OpenFontFormat;
using Saket.Typography.OpenFontFormat.Serialization;
using System;


namespace Saket.Typography.OpenFontFormat.Tables.Required
{
    /// <summary>
    /// This  table  gives  global  information  about  the  font.  The  bounding  box  values  should  be  computed  using  only 
    /// glyphs that have contours.Glyphs with no contours should be ignored for the purposes of these calculations.
    /// </summary>
    public class Table_head : Table
    {
        #region Internal Types
        [Flags]
        public enum Flags : ushort
        {
            /// <summary>
            /// Baseline for font at y=0
            /// </summary>
            BaselineYZero = 0,
            /// <summary>
            /// Left sidebearing point at x=0 (relevant only for TrueType rasterizers) 
            /// </summary>
            SidebearingXZero = 1,
            /// <summary>
            /// Instructions may depend on point size
            /// </summary>
            PointSizeDependentInstructions = 2,
            /// <summary>
            /// Force ppem to integer values for all internal scaler math; may use fractional ppem sizes if this bit is clear;
            /// </summary>
            ForceInterger = 3,
            /// <summary>
            /// Font data is ‘lossless’ as a result of having been 
            /// subjected to optimizing transformation and/or
            /// compression (such as font compression mechanisms
            /// defined by ISO/IEC 14496-18, MicroType®5 Express, 
            /// WOFF2[24], or similar) where the original font
            /// functionality and features are retained but the binary
            /// compatibility between input and output font files is not
            /// guaranteed. As a result of the applied transform, the
            /// ‘DSIG’ Table may also be invalidated.
            /// </summary>
            Lossless = 11,
            /// <summary>
            /// Font converted (produce compatible metrics).
            /// </summary>
            Converted = 12,
            /// <summary>
            /// Font optimized for ClearType®. Note, fonts that  rely on embedded bitmaps (EBDT) for rendering should
            /// not be considered optimized for ClearType, and therefore should keep this bit cleared.
            /// </summary>
            ClearTypeOptimized = 13,
            /// <summary>
            /// Last Resort font. If set, indicates that the glyphs encoded in the cmap subtables are simply generic
            /// symbolic representations of code point ranges and don’t
            /// truly represent support for those code points. If unset,
            /// indicates that the glyphs encoded in the cmap subtables
            /// represent proper support for those code points.
            /// </summary>
            LastResortFont = 14,
        }
        [Flags]
        public enum MacStyle : ushort
        {
            Bold = 0,
            Italic = 1,
            Underline = 2,
            Outline = 3,
            Shadow = 4,
            Condensed = 5,
            Extended = 6
        }

        /// <summary>
        /// [Obsolete("Deprecated. Set to 2.")]
        /// </summary>
        public enum FontDirectionHint : short
        {
            FullyMixedDirectionalGlyphs = 0,
            OnlyStronglyLeftToRight = 1,
            Like1ButAlsoContainsNeutrals = 2,
            OnlyStronglyRightToLeft = -1,
            LikeMinus1ButAlsoContainsNeutrals = -2
        }

        public enum IndexToLocFormat : short
        {
            @short = 0,
            @long = 1
        }

        #endregion

        /// <summary>
        /// Major version number of the font header table – set to 1.
        /// </summary>
        public ushort majorVersion = 1;
        /// <summary>
        /// Minor version number of the font header table – set to 0.
        /// </summary>
        public ushort minorVersion = 0;
        /// <summary>
        /// Set by font manufacturer.
        /// </summary>
        public uint fontRevision;
        /// <summary>
        /// To compute: set it to 0, sum the entire font as uint32,
        /// then store 0xB1B0AFBA - sum. If the font is used as a
        /// component in a font collection file, the value of this field
        /// will be invalidated by changes to the file structure and
        /// font table directory, and must be ignored.
        /// </summary>
        public uint checkSumAdjustment;
        /// <summary>
        /// Set to 0x5F0F3CF5. 
        /// </summary>
        public uint magicNumber = 0x5F0F3CF5;
        /// <summary>
        /// Flags
        /// </summary>
        public Flags flags;
        /// <summary>
        /// Set to a value from 16 to 16384. Any value in this range 
        /// is valid.In fonts that have TrueType outlines, a power of 2 
        /// is recommended as this allows performance optimization in some rasterizers.
        /// </summary>
        public ushort unitsPerEm;
        /// <summary>
        /// Number  of  seconds  since  12:00  midnight  that  started January 1st, 1904, in GMT/UTC time zone. 64-bit integer
        /// </summary>
        public long created;
        /// <summary>
        /// Number  of  seconds  since  12:00  midnight  that  started January 1st, 1904, in GMT/UTC time zone. 64-bit integer
        /// </summary>
        public long modified;

        // For all glyph bounding boxes.

        public short xMin;
        public short yMin;
        public short xMax;
        public short yMax;

        /// <summary>
        /// Mac Style
        /// </summary>
        public MacStyle macStyle;
        /// <summary>
        /// Smallest readable size in pixels.
        /// </summary>
        public ushort lowestRecPPEM;

        public FontDirectionHint fontDirectionHint;

        public IndexToLocFormat indexToLocFormat;

        public short glyphDataFormat = 0;


        public override uint Tag => 0x68656164;

        public Table_head()
        {
        }


        public override void Deserialize(OFFReader reader)
        {
            ushort uv = 0;
            short v = 0;
            reader.LoadBytes(2 * 2 + 4 * 3 + 2 * 2 + 8 * 2 + 2 * 9);
            reader.ReadUInt16(ref majorVersion);
            reader.ReadUInt16(ref minorVersion);
            reader.ReadUInt32(ref fontRevision);
            reader.ReadUInt32(ref checkSumAdjustment);
            reader.ReadUInt32(ref magicNumber);

            reader.ReadUInt16(ref uv);
            flags = (Flags)uv;

            reader.ReadUInt16(ref unitsPerEm);
            reader.ReadLONGDATETIME(ref created);
            reader.ReadLONGDATETIME(ref modified);
            reader.ReadInt16(ref xMin);
            reader.ReadInt16(ref yMin);
            reader.ReadInt16(ref yMin);
            reader.ReadInt16(ref yMax);


            reader.ReadUInt16(ref uv);
            macStyle = (MacStyle)v;

            reader.ReadUInt16(ref lowestRecPPEM);

            reader.ReadInt16(ref v);
            fontDirectionHint = (FontDirectionHint)v;

            reader.ReadInt16(ref v);
            indexToLocFormat = (IndexToLocFormat)v;

            reader.ReadInt16(ref glyphDataFormat);
        }

        public override void Serialize(OFFWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}