using System;

namespace Saket.Engine.Filetypes.Font.OpenFontFormat
{
	public enum PlatformID : UInt16
	{
		Unicode = 0,
		Machintosh = 1,
		[Obsolete("Platform ID 2 (ISO) was deprecated as of OpenType version v1.3.")]
		ISO = 2,
		Windows = 3,
		Custom = 4
	}
}