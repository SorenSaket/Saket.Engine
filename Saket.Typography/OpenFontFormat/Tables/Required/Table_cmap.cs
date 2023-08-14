using Saket.Typography.OpenFontFormat;
using Saket.Typography.OpenFontFormat.Serialization;

namespace Saket.Typography.OpenFontFormat.Tables.Required
{
    // Freetype implementations of cmap table fomats:
    // https://github.com/freetype/freetype/blob/29818e7ab436ae50d47c5e5a1cee41c5c12d5d69/src/sfnt/ttcmap.c

    // Apple Truetype Reference Manual
    // https://developer.apple.com/fonts/TrueType-Reference-Manual/RM06/Chap6cmap.html

    // The language field must be set to zero for all cmap subtables whose platform IDs are other than Macintosh (platform  ID  1).


    /// <summary>
    /// Character to glyph mapping. Defines the mapping of character codes to a default glyph index. 
    /// Reference: <see href="https://learn.microsoft.com/typography/opentype/spec/cmap"/>
    /// </summary>
    public class Table_cmap : Table
    {
        public override uint Tag => 0x636d6170;

        #region Character Map Formats


        /// <summary>
        /// The first uint16: format should not be read/skipped by the individual character map. 
        /// However when writing they should.
        /// </summary>
        public abstract class CharacterMap : IOFFSerializable
        {
            public abstract ushort format { get; }
            public ushort language;
            public ushort length;

            public abstract void Serialize(OFFWriter writer);
            public abstract void Deserialize(OFFReader reader);

            /// <summary>
            /// Returns a dictionary mapping charactercodes to glyph indexes.
            /// </summary>
            /// <returns></returns>
            public abstract Dictionary<int, int> MapToDictionary();
        }

        /// <summary>
        /// This is a simple 1 to 1 mapping of character codes to glyph indices.  
        /// The glyph set is limited to 256. If this format is used to index into a larger glyph set, only the first 256 glyphs will be accessible.
        /// </summary>
        public class CharacterMapFormat0 : CharacterMap
        {
            public override ushort format => 0;

            /// <summary>
            /// Must always have a length of 256
            /// </summary>
            public byte[] glyphIdArray;

            public override Dictionary<int, int> MapToDictionary() => mapping;


            Dictionary<int, int> mapping = new();

            public override void Serialize(OFFWriter writer)
            {
                throw new NotImplementedException();
            }

            public override void Deserialize(OFFReader reader)
            {
                reader.LoadBytes(4);
                reader.ReadUInt16(ref length);
                reader.ReadUInt16(ref language);
                reader.LoadBytes(length);
                glyphIdArray = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    reader.ReadUInt8(ref glyphIdArray[i]);
                }
            }

        }
        /// <summary>
        /// Format 2: High byte mapping through table
        /// </summary>
        public class CharacterMapFormat2
        {
            public ushort format;
            public ushort length;
            public ushort language;
            public byte[] glyphIdArray;

            public struct SubHeader
            {
                public ushort firstCode;
                public ushort entryCount;
                public short idDelta;
                public ushort idRangeOffset;
            }
        }
        /// <summary>
        /// Format 4: Segment mapping to delta values.
        /// 
        /// Each segment is described by a startCode, an endCode, an idDelta and an idRangeOffset. These are used for mapping the character codes in the segment. The segments are sorted in order of increasing endCode values.
        /// 
        /// Type 4 cmaps are required for backwards compatibility in Windows and are generally useful for BMP-only Unicode fonts. The redundancies in the header are for historical purposes—by pre-calculating these values, the performance of the lookup algorithm was substantially improved on older, slower processors.
        /// </summary>
        public class CharacterMapFormat4 : CharacterMap
        {
            public override ushort format => 4;

            /// <summary> 2 x segCount </summary>
            public ushort segCountX2;
            /// <summary> 2 x (2**floor(log2(segCount))) </summary>
            public ushort searchRange;
            /// <summary> log2(searchRange/2)  </summary>
            public ushort entrySelector;
            /// <summary> 2 x segCount - searchRange </summary>
            public ushort rangeShift;

            /// <summary> End characterCode for each segment, last=0xFFFF. </summary>
            public ushort[] endCode;
            /// <summary> Start character code for each segment. </summary>
            public ushort[] startCode;
            /// <summary> Delta for all character codes in segment. </summary>
            public short[] idDelta;
            /// <summary> Offsets into glyphIdArray or 0. </summary>
            public ushort[] idRangeOffset;

