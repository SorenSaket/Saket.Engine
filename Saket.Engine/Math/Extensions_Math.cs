using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine;

public static class Extensions_Math
{
   public static T RoundUpToNextMultiple<T>(T number, T multiple) where T : INumber<T>
    {
        if (multiple == T.Zero)
        {
            throw new ArgumentException("Multiple must be greater than 0.");
        }

        return ((number + multiple - T.One) / multiple) * multiple;
    }
}