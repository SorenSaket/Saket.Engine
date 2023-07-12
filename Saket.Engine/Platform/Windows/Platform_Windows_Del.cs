using System;
using System.Runtime.InteropServices;

namespace Saket.Engine.Platform.Windows
{
    // List of windowing functions:https://learn.microsoft.com/en-us/windows/win32/winmsg/window-functions
    // List of windows types: https://learn.microsoft.com/en-us/windows/win32/winprog/windows-data-types
    // Pinvoke.net: http://pinvoke.net/index.aspx

    public class Platform_Windows_Del
    {
        public static IntPtr lib_user32 = FuncLoader.LoadLibrary("user32");
        public static IntPtr lib_kernel = FuncLoader.LoadLibrary("kernel32");


    
        /// <summary>
        /// Registers a window class for subsequent use in calls to the CreateWindow or CreateWindowEx function.
        /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerclassexw">Microsoft documentation</seealso>
        /// </summary>
        /// <param name="lpwcx">A pointer to a WNDCLASSEX structure. You must fill the structure with the appropriate class attributes before passing it to the function.</param>
        /// <returns>If the function succeeds, the return value is a class atom that uniquely identifies the class being registered. This atom can only be used by the CreateWindow, CreateWindowEx, GetClassInfo, GetClassInfoEx, FindWindow, FindWindowEx, and UnregisterClass functions and the IActiveIMMap::FilterClientWindows method.  If the function fails, the return value is zero.To get extended error information, call GetLastError.</returns>
        public delegate short del_RegisterClassExW(
            ref WNDCLASSEX lpwcx);

        public static del_RegisterClassExW RegisterClassExW = FuncLoader.LoadFunction<del_RegisterClassExW>(lib_user32, "RegisterClassExW");


        /// <summary>
        /// Creates an overlapped, pop-up, or child window with an extended window style; otherwise, this function is identical to the CreateWindow function. For more information about creating a window and for full descriptions of the other parameters of CreateWindowEx, see CreateWindow.
        /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-createwindowexa">Microsoft documentation</see>
        /// </summary>
        /// 
        /// <param name="lpClassName">The extended window style of the window being created. For a list of possible values, see Extended Window Styles.</param>
        /// <param name="lpWindowName"></param>
        /// <param name="dwStyle"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="nWidth"></param>
        /// <param name="nHeight"></param>
        /// <param name="hWndParent"></param>
        /// <param name="hMenu"></param>
        /// <param name="hInstance"></param>
        /// <param name="lpParam"></param>
        /// <returns></returns>
        public delegate IntPtr del_CreateWindowExW(
            uint dwExStyle,
            string lpClassName,
            string lpWindowName,
            uint dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent = 0,
            IntPtr hMenu = 0,
            IntPtr hInstance = 0,
            IntPtr lpParam = 0);

        public static del_CreateWindowExW CreateWindowExW = FuncLoader.LoadFunction<del_CreateWindowExW>(lib_user32, "CreateWindowExW");

        public delegate UInt32 del_GetLastError();
        public static del_GetLastError GetLastError = FuncLoader.LoadFunction<del_GetLastError>(lib_kernel, "GetLastError");



        public delegate IntPtr del_WindowProc(IntPtr hwnd, WindowMessage uMsg, IntPtr wParam, IntPtr lParam);


        public delegate IntPtr del_DefWindowProc(IntPtr hwnd, WindowMessage uMsg, IntPtr wParam, IntPtr lParam);
        public static del_DefWindowProc DefWindowProc = FuncLoader.LoadFunction<del_DefWindowProc>(lib_user32, "DefWindowProcW");

    }
}
