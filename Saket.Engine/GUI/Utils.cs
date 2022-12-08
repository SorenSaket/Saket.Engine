using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Saket.UI
{
    public static class Utils
    {
        public static Dictionary<string, int> ids = new Dictionary<string, int>();
        public static Dictionary<string[], int> classes = new Dictionary<string[], int>();

        public static int GetAndRegisterID(string id)
        {
            if (ids.ContainsKey(id))
                return ids[id];
            else
            {
                int value = ids.Count;
                ids.Add(id, value);
                return value;
            }
        }

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