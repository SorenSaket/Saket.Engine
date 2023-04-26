using System;
using System.Collections.Generic;
using System.IO;
using Saket.Engine.Typography.OpenFontFormat.Serialization;

namespace Saket.Engine.Filetypes.Font.OpenFontFormat.Tables
{
    // Freetype implementations of cmap table fomats:
    // https://github.com/freetype/freetype/blob/29818e7ab436ae50d47c5e5a1cee41c5c12d5d69/src/sfnt/ttcmap.c
    //

    // The language field must be set to zero for all cmap subtables whose platform IDs are other than Macintosh (platform  ID  1).
    //
    //
    //
    //
    //


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
			public abstract UInt16 format {get;}
			public UInt16 language;
            public UInt16 length;

			
			public abstract void Serialize(OFFWriter reader);
			public abstract void Deserialize(OFFReader reader);

			/// <summary>
			/// Lookup a single index from character code.
			/// If the glyph does not exists 0 returns.
			/// </summary>
			/// <param name="characterCode"></param>
			/// <returns></returns>
			public abstract uint Lookup (uint characterCode);
			/// <summary>
			/// Returns a dictionary mapping charactercodes to glyph indexes.
			/// </summary>
			/// <param name="reader"></param>
			/// <returns></returns>
			public abstract Dictionary<int,int> MapToDictionary(OFFReader reader);
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

			public override uint Lookup(uint index)
			{
				if(index > byte.MaxValue)
					return 0;

				return glyphIdArray[index];
			}

			public override void Serialize(OFFWriter reader)
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

			public override Dictionary<int, int> MapToDictionary(OFFReader reader)
			{
				throw new NotImplementedException();
			}
		}

        public struct CharacterMapFormat2
        {
            public UInt16 format;
            public UInt16 length;
            public UInt16 language;
            public byte[] glyphIdArray;

            public struct SubHeader
            {
                public UInt16 firstCode;
                public UInt16 entryCount;
                public Int16 idDelta;
                public UInt16 idRangeOffset;
            }
        }

		/// <summary>
		/// Format 4: Segment mapping to delta values.
		/// 
		/// </summary>
        public class CharacterMapFormat4 : CharacterMap

        {
			public override UInt16 format => 4; 

			/// <summary> 2 x segCount </summary>
            public UInt16 segCountX2;
			/// <summary> 2 x (2**floor(log2(segCount))) </summary>
            public UInt16 searchRange;
			/// <summary> log2(searchRange/2)  </summary>
			public UInt16 entrySelector;
			/// <summary> 2 x segCount - searchRange </summary>
            public UInt16 rangeShift;
			/// <summary> End characterCode for each segment, last=0xFFFF. </summary>
            public UInt16[] endCode;
			/// <summary> Start character code for each segment. </summary>
            public UInt16[] startCode;
			/// <summary> Delta for all character codes in segment. </summary>
            public Int16[] idDelta;
			/// <summary> Offsets into glyphIdArray or 0. </summary>
            public UInt16[] idRangeOffset;
			/// <summary> Glyph index array (arbitrary length). </summary>
            public UInt16[] glyphIdArray;
        
			public override void Serialize(OFFWriter reader)
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

				int segmentCount = segCountX2/2;

				reader.LoadBytes(segmentCount*8+2);

   				endCode = new ushort[segmentCount];
				for (int i = 0; i < segmentCount; i++)
                	reader.ReadUInt16(ref endCode[i]);

				reader.Advance(2);     // padding

				startCode =  new ushort[segmentCount];
				for (int i = 0; i < segmentCount; i++)
					reader.ReadUInt16(ref startCode[i]);

				idDelta =  new short[segmentCount];
				for (int i = 0; i < segmentCount; i++)
					reader.ReadInt16(ref idDelta[i]);

				// The length of the glyphIdArray can be calculated by taking the 
				//  sum of ranges which idRangeOffset is not equals zero 
				int glyphIdArrayLength = 0;


				idRangeOffset =  new ushort[segmentCount];
				for (int i = 0; i < segmentCount; i++){
					reader.ReadUInt16(ref idRangeOffset[i]);
					if(idRangeOffset[i] != 0)
					{
						// endCode is inclusive so + 1
						glyphIdArrayLength += endCode[i]-startCode[i]+1;
					}
				}

				reader.LoadBytes(glyphIdArrayLength*2);
				glyphIdArray = new ushort[glyphIdArrayLength];
				for (int i = 0; i < glyphIdArrayLength; i++){
					reader.ReadUInt16(ref glyphIdArray[i]);
				}
			}

			public override uint Lookup (uint characterCode)
			{
				throw new NotImplementedException();
			}
			public override Dictionary<int,int> MapToDictionary(OFFReader reader)
			{
				throw new NotImplementedException();
			}
