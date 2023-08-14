using System.Numerics;

namespace Saket.Engine.Typography.TrueType
{
    /// <summary>
    /// The setting of the Graphics State variables will affect the actions of certain instructions. 
    /// <see href="https://developer.apple.com/fonts/TrueType-Reference-Manual/RM04/Chap4.html">Developer Apple TrueType Reference Manual - Graphics State</see>
    /// </summary>
    internal struct GraphicsState
    {
        /// <summary>
        /// A unit vector that establishes an axis along which points can move.
        /// </summary>
        public Vector2 Freedom;
        /// <summary>
        /// A second projection vector set to a line defined by the original outline location of two points. The dual projection vector is used when it is necessary to measure distances from the scaled outline before any instructions were executed.
        /// </summary>
        public Vector2 DualProjection;
        /// <summary>
        /// A unit vector whose direction establishes an axis along which distances are measured.
        /// </summary>
        public Vector2 Projection;
        /// <summary>
        /// Makes it possible to turn off instructions under some circumstances. When set to TRUE, no instructions will be executed.
        /// </summary>
        public InstructionControlFlags InstructionControl;
        /// <summary>
        /// Determines the manner in which values are rounded. Can be set to a number of predefined states or to a customized state with the SROUND or S45ROUND instructions.
        /// </summary>
        public RoundMode RoundState;
        /// <summary>
        /// Establishes the smallest possible value to which a distance will be rounded.
        /// </summary>
        public float MinDistance;
        /// <summary>
        /// Limits the regularizing effects of control value table entries to cases where the difference between the table value and the measurement taken from the original outline is sufficiently small.
        /// </summary>
        public float ControlValueCutIn;
        /// <summary>
        /// The distance difference below which the interpreter will replace a CVT distance or an actual distance in favor of the single width value.
        /// </summary>
        public float SingleWidthCutIn;
        /// <summary>
        /// The value used in place of the control value table distance or the actual distance value when the difference between that distance and the single width value is less than the single width cut-in.
        /// </summary>
        public float SingleWidthValue;
        /// <summary>
        /// Establishes the base value used to calculate the range of point sizes to which a given DELTAC[] or DELTAP[] instruction will apply. The formulas given below are used to calculate the range of the various DELTA instructions.
        /// </summary>
        public int DeltaBase;
        /// <summary> Determines the range of movement and smallest magnitude of movement (the step) in a DELTAC[] or DELTAP[] instruction. Changing the value of the delta shift makes it possible to trade off fine control of point movement for range of movement. A low delta shift favors range of movement over fine control. A high delta shift favors fine control over range of movement. The step has the value 1/2 to the power delta shift. The range of movement is calculated by taking the number of steps allowed (16) and multiplying it by the step.
        /// The legal range for delta shift is zero through six.Negative values are illegal.
        /// </summary>
        public int DeltaShift;
        /// <summary>
        /// Makes it possible to repeat certain instructions a designated number of times. The default value of one assures that unless the value of loop is altered, these instructions will execute one time.
        /// </summary>
        public int Loop;
        /// <summary>
        /// The first of three reference points. References a point number that together with a zone designation specifies a point in either the glyph zone or the twilight zone.
        /// </summary>
        public int Rp0;
        /// <summary>
        /// The second of three reference points. References a point number that together with a zone designation specifies a point in either the glyph zone or the twilight zone.
        /// </summary>
        public int Rp1;
        /// <summary>
        /// The third of three reference points. References a point number that together with a zone designation specifies a point in to either the glyph zone or the twilight zone.
        /// </summary>
        public int Rp2;
        /// <summary>
        /// Controls whether the sign of control value table entries will be changed to match the sign of the actual distance measurement with which it is compared.  Setting auto flip to TRUE makes it possible to control distances measured with or against the projection vector with a single control value table entry. auto flip is set to FALSE, distances must be measured with the projection vector.
        /// <list type="table">
        ///     <item>
        ///         <term>Affects </term>
        ///         <description>MIRP[]</description>
        ///     </item>
        ///     <item>
        ///         <term>Set with</term>
        ///         <description>FLIPOFF[], FLIPON[]</description>
        ///     </item>
        ///</list>
        /// </summary>
        public bool AutoFlip;

        public void Reset()
        {
            Freedom = Vector2.UnitX;
            Projection = Vector2.UnitX;
            DualProjection = Vector2.UnitX;
            InstructionControl = InstructionControlFlags.None;
            RoundState = RoundMode.ToGrid;
            MinDistance = 1.0f;
            ControlValueCutIn = 17.0f / 16.0f;
            SingleWidthCutIn = 0.0f;
            SingleWidthValue = 0.0f;
            DeltaBase = 9;
            DeltaShift = 3;
            Loop = 1;
            Rp0 = Rp1 = Rp2 = 0;
            AutoFlip = true;
        }
    }
}
