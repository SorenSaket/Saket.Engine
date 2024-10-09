using System;
using System.Collections.Generic;
using System.Reflection;

namespace Saket.Engine;

public static class Extensions_Enumeration
{
    
    public static bool AddRange<T>(this HashSet<T> source, IEnumerable<T> items)
    {
        bool allAdded = true;
        foreach (T item in items)
        {
            allAdded &= source.Add(item);
        }
        return allAdded;
    }

    public static bool RemoveRange<T>(this HashSet<T> source, IEnumerable<T> items)
    {
        bool allRemoved = true;
        foreach (T item in items)
        {
            allRemoved &= source.Remove(item);
        }
        return allRemoved;
    }
}