/*
			public override uint Lookup (uint characterCode)
			{
				if(characterCode > ushort.MaxValue)
					return 0;

				
				int i = Array.BinarySearch(endCode, (ushort)characterCode);
				if(i < 0)
					return 0;
				if (startCode[i] > characterCode)
				{
					return 0;
				}
  			
				
				if (idRangeOffset[i] == 0)
				{
					return (ushort)((characterCode + idDelta[i]) % 65536);
				}
				else
				{
					
				}



				
			}
			public override Dictionary<int,int> MapToDictionary(OFFReader reader)
			{
				Dictionary<int,int> r = new();
				// Add all mapppings
				// iterate trough all segements
				for (int i = 0; i < segCountX2/2; i++)
				{
					// If this range uses glyphIdArray for lookup
					if(idRangeOffset[i] != 0)
					{
						var currentOffset = reader.stream.Position;
						return glyphIndexArray[i - segCount + idRangeOffset[i]/2 + (c - startCode[i])]
					}
					// Simple range mapping
					else
					{
						var end = endCode[i];
						var delta = idDelta[i];
						for (var index = startCode[i]; index <= end; index++) {
							var glyphIndex = (index + idDelta[i]) % ushort.MaxValue;

							if (glyphIndex != 0)
								r.Add(index, glyphIndex);
						}
					}
					reader.ReadUInt8(ref glyphIdArray[i]);
				}
			}*/


		}

        public struct CharacterMapFormat6
        {
            public UInt16 format;
            public UInt16 length;
            public UInt16 language;
            public UInt16 firstCode;
            public UInt16 tryCount;
            public UInt16[] glyphIdArray;
        }

        public struct CharacterMapFormat8
        {
            public UInt16 format;
            public UInt16 reserved;
            public UInt16 length;
            public UInt16 language;
            public byte[] is32;
            public UInt32 numGroups;


          
        }
        /// <summary>
        /// Format 10: Trimmed array
        /// </summary>
        public struct CharacterMapFormat10
        {
            public UInt16 format;
            public UInt16 reserved;
            public UInt32 length;
            public UInt32 language;
            public UInt32 startCharCode;
            public UInt32 numChars;
            public UInt16[] glyphs;
        }

        public struct CharacterMapFormat12
        {
            public UInt16 format;
            public UInt16 reserved;
            public UInt32 length;
            public UInt32 language;
            public UInt32 numGroups;
            public MapGroup[] groups;
        }
        /// <summary>
        /// Format 13: Many-to-one range mappings 
        /// </summary>
        public struct CharacterMapFormat13
        {
            public UInt16 format;
            public UInt16 reserved;
            public UInt32 length;
            public UInt32 language;
            public UInt32 numGroups;
            public MapGroup[] groups;
        }

        /// <summary>
        /// Format 14: Unicode variation sequences
        /// </summary>
        public struct CharacterMapFormat14
        {
            public UInt16 format;
            public UInt32 length;
            public UInt32 numVarSelectorRecords;
        }

        #endregion

        #region Records

        public struct VariationSelector
        {
            /// <summary>
            /// Variation selector. Stored as 24 bits
            /// </summary>
            public UInt32 varSelctor;
            /// <summary>
            /// Offset  from the start of the format 14 subtable to Non-Default UVS Table. May be 0.
            /// </summary>
            public UInt32 defaultUVSOffset;
            /// <summary>
            /// Offset from the start of the format 14 subtable to Default UVS Table. May be 0.
            /// </summary>
            public UInt32 nonDefaultUVSOffset;
        }

        public struct DefaultUVSTable
        {
            public UInt32 numUnicodeValueRanges;
            public UnicodeRange[] ranges;
        }

        public struct NonDefaultUVSTable
        {
            public UInt32 numUVSMappings;
            public UVSMapping[] uvsMappings;
        }

        public struct UVSMapping
        {
            /// <summary>
            /// Stored as 24 bits
            /// </summary>
            public UInt32 unicodeValue;
            public UInt16[] glyphID;
        }

        public struct UnicodeRange
        {
            /// <summary>
            /// stored as 24 bits
            /// </summary>
            public UInt32 startUnicodeValue;
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
            public UInt32 startCharCode;
            /// <summary>
            /// Last character code in this group; same condition as for the startCharCode
            /// </summary>
            public UInt32 endCharCode;
            /// <summary>
            /// Glyph index corresponding to the starting character code.
            /// </summary>
            public UInt32 startGlyphID;
        }


        #endregion


        ushort version;
        ushort numTables;
        EncodingRecord[] encodingRecords;
		CharacterMap[] characterMaps;


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
					default:
						throw new NotImplementedException("Other cmap subformats coming soon.");
				}
            }
        }
        
    }
}
