using System;
using Saket.Engine.Typography.OpenFontFormat.Serialization;

namespace Saket.Engine.Filetypes.Font.OpenFontFormat.Tables
{
    /// <summary>
    /// loca – Index to location
	/// The loca table stores the offsets to the locations of the glyphs in the font, relative to the beginning of the glyf
	/// table. In order to compute the length of the last glyph element, there is an extra entry after the last valid index. 
    /// </summary>
    public class Table_loca : Table
    {
		public override uint Tag => 0x6c6f6361;

		public int n;
		public Table_head.IndexToLocFormat locFormat;
		public UInt16[]? offsets_short;
		public UInt32[]? offsets_long;

		public Table_loca(int numGlyphs, Table_head.IndexToLocFormat locFormat)
        {
			this.locFormat = locFormat;
			this.n = numGlyphs+1;
        }
        public override void Deserialize(OFFReader reader)
        {
			if(locFormat == Table_head.IndexToLocFormat.@short)
			{
				offsets_short = new UInt16[n];
                offsets_long = null;
                reader.LoadBytes(n*2);
				for (int i = 0; i < n; i++)
				{
					reader.ReadOffset16(ref offsets_short[i]);
				}
			}
			else
			{
				offsets_short = null;
				offsets_long = new UInt32[n];
				reader.LoadBytes(n*4);
				for (int i = 0; i < n; i++)
				{
					reader.ReadOffset32(ref offsets_long[i]);
				}
			}
		
        }

        public override void Serialize(OFFWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}