            /// <summary> Glyph index array (arbitrary length). </summary>
            public ushort[] glyphIdArray;

            public override Dictionary<int, int> MapToDictionary() => mapping;


            Dictionary<int, int> mapping = new();


            public override void Serialize(OFFWriter writer)
            {
                throw new NotImplementedException();
            }

            public override void Deserialize(OFFReader reader)
            {
                reader.LoadBytes(14);
                reader.ReadUInt16(ref length);
                reader.ReadUInt16(ref language);
                reader.ReadUInt16(ref segCountX2);
                reader.ReadUInt16(ref searchRange);
                reader.ReadUInt16(ref entrySelector);
                reader.ReadUInt16(ref rangeShift);

                int segmentCount = segCountX2 / 2;

                reader.LoadBytes(segmentCount * 8 + 2);

                endCode = new ushort[segmentCount];
                for (int i = 0; i < segmentCount; i++)
                    reader.ReadUInt16(ref endCode[i]);

                reader.Advance(2);     // padding

                startCode = new ushort[segmentCount];
                for (int i = 0; i < segmentCount; i++)
                    reader.ReadUInt16(ref startCode[i]);

                idDelta = new short[segmentCount];
                for (int i = 0; i < segmentCount; i++)
                    reader.ReadInt16(ref idDelta[i]);

                // The length of the glyphIdArray can be calculated by taking the 
                // sum of ranges which idRangeOffset is not equals zero 
                int glyphIdArrayLength = 0;
                //var position_idRangeOffset = reader.BufferPosition;

                idRangeOffset = new ushort[segmentCount];
                for (int i = 0; i < segmentCount; i++)
                {
                    reader.ReadUInt16(ref idRangeOffset[i]);
                    if (idRangeOffset[i] != 0)
                    {
                        // endCode is inclusive so + 1
                        glyphIdArrayLength += endCode[i] - startCode[i] + 1;
                    }
                }
                //var position_glyphIdArray = reader.BufferPosition;

                reader.LoadBytes(glyphIdArrayLength * 2);
                glyphIdArray = new ushort[glyphIdArrayLength];
                for (int i = 0; i < glyphIdArrayLength; i++)
                {
                    reader.ReadUInt16(ref glyphIdArray[i]);
                }

                mapping = new();

                // Add all mapppings
                // https://stackoverflow.com/questions/57461636/how-to-correctly-understand-truetype-cmaps-subtable-format-4
                // iterate trough all segements
                for (int i = 0; i < segmentCount; i++)
                {
                    // If the idRangeOffset value for the segment is not 0, the mapping of the character codes relies on the glyphIndexArray. 
                    if (idRangeOffset[i] != 0)
                    {
                        var end = endCode[i];
                        var delta = idDelta[i];
                        for (int index = startCode[i]; index <= end; index++)
                        {
                            var idIndex = (idRangeOffset[i] - (segmentCount * 2 - i * 2) + sizeof(ushort)) / 2;
                            var glyphID = glyphIdArray[idIndex + index - startCode[i]] % ushort.MaxValue;

                            if (glyphID != 0)
                            {
                                var glyphIndex = (glyphID + delta) % ushort.MaxValue;
                                if (glyphIndex != 0)
                                    mapping.Add(index, glyphIndex);
                            }
                        }
                    }
                    else
                    {
                        var end = endCode[i];
                        var delta = idDelta[i];
                        for (int index = startCode[i]; index <= end; index++)
                        {
                            var glyphIndex = (index + delta) % ushort.MaxValue;

                            if (glyphIndex != 0)
                                mapping.Add(index, glyphIndex);
                        }
                    }
                }
            }

        }
        /// <summary>
        /// 
        /// </summary>
        public class CharacterMapFormat6
        {
            public ushort format;
            public ushort length;
            public ushort language;
            public ushort firstCode;
            public ushort tryCount;
            public ushort[] glyphIdArray;
        }
        /// <summary>
        /// 
        /// </summary>
        public class CharacterMapFormat8
        {
            public ushort format;
            public ushort reserved;
            public ushort length;
            public ushort language;
            public byte[] is32;
            public uint numGroups;



        }
        /// <summary>
        /// Format 10: Trimmed array
        /// </summary>
        public class CharacterMapFormat10
        {
            public ushort format;
            public ushort reserved;
            public uint length;
            public uint language;
            public uint startCharCode;
            public uint numChars;
            public ushort[] glyphs;
        }
        /// <summary>
        /// 
        /// </summary>
        public class CharacterMapFormat12
        {
            public ushort format;
            public ushort reserved;
            public uint length;
            public uint language;
            public uint numGroups;
            public MapGroup[] groups;
        }
        /// <summary>
        /// Format 13: Many-to-one range mappings 
        /// </summary>
        public class CharacterMapFormat13
        {
            public ushort format;
            public ushort reserved;
            public uint length;
            public uint language;
            public uint numGroups;
            public MapGroup[] groups;
        }
        /// <summary>
        /// Format 14: Unicode variation sequences
        /// </summary>
        public class CharacterMapFormat14
        {
            public ushort format;
            public uint length;
            public uint numVarSelectorRecords;
        }

