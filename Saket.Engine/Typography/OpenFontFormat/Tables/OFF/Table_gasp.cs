using System;

namespace Saket.Engine.Filetypes.Font.OpenFontFormat.Tables
{
	/// <summary>
	/// gasp – Grid-fitting and scan-conversion procedure table.
	///This table contains information which describes the preferred rasterization techniques for the typeface when it
	/// is rendered on grayscale-capable devices. This table also has some use for monochrome devices, which may
	/// use the table to turn off hinting at very large or small sizes, to improve performance. 
	/// </summary>
	/// <remarks>
	/// At very small sizes, the best appearance on grayscale devices can usually be achieved by rendering the
	/// glyphs in grayscale without using hints. At intermediate sizes, hinting and monochrome rendering will usually
	/// produce the best appearance. At large sizes, the combination of hinting and grayscale rendering will typically
	/// produce the best appearance.
	/// If the 'gasp' table is not present in a typeface, the rasterizer may apply default rules to decide how to render
	/// the glyphs on grayscale devices.
	/// </remarks>
	public class Table_gasp : Table
	{
		public override uint Tag => 0x67617370;

		[Flags]
		public enum RangeGaspBehavior : UInt16
		{
			/// <summary>
			/// Use gridfitting
			/// </summary>
			GASP_GRIDFIT = 0x0001,
			/// <summary>
			/// Use grayscale rendering 
			/// </summary>
			GASP_DOGRAY= 0x0002,
			/// <summary>
			/// Use gridfitting with ClearType symmetric smoothing. 
			/// Only supported in version 1 of 'gasp' table 
			/// </summary>
			GASP_SYMMETRIC_GRIDFIT = 0x0004,
			/// <summary>
			/// Use smoothing along multiple axes with ClearType®.
			/// Only supported in version 1 of 'gasp' table.
			/// </summary>
			GASP_SYMMETRIC_SMOOTHING = 0x0008,
		}

		public struct GaspRangeRecord
		{
			public UInt16 rangeMaxPPEM;
			public UInt16 rangeGaspBehavoir;
		}

		/// <summary>
		/// Version number (set to 0 or 1)
		/// </summary>
		public UInt16 version;
		/// <summary>
		/// Number of records to follow 
		/// </summary>
		public UInt16 numRanges;
		public GaspRangeRecord[] gaspRanges;

		public Table_gasp()
		{
		}

		public override void Deserialize(OFFReader reader)
		{
			reader.LoadBytes(4);
			reader.ReadUInt16(ref version);
			reader.ReadUInt16(ref numRanges);

			gaspRanges = new GaspRangeRecord[numRanges];
			for (int i = 0; i < numRanges; i++)
			{
				reader.ReadUInt16(ref gaspRanges[i].rangeMaxPPEM);
				reader.ReadUInt16(ref gaspRanges[i].rangeGaspBehavoir);

			}
		}

		public override void Serialize(OFFWriter reader)
		{
			throw new NotImplementedException();
		}
	}
}