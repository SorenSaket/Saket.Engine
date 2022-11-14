using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Filetypes.Font.OpenFontFormat.Tables
{
    public class Table_directory : IOFFSerializable
    {
        public string tableName ;

        /// <summary>
        /// 4-byte identifier
        /// </summary>
        public Tag tag;

        /// <summary>
        /// Checksum for this table
        /// </summary>
        public UInt32 checksum;

        /// <summary>
        /// Offset from beginning of sfnt
        /// </summary>
        public UInt32 offset;

        /// <summary>
        /// Length of this table in byte (actual length not padded length)
        /// </summary>
        public UInt32 length;

        public Table_directory()
        {
        }

        /*
public Table_directory(uint tag, uint checksum, uint offset, uint length)
{
   this.tag = tag;
   tableName = new string(new char[] { Convert.ToChar((byte)(tag)), Convert.ToChar((byte)(tag >> 8)), Convert.ToChar((byte)(tag >> 16)), Convert.ToChar((byte)(tag >> 24)) });

   this.checksum = checksum;
   this.offset = offset;
   this.length = length;
}*/

        public  void Deserialize(OFFReader reader)
        {
            reader.LoadBytes(16);
            reader.ReadTag(ref tag);
            reader.ReadUInt32(ref checksum);
            reader.ReadOffset32(ref offset);
            reader.ReadUInt32(ref length);
			tableName = System.Text.Encoding.UTF8.GetString(new byte[]
			{
				(byte)(tag.value >> 24) ,
				(byte)(tag.value >> 16),
				(byte)(tag.value >> 8),
				(byte)(tag.value ),
			});
        }

        public  void Serialize(OFFWriter reader)
        {
            throw new NotImplementedException();
        }
    }
}
