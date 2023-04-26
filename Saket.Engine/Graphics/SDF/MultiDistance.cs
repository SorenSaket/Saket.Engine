using System.Runtime.CompilerServices;


namespace Saket.Engine.Graphics.SDF
{
    /// <summary>
    /// 
    /// </summary>
    public struct MultiDistance
    {
        public float R, G, B;
        public float Med;

        public float Dist
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Med;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Med = value;
        }
    }
}
