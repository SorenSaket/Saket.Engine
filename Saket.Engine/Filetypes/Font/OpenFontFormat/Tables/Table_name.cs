using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Filetypes.Font.OpenFontFormat.Tables
{
	/// <summary>
	/// Horizontal metrics 
	/// </summary>
	public class Table_name : Table
	{
		public struct NameRecord
		{

			
		}




		public override uint Tag => 0x6e616d65;

		/// <summary> Format selector. </summary>
		public UInt16 format;
		/// <summary> Number of name records. </summary>
		public UInt16 count;
		/// <summary> Offset to start of string storage (from start of table). </summary>
		public UInt16 stringOffset;



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