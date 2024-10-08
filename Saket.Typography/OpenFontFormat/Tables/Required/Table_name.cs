using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Saket.Typography.OpenFontFormat;
using Saket.Typography.OpenFontFormat.Serialization;

namespace Saket.Typography.OpenFontFormat.Tables.Required
{
    /// <summary>
    /// Horizontal metrics 
    /// </summary>
    public class Table_name : Table
    {
        public struct NameRecord
        {
            /// <summary> Platform ID. </summary>
            public ushort platformID;
            /// <summary> Platform-specific encoding ID. </summary>
            public ushort encodingID;
            /// <summary> Language ID. </summary>
            public ushort languageID;
            /// <summary> Name ID. </summary>
            public ushort nameID;
            /// <summary> String length (in bytes). </summary>
            public ushort length;
            /// <summary> String offset from start of storage area (in bytes). </summary>
            public ushort offset;

        }

        public struct LangTagRecord
        {
            /// <summary> Language-tag string length (in bytes).  </summary>
            public ushort length;
            /// <summary> Language-tag string offset from start of storage area (in bytes).  </summary>
            public ushort offset;
        }

        public override uint Tag => 0x6e616d65;

        /// <summary> Format selector. </summary>
        public ushort format;
        /// <summary> Number of name records. </summary>
        public ushort count;
        /// <summary> Offset to start of string storage (from start of table). </summary>
        public ushort stringOffset;
        /// <summary> The name records where count is the number of records. </summary>
        public NameRecord[] nameRecord;
        /// <summary> Number of language-tag records. </summary>
        public ushort langTagCount;
        /// <summary> The language-tag records where langTagCount is the number of records.  </summary>
        public LangTagRecord[] langTagRecord;

        public override void Deserialize(OFFReader reader)
        {
            reader.LoadBytes(6);
            reader.ReadUInt16(ref format);
            reader.ReadUInt16(ref count);
            reader.ReadUInt16(ref stringOffset);

            nameRecord = new NameRecord[count];
            reader.LoadBytes(12 * count);
            for (int i = 0; i < count; i++)
            {
                reader.ReadUInt16(ref nameRecord[i].platformID);
                reader.ReadUInt16(ref nameRecord[i].encodingID);
                reader.ReadUInt16(ref nameRecord[i].languageID);
                reader.ReadUInt16(ref nameRecord[i].nameID);
                reader.ReadUInt16(ref nameRecord[i].length);
                reader.ReadUInt16(ref nameRecord[i].offset);
            }

            if (format == 1)
            {
                reader.LoadBytes(2);
                reader.ReadUInt16(ref langTagCount);
                langTagRecord = new LangTagRecord[langTagCount];
                reader.LoadBytes(4 * langTagCount);
                for (int i = 0; i < langTagCount; i++)
                {
                    reader.ReadUInt16(ref langTagRecord[i].length);
                    reader.ReadUInt16(ref langTagRecord[i].offset);
                }

            }
            else if (format == 0)
            {

            }
            else
            {
                throw new Exception($"Invalid name table format {format}.");
            }
        }

        public override void Serialize(OFFWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}