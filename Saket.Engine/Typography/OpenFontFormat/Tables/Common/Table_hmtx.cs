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
    public class Table_hmtx : Table
	{
		public struct longHorMetric
		{
			/// <summary> Advance width, in font design units. </summary>
			public UInt16 advanceWidth;
			/// <summary> Glyph left side bearing, in font design units. </summary>
			public Int16 lsb;

			public longHorMetric(ushort advanceWidth, short lsb)
			{
				this.advanceWidth = advanceWidth;
				this.lsb = lsb;
			}
		}


		public override uint Tag => 0x686d7478;

		
		public UInt16 numberOfHMetrics;
		public UInt16 numGlyphs;
		public longHorMetric[] hMetrics;
		public Int16[] leftSideBearing;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="numGlyphs">The number of glyphs in the font. Obtained from maxp table. </param>
		/// <param name="numberOfHMetrics">Number of horizontal metrics. Obtained from hhea table. </param>
		public Table_hmtx(UInt16 numGlyphs, UInt16 numberOfHMetrics){
			this.numGlyphs = numGlyphs;
			this.numberOfHMetrics = numberOfHMetrics;
		}

		public override void Deserialize(OFFReader reader)
		{
			reader.LoadBytes(4*numberOfHMetrics);

			hMetrics = new longHorMetric[numberOfHMetrics];
			for (int i = 0; i < hMetrics.Length; i++)
			{
				reader.ReadUInt16(ref hMetrics[i].advanceWidth);
				reader.ReadInt16(ref hMetrics[i].lsb);
			}

			leftSideBearing = new Int16[numGlyphs- numberOfHMetrics];
			reader.LoadBytes(2*leftSideBearing.Length);
			for (int i = 0; i < leftSideBearing.Length; i++)
			{
				reader.ReadInt16(ref leftSideBearing[i]);
			}
		}

		public override void Serialize(OFFWriter reader)
		{
			throw new NotImplementedException();
		}
	}
}
