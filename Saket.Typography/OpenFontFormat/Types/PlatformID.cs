namespace Saket.Typography.OpenFontFormat
{
    public enum PlatformID : ushort
    {
        Unicode = 0,
        Machintosh = 1,
        /// <summary>
        /// [Obsolete("Platform ID 2 (ISO) was deprecated as of OpenType version v1.3.")]
        /// </summary>
        ISO = 2,
        Windows = 3,
        Custom = 4
    }
}