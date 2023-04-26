using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saket.Engine.Typography.OpenFontFormat.Serialization;

namespace Saket.Engine.Filetypes.Font.OpenFontFormat.Tables
{
    /// <summary>
    /// Horizontal metrics 
    /// </summary>
    public class Table_maxp : Table
	{
		public enum Version : UInt32
		{
			v05 = 0x00005000,
			v10 = 0x00010000
		}

		public override uint Tag => 0x6d617870;

		public UInt32 version;
		/// <summary> The number of glyphs in the font.  </summary>
		public UInt16 numGlyphs;
		/// <summary> Maximum points in a non-composite glyph. </summary>
		public UInt16 maxPoints;
		/// <summary> Maximum contours in a non-composite glyph. </summary>
		public UInt16 maxContours;
		/// <summary> Maximum points in a composite glyph. </summary>
		public UInt16 maxCompositePoints;
		/// <summary> Maximum contours in a composite glyph. </summary>
		public UInt16 maxCompositeContours;
		/// <summary> 1 if instructions do not use the twilight zone (Z0), or 2 if instructions do use Z0; should be set to 2 in most cases.  </summary>
		public UInt16 maxZones;
		/// <summary> Maximum points used in Z0. </summary>
		public UInt16 maxTwilightPoints;
		/// <summary> Number of Storage Area locations. </summary>
		public UInt16 maxStorage;
		/// <summary> Number of FDEFs, equal to the highest function number + 1. </summary>
		public UInt16 maxFunctionDefs;
		/// <summary> Number of IDEFs. </summary>
		public UInt16 maxInstructionDefs;
		/// <summary> Maximum stack depth across Font Program ('fpgm' table), CVT Program ('prep' table) and all glyph instructions (in the 'glyf' table). </summary>
		public UInt16 maxStackElements;
		/// <summary> Maximum byte count for glyph instructions. </summary>
		public UInt16 maxSizeOfInstructions;
		/// <summary> Maximum number of components referenced at "top level" for any composite glyph.  </summary>
		public UInt16 maxComponentElements;
		/// <summary> Maximum levels of recursion; 1 for simple components. </summary>
		public UInt16 maxComponentDepth;

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

		public override void Serialize(OFFWriter reader)
		{
			throw new NotImplementedException();
		}
	}
}