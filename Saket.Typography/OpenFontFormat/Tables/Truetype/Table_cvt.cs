using System;
using Saket.Typography.OpenFontFormat;
using Saket.Typography.OpenFontFormat.Serialization;

namespace Saket.Typography.OpenFontFormat.Tables.Truetype
{
    /// <summary>
    /// Control Value Table. 
	/// This table contains a list of values that can be referenced by instructions. They can be used, among other
	/// things, to control characteristics for different glyphs. The length of the table must be an integral number of FWORD units. 
    /// </summary>
    public class Table_cvt : Table
    {
        public override uint Tag => 0x63767420;

        public int n;
        public short[] data;
        public Table_cvt(int n)
        {
            this.n = n;
        }
        public override void Deserialize(OFFReader reader)
        {
            data = new short[n];
            reader.LoadBytes(2 * n);
            for (int i = 0; i < n; i++)
            {
                reader.ReadInt16(ref data[i]);
            }
        }

        public override void Serialize(OFFWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}