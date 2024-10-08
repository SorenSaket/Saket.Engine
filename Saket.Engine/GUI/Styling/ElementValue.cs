﻿namespace Saket.Engine.GUI.Styling
{
    // Consider messing with Pack
    // https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.structlayoutattribute.pack?view=net-6.0
    // this could ensure that no padding is added
    // probably destroys possibility of interop
    // "Occasionally, the field is used to reduce memory requirements by producing a tighter packing size.
    // However, this usage requires careful consideration of actual hardware constraints, and may actually degrade performance."

    public struct ElementValue
    {
        public ElementValue(float value, Measurement measurement = Measurement.Pixels)
        {
            Value = value;
            Measurement = measurement;
        }
        public float Value { get; set; }

        public Measurement Measurement { get; set; }

    }

    public enum Measurement : byte
    {
        /// <summary>
        /// Absolute units in terms of pixels
        /// </summary>
        Pixels = 0,
        /// <summary>
        /// Percentage of parents inner size
        /// </summary>
        Percentage = 1,
        /// <summary>
        /// still dont know
        /// </summary>
        Stretch = 2,
    }
}
