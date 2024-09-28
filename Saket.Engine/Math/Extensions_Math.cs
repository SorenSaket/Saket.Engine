using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine;

public static class Extensions_Math
{
    public static T Mod<T>(T value, T mod) where T : INumber<T>
    {
        return (value % mod + mod) % mod;
    }
    public static T RoundUpToNextMultiple<T>(T number, T multiple) where T : INumber<T>
    {
        if (multiple == T.Zero)
        {
            throw new ArgumentException("Multiple must be greater than 0.");
        }

        return ((number + multiple - T.One) / multiple) * multiple;
    }
    public static T Min<T>(params Span<T> values) where T : INumber<T>
    {
        T min = values[0];

        for (int i = 1; i < values.Length; i++)
        {
            if (min > values[i])
                min = values[i];
        }

        return min;
    }

    public static T Max<T>(params Span<T> values) where T : INumber<T>
    {
        T max = values[0];

        for (int i = 1; i < values.Length; i++)
        {
            if(max < values[i])
                max = values[i];
        }

        return max;
    }

}