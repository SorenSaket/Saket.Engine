using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saket.Typography.OpenFontFormat.Serialization;

namespace Saket.Typography.OpenFontFormat.Tables.Required
{
    public class Table_offset : IOFFSerializable
    {
        public enum SFNTVersion : uint
        {
            TrueType = 0x00010000,
            CFF = 0x4F54544F
        }

        public uint sfntVersion;

        /// <summary>
        /// A tag to indicate the OFA scaler to be used to rasterize this font.
        /// </summary>
        public uint scalarType;

        public ushort numTables;

        public ushort searchRange;

        public ushort entrySelector;

        public ushort rangeShift;

        public Table_offset(uint scalarType, ushort numTables, ushort searchRange, ushort entrySelector, ushort rangeShift)
        {
            this.scalarType = scalarType;
            this.numTables = numTables;
            this.searchRange = searchRange;
            this.entrySelector = entrySelector;
            this.rangeShift = rangeShift;
        }

        public Table_offset(uint scalarType, ushort numTables)
        {
            this.scalarType = scalarType;
            this.numTables = numTables;

            searchRange = 2;
            while (searchRange < numTables)
            {
                searchRange *= 2;
            }
            entrySelector = (ushort)MathF.Log2(searchRange);
            searchRange *= 16;
            rangeShift = (ushort)(numTables * 16 - searchRange);
        }

        public Table_offset()
        {
        }

        public void Serialize(OFFWriter writer)
        {

        }

        public void Deserialize(OFFReader reader)
        {
            reader.LoadBytes(12);
            reader.ReadUInt32(ref sfntVersion);
            reader.ReadUInt16(ref numTables);

            reader.ReadUInt16(ref searchRange);
            reader.ReadUInt16(ref entrySelector);
            reader.ReadUInt16(ref rangeShift);
        }
    }

}
