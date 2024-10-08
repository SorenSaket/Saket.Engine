using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine
{
    public static class Extensions_Vector
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T X<T>(this Vector<T> vector) where T : struct
        {
            return vector[0];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Y<T>(this Vector<T> vector) where T : struct
        {
            return vector[1];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Z<T>(this Vector<T> vector) where T : struct
        {
            return vector[2];
        }
    }
}
