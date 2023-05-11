using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saket.Engine.Typography.OpenFontFormat.Serialization;

namespace Saket.Engine.Filetypes.Font.OpenFontFormat.Tables
{
    /// <summary>
    /// Horizontal header
    /// </summary>
    public class Table_hhea : Table
	{
		public override uint Tag => 0x68686561;

		/// <summary> Major version number of the horizontal header table – set to 1. </summary>
		public ushort majorVersion;
		/// <summary> Minor version number of the horizontal header table – set to 0. </summary>
		public ushort minorVersion;
		/// <summary> Typographic ascent. (Distance from baseline of highest ascender). </summary>
		public short acender;
		/// <summary> Typographic descent. (Distance from baseline of lowest descender).  </summary>
		public short decender;
		/// <summary> Typographic line gap. Negative lineGap values are treated as zero in in some legacy platform implementations. </summary>
		public short lineGap;
		/// <summary> Maximum advance width value in 'hmtx' table.  </summary>
		public ushort advanceWidthMax;
		/// <summary> Minimum left sidebearing value in 'hmtx' table. </summary>
		public short minLeftSideBearing;
		/// <summary> Minimum right sidebearing value; calculated as Min (aw - lsb - (xMax - xMin)).  </summary>
		public short minRightSideBearing;
		/// <summary> Max(lsb + (xMax - xMin)). </summary>
		public short xMaxExtent;
		/// <summary> Used to calculate the slope of the cursor (rise/run); 1 for vertical. </summary>
		public short caretSlopeRise;
		/// <summary>  0 for vertical. </summary>
		public short caretSlopeRun;
		/// <summary> The amount by which a slanted highlight on a glyph needs to be shifted to produce the best appearance. Set to 0 for non-slanted fonts. </summary>
		public short caretOffset;
		/// <summary>  0 for current format. </summary>
		public short metricDataFormat;
		/// <summary> Number of hMetric entries in 'hmtx' table  </summary>
		public ushort numberOfHMetrics;


		public override void Deserialize(OFFReader reader)
		{
			reader.LoadBytes(36);
			reader.ReadUInt16(ref majorVersion);
			reader.ReadUInt16(ref minorVersion);
			reader.ReadFWORD(ref acender);
			reader.ReadFWORD(ref decender);
			reader.ReadFWORD(ref lineGap);
			reader.ReadUFWORD(ref advanceWidthMax);
			reader.ReadFWORD(ref minLeftSideBearing);
			reader.ReadFWORD(ref minRightSideBearing);
			reader.ReadFWORD(ref xMaxExtent);
			reader.ReadInt16(ref caretSlopeRise);
			reader.ReadInt16(ref caretSlopeRun);
			reader.ReadInt16(ref caretOffset);
			reader.Advance(8); // Reserved
			reader.ReadInt16(ref metricDataFormat);
			reader.ReadUInt16(ref numberOfHMetrics);
		}

		public override void Serialize(OFFWriter writer)
		{
			throw new NotImplementedException();
		}
	}
}
