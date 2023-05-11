using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saket.Engine.Typography.OpenFontFormat.Serialization;

namespace Saket.Engine.Filetypes.Font.OpenFontFormat.Tables
{
    public class Table_offset : IOFFSerializable 
    {
        public enum SFNTVersion : UInt32
        {
            TrueType = 0x00010000,
            CFF = 0x4F54544F
        }

        public UInt32 sfntVersion;

        /// <summary>
        /// A tag to indicate the OFA scaler to be used to rasterize this font.
        /// </summary>
        public UInt32 scalarType;

        public UInt16 numTables;

        public UInt16 searchRange;

        public UInt16 entrySelector;

        public UInt16 rangeShift;

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
            rangeShift = (ushort)((numTables * 16) - searchRange);
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
