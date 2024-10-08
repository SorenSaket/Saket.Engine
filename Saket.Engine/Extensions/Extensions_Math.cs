using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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


    public static T RoundToNearestMultiple<T>(this T number, T multiple, MidpointRounding mode = MidpointRounding.AwayFromZero) where T : INumber<T>
    {
        if (multiple == T.Zero)
        {
            return number;
        }

        T remainder = number % multiple;
        if (remainder == T.Zero)
        {
            return number;
        }

        T halfMultiple = multiple / T.CreateChecked(2);

        if (remainder >= halfMultiple)
        {
            return number + (multiple - remainder);
        }
        else
        {
            return number - remainder;
        }
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithin<T>(this T v, T a, T b) where T : INumber<T>
    {
        return v >= a && v < b;
    }
}