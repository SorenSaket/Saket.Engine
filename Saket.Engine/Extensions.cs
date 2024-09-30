using System;
using System.Collections.Generic;

namespace Saket.Engine;

public static class Extensions
{
    public static IEnumerable<T> EnumerateFlags<T>(this T input) where T : Enum
    {
        foreach (T value in Enum.GetValues(input.GetType()))
            if (input.HasFlag(value))
                yield return value;
    }
}