        #endregion

        #region Records

        public struct VariationSelector
        {
            /// <summary>
            /// Variation selector. Stored as 24 bits
            /// </summary>
            public uint varSelctor;
            /// <summary>
            /// Offset  from the start of the format 14 subtable to Non-Default UVS Table. May be 0.
            /// </summary>
            public uint defaultUVSOffset;
            /// <summary>
            /// Offset from the start of the format 14 subtable to Default UVS Table. May be 0.
            /// </summary>
            public uint nonDefaultUVSOffset;
        }

        public struct DefaultUVSTable
        {
            public uint numUnicodeValueRanges;
            public UnicodeRange[] ranges;
        }

        public struct NonDefaultUVSTable
        {
            public uint numUVSMappings;
            public UVSMapping[] uvsMappings;
        }

        public struct UVSMapping
        {
            /// <summary>
            /// Stored as 24 bits
            /// </summary>
            public uint unicodeValue;
            public ushort[] glyphID;
        }

        public struct UnicodeRange
        {
            /// <summary>
            /// stored as 24 bits
            /// </summary>
            public uint startUnicodeValue;
            /// <summary>
            /// Number of additional values in this range
            /// </summary>
            public byte additionalCount;
        }

        /// <summary>
        /// Common struct for both SequentialMapGroup and ConstantMapGroup 
        /// </summary>
        public struct MapGroup
        {
            /// <summary>
            /// First character code in this group; note that if this group is for one or 
            /// more  16-bit character  codes (which  is  determined from  the is32 array), 
            /// this 32-bit value will have the high 16-bits set to zero.
            /// </summary>
            public uint startCharCode;
            /// <summary>
            /// Last character code in this group; same condition as for the startCharCode
            /// </summary>
            public uint endCharCode;
            /// <summary>
            /// Glyph index corresponding to the starting character code.
            /// </summary>
            public uint startGlyphID;
        }


        #endregion

        public ushort version;
        public ushort numTables;
        public EncodingRecord[] encodingRecords;
        public CharacterMap[] characterMaps;


        public override void Serialize(OFFWriter writer)
        {

        }

        public override void Deserialize(OFFReader reader)
        {
            long beginAt = reader.stream.Position;

            reader.LoadBytes(4);
            reader.ReadUInt16(ref version);
            reader.ReadUInt16(ref numTables);

            reader.LoadBytes(8 * numTables);
            encodingRecords = new EncodingRecord[numTables];

            for (int i = 0; i < numTables; i++)
            {
                ushort platformID = 0;
                reader.ReadUInt16(ref platformID);
                encodingRecords[i].platformID = (PlatformID)platformID;

                reader.ReadUInt16(ref encodingRecords[i].encodingID);
                reader.ReadUInt32(ref encodingRecords[i].offset);
            }

            characterMaps = new CharacterMap[numTables];
            for (int i = 0; i < numTables; i++)
            {
                reader.stream.Seek(beginAt + encodingRecords[i].offset, SeekOrigin.Begin);
                reader.LoadBytes(2);
                ushort format = 0;
                reader.ReadUInt16(ref format);

                switch (format)
                {
                    case 4:
                        characterMaps[i] = new CharacterMapFormat4();
                        characterMaps[i].Deserialize(reader);
                        break;
                        /*default:
                            throw new NotImplementedException("Cmap subformat not supported");*/
                }
            }
        }
    }
}
