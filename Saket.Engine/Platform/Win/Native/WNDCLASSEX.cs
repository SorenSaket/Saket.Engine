using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Platform.Win
{
    /// <summary>
    ///  <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-wndclassexa"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct WNDCLASSEX
    {
        /// <summary>
        /// The size, in bytes, of this structure. Set this member to sizeof(WNDCLASSEX). Be sure to set this member before calling the GetClassInfoEx function.
        /// </summary>
        public uint cbSize;
        /// <summary>
        /// The class style(s). This member can be any combination of the Class Styles.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public ClassStyles style;
        /// <summary>
        /// A pointer to the window procedure. You must use the CallWindowProc function to call the window procedure. For more information, see WindowProc.
        /// </summary>
        public IntPtr lpfnWndProc; // not WndProc
        /// <summary>
        /// The number of extra bytes to allocate following the window-class structure. The system initializes the bytes to zero.
        /// </summary>
        public int cbClsExtra;
        /// <summary>
        /// The number of extra bytes to allocate following the window instance. The system initializes the bytes to zero. If an application uses WNDCLASSEX to register a dialog box created by using the CLASS directive in the resource file, it must set this member to DLGWINDOWEXTRA.
        /// </summary>
        public int cbWndExtra;
        /// <summary>
        /// A handle to the instance that contains the window procedure for the class.
        /// </summary>
        public IntPtr hInstance;
        /// <summary>
        /// A handle to the class icon. This member must be a handle to an icon resource. If this member is NULL, the system provides a default icon.
        /// </summary>
        public IntPtr hIcon;
        /// <summary>
        /// A handle to the class cursor. This member must be a handle to a cursor resource. If this member is NULL, an application must explicitly set the cursor shape whenever the mouse moves into the application's window.
        /// </summary>
        public IntPtr hCursor;
        /// <summary>
        /// A handle to the class background brush. This member can be a handle to the brush to be used for painting the background, or it can be a color value. A
        /// </summary>
        public IntPtr hbrBackground;
        /// <summary>
        /// 
        /// </summary>
        public string lpszMenuName;
        /// <summary>
        /// A pointer to a null-terminated string.
        /// </summary>
        public string lpszClassName;
        /// <summary>
        /// A handle to a small icon that is associated with the window class. If this member is NULL, the system searches the icon resource specified by the hIcon member for an icon of the appropriate size to use as the small icon.
        /// </summary>
        public IntPtr hIconSm;
    }

}
