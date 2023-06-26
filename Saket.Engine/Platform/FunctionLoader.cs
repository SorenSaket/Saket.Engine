using System;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Saket.Engine.Platform
{
    public class FuncLoader
    {
        private class Windows
        {
            /// <summary>
            /// Retrieves the address of an exported function (also known as a procedure) or variable from the specified dynamic-link library (DLL).
            /// </summary>
            /// <param name="hModule"></param>
            /// <param name="procName">The function or variable name, or the function's ordinal value. If this parameter is an ordinal value, it must be in the low-order word; the high-order word must be zero.</param>
            /// <returns>If the function succeeds, the return value is the address of the exported function or variable. If the function fails, the return value is NULL.To get extended error information, call GetLastError.</returns>
            [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern IntPtr LoadLibraryW(string lpszLib);

            [DllImport("kernel32")]
            public static extern int FreeLibrary(IntPtr module);
        }

        public static IntPtr LoadLibrary(string libraryName)
        {
            return Windows.LoadLibraryW(libraryName);
        }

        public static T LoadFunction<T>(IntPtr library, string function, bool throwIfNotFound = false)
        {
            var ret = Windows.GetProcAddress(library, function);

            if (ret == IntPtr.Zero)
            {
                if (throwIfNotFound)
                    throw new EntryPointNotFoundException(function);

                return default(T);
            }
           
            return Marshal.GetDelegateForFunctionPointer<T>(ret);
        }
    }
}
