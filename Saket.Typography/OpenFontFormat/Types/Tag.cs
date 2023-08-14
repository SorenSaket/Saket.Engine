namespace Saket.Typography.OpenFontFormat
{
    public struct Tag
    {
        public uint value;

        public Tag(uint value)
        {
            this.value = value;
        }

        public static implicit operator Tag(uint packed)
        {
            return new Tag(packed);
        }
    }
}
