using System;

namespace Saket.Engine.Filetypes.Font.OpenFontFormat.Tables
{
    /// <summary>
    /// Font program.
	/// This table is similar to the CV Program, except that it is only run once, when the font is first used. It is used
	/// only for FDEFs and IDEFs. Thus the CV Program need not contain function definitions. However, the CV Program may redefine existing FDEFs or IDEFs. 
    /// </summary>
    public class Table_fpgm : Table
    {
		public override uint Tag => 0x6670676d;

		public int n;
		public byte[] data;
		public Table_fpgm(int n)
        {
			this.n = n;
        }
        public override void Deserialize(OFFReader reader)
        {
			data = new byte[n];
			reader.LoadBytes(n);
            for (int i = 0; i < n; i++)
			{
				reader.ReadUInt8(ref data[i]);
			}
        }

        public override void Serialize(OFFWriter reader)
        {
            throw new NotImplementedException();
        }
    }
}