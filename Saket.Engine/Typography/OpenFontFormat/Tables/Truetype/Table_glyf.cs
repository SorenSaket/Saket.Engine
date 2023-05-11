using System;
using Saket.Engine.Typography.OpenFontFormat.Serialization;

namespace Saket.Engine.Filetypes.Font.OpenFontFormat.Tables
{
    /// <summary>
    /// Glyf data.
	/// This table contains information that describes the glyphs in the font in the TrueType outline format. Information
	/// regarding the rasterizer (scaler) refers to the TrueType rasterizer. 
    /// </summary>
	/// <remarks>
	/// The 'glyf' table is comprised of a list of glyph data blocks, each of which provides the description for a single
	/// glyph. Glyphs are referenced by identifiers (glyph IDs), which are sequential integers beginning at zero. The
	/// total number of glyphs is specified by the numGlyphs field in the 'maxp' table. The 'glyf' table does not include
	/// any overall table header or records providing offsets to glyph data blocks. Rather, the 'loca' table provides an
	/// array of offsets, indexed by glyph IDs, which provide the location of each glyph data block within the 'glyf' table.
	/// Note that the 'glyf' table must always be used in conjunction with the 'loca' and 'maxp' tables. The size of each
	/// glyph data block is inferred from the difference between two consecutive offsets in the 'loca' table (with one
	/// extra offset provided to give the size of the last glyph data block). As a result of the 'loca' format, glyph data
	/// blocks within the 'glyf' table must be in glyph ID order. 
	/// </remarks>
    public class Table_glyf : Table
    {
		/// <summary>
		/// Each glyph begins with this header. 
		/// </summary>
		public struct GlyphHeader : IOFFSerializable
        {
			/// <summary>
			/// If the number of contours is greater than or equal to zero, this is a
			/// simple glyph; if negative, this is a composite glyph – the value -1
			/// should be used for composite glyphs. 
			/// </summary>
			public Int16 numberOfContours;
			/// <summary>
			/// Minimum x for coordinate data.
			/// </summary>
			public Int16 xMin;
			/// <summary>
			/// Minimum y for coordinate data. 
			/// </summary>
			public Int16 yMin;
			/// <summary>
			/// Maximum x for coordinate data. 
			/// </summary>
			public Int16 xMax;
			/// <summary>
			/// Maximum y for coordinate data. 
			/// </summary>
			public Int16 yMax;

            public void Deserialize(OFFReader reader)
            {
				reader.LoadBytes(10);
				reader.ReadInt16(ref numberOfContours);
				reader.ReadInt16(ref xMin);
				reader.ReadInt16(ref yMin);
				reader.ReadInt16(ref xMax);
				reader.ReadInt16(ref yMax);
            }

            public void Serialize(OFFWriter writer)
            {
                
            }
        }

		public struct SimpleGlyph
		{
			[Flags]
			public enum Flags : byte
			{
				/// <summary>
				/// Bit 0: If set, the point is on the curve; otherwise, it is off the curve.
				/// </summary>
				ON_CURVE_POINT = 0x01,
				/// <summary>
				/// Bit 1: If set, the corresponding x-coordinate is 1 byte long. If
				/// not set, it is two bytes long. For the sign of this value, see
				/// the description of the X_IS_SAME_OR_POSITIVE_X_SHORT_VECTOR flag
				/// </summary>
				X_SHORT_VECTOR = 0x02,
				/// <summary>
				/// Bit 2: If set, the corresponding y-coordinate is 1 byte long. If 
				/// not set, it is two bytes long. For the sign of this value, see
				/// the description of the Y_IS_SAME_OR_POSITIVE_Y_SHORT_VECTOR flag. 
				/// </summary>
				Y_SHORT_VECTOR = 0x04,
				/// <summary>
				/// Bit 3: If set, the next byte (read as unsigned) specifies the
				/// number of additional times this flag byte is to be repeated in
				/// the logical flags array – that is, the number of additional
				/// logical flag entries inserted after this entry. (In the expanded
				/// logical array this bit is ignored.) In this way, the number of
				/// flags listed can be smaller than the number of points in the glyph description. 
				/// </summary>
				REPEAT_FLAG = 0x08,
				/// <summary>
				/// Bit 4: This flag has two meanings, depending on how the 
				/// X_SHORT_VECTOR flag is set. If X_SHORT_VECTOR is
				/// set, this bit describes the sign of the value, with 1 equalling
				/// positive and 0 negative. If X_SHORT_VECTORt is not set
				/// and this bit is set, then the current x-coordinate is the same
				/// as the previous x-coordinate. If X_SHORT_VECTOR is not
				/// set and this bit is also not set, the current x-coordinate is a
				/// signed 16-bit delta vector. 
				/// </summary>
				X_IS_SAME_OR_POSITIVE_X_SHORT_VECTOR = 0x10,
				/// <summary>
				/// Bit 5: This flag has two meanings, depending on how the
				/// Y_SHORT_VECTOR flag is set. If Y_SHORT_VECTOR is
				/// set, this bit describes the sign of the value, with 1 equalling
				/// positive and 0 negative. If Y_SHORT_VECTOR is not set
				/// and this bit is set, then the current y-coordinate is the same
				/// as the previous y-coordinate. If Y_SHORT_VECTOR is not
				/// set and this bit is also not set, the current y-coordinate is a
				/// signed 16-bit delta vector. 
				/// </summary>
				Y_IS_SAME_OR_POSITIVE_Y_SHORT_VECTOR = 0x20,
				/// <summary>
				/// Bit 6: If set, contours in the glyph description may overlap.
				/// Use of this flag is not required in OFF – that is, it is valid to
				/// have contours overlap without having this flag set. It may
				/// affect behaviors in some platforms, however. (See Apple’s
				/// TrueType Reference Manual [7] for details regarding
				/// behavior in Apple platforms.) When used, it must be set on
				/// the first flag byte for the glyph.
				/// </summary>
				OVERLAP_SIMPLE = 0x40
			}

