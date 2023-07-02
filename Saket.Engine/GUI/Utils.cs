using Saket.Engine.GUI.Styling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace Saket.Engine.GUI
{
    public static class Utils
    {
        internal static void ParseMeasurement(string measurement, out int value, out Measurement unit)
        {
            value = int.Parse(Regex.Match(measurement, $"\\d+").Value);
            
            if (measurement.EndsWith("%"))
                unit = Measurement.Percentage;
            else if (measurement.EndsWith("px"))
                unit = Measurement.Pixels;
            else if (measurement.EndsWith("s"))
                unit = Measurement.Stretch;
            else
                unit = Measurement.Percentage;
        }
    }
}