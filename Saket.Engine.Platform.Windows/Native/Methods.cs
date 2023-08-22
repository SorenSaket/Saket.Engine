using Saket.Engine.Platform.Windows.Native;
using System;
using System.Runtime.InteropServices;

namespace Saket.Engine.Platform.Windows.Native
{
    // List of windowing functions:https://learn.microsoft.com/en-us/windows/win32/winmsg/window-functions
    // List of windows types: https://learn.microsoft.com/en-us/windows/win32/winprog/windows-data-types
    // Pinvoke.net: http://pinvoke.net/index.aspx

    public class Platform_Windows_PInvoke
    {

        /// <summary>
        /// Registers a window class for subsequent use in calls to the CreateWindow or CreateWindowEx function.
        /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerclassexw">Microsoft documentation</seealso>
        /// </summary>
        /// <param name="lpwcx">A pointer to a WNDCLASSEX structure. You must fill the structure with the appropriate class attributes before passing it to the function.</param>
        /// <returns>If the function succeeds, the return value is a class atom that uniquely identifies the class being registered. This atom can only be used by the CreateWindow, CreateWindowEx, GetClassInfo, GetClassInfoEx, FindWindow, FindWindowEx, and UnregisterClass functions and the IActiveIMMap::FilterClientWindows method.  If the function fails, the return value is zero.To get extended error information, call GetLastError.</returns>
        [DllImport("User32", CharSet = CharSet.Unicode)]
        public static extern short RegisterClassExW(ref WNDCLASSEX lpwcx);


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
        [DllImport("User32", CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateWindowExW(
            uint dwExStyle,
            string lpClassName,
            string lpWindowName,
            WindowStyles dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent = 0,
            IntPtr hMenu = 0,
            IntPtr hInstance = 0,
            IntPtr lpParam = 0);

        [DllImport("Kernel32", CharSet = CharSet.Unicode)]
        public static extern UInt32 GetLastError();

        /// <summary>
        /// Retrieves a message from the calling thread's message queue. The function dispatches incoming sent messages until a posted message is available for retrieval.
        /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getmessage">Microsoft documentation</see>
        /// </summary>
        /// <param name="lpMsg"></param>
        /// <param name="hWnd"></param>
        /// <param name="wMsgFilterMin"></param>
        /// <param name="wMsgFilterMax"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int GetMessageW(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        /// <summary>
        /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-peekmessagew">Microsoft documentation</see>
        /// </summary>
        /// <param name="lpMsg"></param>
        /// <param name="hWnd"></param>
        /// <param name="wMsgFilterMin"></param>
        /// <param name="wMsgFilterMax"></param>
        /// <param name="wRemoveMsg">Specifies how messages are to be handled</param>
        /// <returns>If a message is available, the return value is nonzero.   If no messages are available, the return value is zero.</returns>
        [DllImport("user32.dll")]
        public static extern int PeekMessageW(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);



        /// <summary>
        /// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-defwindowprocw
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="uMsg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
         public static extern IntPtr DefWindowProcW(IntPtr hwnd, WindowMessage uMsg, IntPtr wParam, IntPtr lParam);



        [DllImport("user32.dll")]
        public static extern bool DestroyWindow(IntPtr hWnd);


        /// <summary>
        /// Translates virtual-key messages into character messages. The character messages are posted to the calling thread's message queue, to be read the next time the thread calls the GetMessage or PeekMessage function.
        /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-translatemessage">Microsoft documentation</see>
        /// </summary>
        /// <param name="lpMsg"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool TranslateMessage([In] in MSG lpMsg);

        /// <summary>
        /// Dispatches a message to a window procedure. It is typically used to dispatch a message retrieved by the GetMessage function.
        ///<see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-dispatchmessage">Microsoft documentation</see>
        /// </summary>
        /// <param name="lpmsg">A pointer to a structure that contains the message.</param>
        /// <returns>The return value specifies the value returned by the window procedure. Although its meaning depends on the message being dispatched, the return value generally is ignored.</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr DispatchMessage([In] in MSG lpmsg);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nCmdShow"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);


        /// <summary>
        /// The UpdateWindow function updates the client area of the specified window by sending a WM_PAINT message to the window if the window's update region is not empty. The function sends a WM_PAINT message directly to the window procedure of the specified window, bypassing the application queue. If the update region is empty, no message is sent.
        /// <see href=https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-updatewindow">Microsoft documentation</see>
        /// </summary>
        /// <param name="hWnd">Handle to the window to be updated.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(IntPtr hWnd);
    }
}