			/// <summary>
			/// Array of point indices for the last point of each contour; in increasing numeric order. 
			/// </summary>
			public UInt16[] endPtsOfContours;
			/// <summary>
			/// Total number of bytes for instructions. If instructionLength is zero, 
			/// no instructions are present for this glyph, and this field is 
			/// followed directly by the flags field. 
			/// </summary>
			public UInt16 instructionLength;
			/// <summary>
			/// Array of instructions for each glyph; shall be as specified in the TrueType Instruction Set. 
			/// </summary>
			public byte[] instructions;
			/// <summary>
			/// Array of flag elements. See below for details regarding the number of flag array elements. 
			/// </summary>
			public Flags[] flags;
			/// <summary>
			/// Contour point x-coordinates. Coordinate for the first point is relative to (0,0); others are relative to previous point. 
			/// </summary>
			public Int16[] xCoordinates;
			/// <summary>
			/// Contour point y-coordinates. Coordinate for the first point is relative to (0,0); others are relative to previous point. 
			/// </summary>
			public Int16[] yCoordinates;

            public void Serialize(OFFWriter writer)
            {
                throw new NotImplementedException();
            }

            public void Deserialize(OFFReader reader, int numberOfContours)
            {
                
            }
        }

		public struct CompositeGlyph : IOFFSerializable
        {
			[Flags]
			public enum Flags : UInt16
			{
				/// <summary>
				/// Bit 0: If this is set, the arguments are words; otherwise, they are bytes. 
				/// </summary>
				ARG_1_AND_2_ARE_WORDS = 0x0001,
				/// <summary>
				/// Bit 1: If this is set, the arguments are xy values; otherwise, they are points. 
				/// </summary>
				ARGS_ARE_XY_VALUES = 0x0002,
				/// <summary>
				/// Bit 2: For the xy values if the preceding is true.
				/// </summary>
				ROUND_XY_TO_GRID = 0x0004,
				/// <summary>
				/// Bit 3: This indicates that there is a simple scale for the component. Otherwise, scale = 1.0. 
				/// </summary>
				WE_HAVE_A_SCALE = 0x0008,
				/// <summary>
				/// Bit 5: Indicates at least one more glyph after this one. 
				/// </summary>
				MORE_COMPONENTS = 0x0020,
				/// <summary>
				/// Bit 6: The x direction will use a different scale from the y direction. 
				/// </summary>
				WE_HAVE_AN_X_AND_Y_SCALE = 0x0040,
				/// <summary>
				/// Bit 7: The bit WE_HAVE_A_TWO_BY_TWO allows for linear transformation of the X and Y
				/// coordinates by specifying a 2 × 2 matrix. 
				/// This could be used for scaling and 90° rotations of the glyph components, for example. 
				/// </summary>
				WE_HAVE_A_TWO_BY_TWO = 0x0080,
				/// <summary>
				/// Bit 8: Following the last component are instructions for the composite character. 
				/// </summary>
				WE_HAVE_INSTRUCTIONS = 0x1000,
				/// <summary>
				/// Bit 9: If set, this forces the aw and lsb (and rsb) for the composite to be equal to those
				/// from this original glyph. This works for hinted and unhinted characters. 
				/// </summary>
				USE_MY_METRICS = 0x0200,
				/// <summary>
				/// Bit 10: If set, the components of this compound glyph overlap. Use of this flag is
				/// not required in OFF — that is, it is valid to have components overlap without having this
				/// flag set. It may affect behaviors in some platforms, however. 
				/// (See Apple’s TrueType Reference Manual [7] for details regarding behavior in Apple platforms.) 
				/// When used, it must be set on the flag word for the first
				/// component. See additional remarks, above, 
				/// for the similar OVERLAP_SIMPLE flag used in simple-glyph descriptions.
				/// </summary>
				OVERLAP_COMPOUND = 0x0400,
				/// <summary>
				/// Bit 11: Composite designed to have the component offset scaled. 
				/// </summary>
				SCALED_COMPONENT_OFFSET = 0x800,
				/// <summary>
				/// Bit 12: Composite designed not to have the component offset scaled. 
				/// </summary>
				UNSCALED_COMPONENT_OFFSET = 0x1000
			}

			public void Deserialize(OFFReader reader)
			{

			}

			public void Serialize(OFFWriter writer)
            {
                throw new NotImplementedException();
            }
        }

		public override uint Tag => 0x676c7966;

		public int numGlyphs;
		

		public Table_glyf(int numGlyphs)
        {
			this.numGlyphs = numGlyphs;
        }
        public override void Deserialize(OFFReader reader)
        {
			/*
            for (int i = 0; i < numGlyphs; i++)
			{
				e
			}*/
        }

        public override void Serialize(OFFWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}