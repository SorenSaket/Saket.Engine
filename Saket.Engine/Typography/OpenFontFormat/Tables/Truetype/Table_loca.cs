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
		public UInt32[] offsets;


		public uint GetLocation(int index)
		{
			return offsets[index];
        }

		public Table_loca(int numGlyphs, Table_head.IndexToLocFormat locFormat)
        {
			this.locFormat = locFormat;
			this.n = numGlyphs+1;
			offsets= new uint[n];
        }
        public override void Deserialize(OFFReader reader)
        {
			if(locFormat == Table_head.IndexToLocFormat.@short)
			{
                reader.LoadBytes(n*2);
				ushort v = 0;

                for (int i = 0; i < n; i++)
				{
                    reader.ReadOffset16(ref v);
					offsets[i] = (uint)(v << 1);
                }
			}
			else
			{
				reader.LoadBytes(n*4);
				for (int i = 0; i < n; i++)
				{
					reader.ReadOffset32(ref offsets[i]);
				}
			}
		
        }

        public override void Serialize(OFFWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}