using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saket.Typography.OpenFontFormat;
using Saket.Typography.OpenFontFormat.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Saket.Typography.OpenFontFormat.Tables.Required
{
    /// <summary>
    /// Maximum profile . This table establishes the memory requirements for this font. Fonts with CFF data must use Version 0.5 of this
	/// table, specifying only the numGlyphs field.Fonts with TrueType outlines must use Version 1.0 of this table, where all data is required.
    /// </summary>
    public class Table_maxp : Table
    {
        public enum Version : uint
        {
            v05 = 0x00005000,
            v10 = 0x00010000
        }

        public override uint Tag => 0x6d617870;

        public uint version;
        /// <summary> The number of glyphs in the font.  </summary>
        public ushort numGlyphs;
        /// <summary> Maximum points in a non-composite glyph. </summary>
        public ushort maxPoints;
        /// <summary> Maximum contours in a non-composite glyph. </summary>
        public ushort maxContours;
        /// <summary> Maximum points in a composite glyph. </summary>
        public ushort maxCompositePoints;
        /// <summary> Maximum contours in a composite glyph. </summary>
        public ushort maxCompositeContours;
        /// <summary> 1 if instructions do not use the twilight zone (Z0), or 2 if instructions do use Z0; should be set to 2 in most cases.  </summary>
        public ushort maxZones;
        /// <summary> Maximum points used in Z0. </summary>
        public ushort maxTwilightPoints;
        /// <summary> Number of Storage Area locations. </summary>
        public ushort maxStorage;
        /// <summary> Number of FDEFs, equal to the highest function number + 1. </summary>
        public ushort maxFunctionDefs;
        /// <summary> Number of IDEFs. </summary>
        public ushort maxInstructionDefs;
        /// <summary> Maximum stack depth across Font Program ('fpgm' table), CVT Program ('prep' table) and all glyph instructions (in the 'glyf' table). </summary>
        public ushort maxStackElements;
        /// <summary> Maximum byte count for glyph instructions. </summary>
        public ushort maxSizeOfInstructions;
        /// <summary> Maximum number of components referenced at "top level" for any composite glyph.  </summary>
        public ushort maxComponentElements;
        /// <summary> Maximum levels of recursion; 1 for simple components. </summary>
        public ushort maxComponentDepth;

        public override void Deserialize(OFFReader reader)
        {
            reader.LoadBytes(6);
            reader.ReadUInt32(ref version);
            reader.ReadUInt16(ref numGlyphs);

            if (version == 0x0010000)
            {
                reader.LoadBytes(26);
                reader.ReadUInt16(ref maxPoints);
                reader.ReadUInt16(ref maxContours);
                reader.ReadUInt16(ref maxCompositePoints);
                reader.ReadUInt16(ref maxCompositeContours);
                reader.ReadUInt16(ref maxZones);
                reader.ReadUInt16(ref maxTwilightPoints);
                reader.ReadUInt16(ref maxStorage);
                reader.ReadUInt16(ref maxFunctionDefs);
                reader.ReadUInt16(ref maxInstructionDefs);
                reader.ReadUInt16(ref maxStackElements);
                reader.ReadUInt16(ref maxSizeOfInstructions);
                reader.ReadUInt16(ref maxComponentElements);
                reader.ReadUInt16(ref maxComponentDepth);
            }
        }

        public override void Serialize(OFFWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}