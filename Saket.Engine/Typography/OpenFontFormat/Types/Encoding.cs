using System;

namespace Saket.Engine.Filetypes.Font.OpenFontFormat
{
	/// <summary>
	/// Encodings specific to the Unicode platform
	/// </summary>
	public enum EncodingUnicode : UInt16
	{
		/// <summary> Unicode 1.0 semantics. </summary> 
		Unicode10 = 0,
		/// <summary> Unicode 1.1 semantics. </summary> 
		Unicode11 = 1,
		/// <summary> ISO/IEC 10646 semantics </summary> 
		ISO10646 = 2,
		/// <summary> Unicode 2.0 and onwards semantics, Unicode BMP only (cmap subtable formats 0, 4, 6). </summary> 
		Unicode20BMP = 3,
		/// <summary> Unicode 2.0 and onwards semantics, Unicode full repertoire (cmap subtable formats 0, 4, 6, 10, 12)</summary> 
		Unicode20 = 4,
		/// <summary> Unicode Variation Sequences (cmap subtable format 14).  </summary> 
		UnicodeVariationSequences = 5,
		/// <summary> Unicode full repertoire (cmap subtable formats 0, 4, 6, 10, 12, 13) </summary> 
		Unicode = 6
	}

	[Obsolete("Platform ID 2 (ISO) was deprecated as of OpenType version v1.3.")]
	public enum EncodingISO : UInt16
	{
		/// <summary> 7-bit ASCII. </summary>
		ASCII = 0,
		/// <summary> ISO 10646. </summary>
		ISO10646 = 1,
		/// <summary> ISO 8859-1. </summary>
		ISO8859_1 = 2
	}

	public enum EncodingWindows : UInt16
	{
		Symbol = 0,
		UnicodeBMP = 1,
		ShiftJIS = 2,
		PRC = 3,
		Big5 = 4,
		Wansung = 5,
		Johab = 6,
		Unicode = 10
	}

	public struct EncodingRecord
	{
		/// <summary>
		/// Platform ID.
		/// </summary>
		public PlatformID platformID;
		/// <summary>
		/// Platform-specific encoding ID.
		/// </summary>
		public UInt16 encodingID;
		/// <summary>
		/// Byte offset from beginning of table to the subtable for this encoding 
		/// </summary>
		public UInt32 offset;

		public EncodingRecord(PlatformID platformID, ushort encodingID, uint offset)
		{
			this.platformID = platformID;
			this.encodingID = encodingID;
			this.offset = offset;
		}
	}
}