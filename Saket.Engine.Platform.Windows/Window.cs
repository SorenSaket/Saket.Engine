using Saket.Engine.Platform.Windowing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Saket.Engine.Platform.Windows
{
    /// <summary>
    /// https://learn.microsoft.com/en-us/windows/win32/winmsg/using-messages-and-message-queues
    /// 
    /// 
    /// </summary>

    public class Window : Engine.Platform.Window
    {
        public IntPtr WindowHandle => windowHandle;
        public IntPtr HInstance => hInstance;

        internal HINSTANCE hInstance;
        internal HWND windowHandle;

        MSG message;
        
        public unsafe Window () : base ()
        {
            fixed (char * f = "mainclass")
            {
                hInstance = new HINSTANCE(System.Diagnostics.Process.GetCurrentProcess().Handle);

                //https://learn.microsoft.com/en-us/windows/win32/learnwin32/writing-the-window-procedure
                // The function to be called every frame
                WNDPROC p = (HWND w, uint p, WPARAM wparam, LPARAM lparam) => {

                    return PInvoke.DefWindowProc(w,p,wparam,lparam);
                }; 

                var cursor = PInvoke.LoadCursor( new HINSTANCE(0), PInvoke.IDC_ARROW);
                
                /*
                var err = Marshal.GetLastWin32Error();
                if (err != 0)
                    Console.WriteLine($"Error code : {Convert.ToString(err, 16)} ");*/
                var windowclass = new WNDCLASSEXW()
                {
                    cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
                    style =
                    WNDCLASS_STYLES.CS_VREDRAW |
                    WNDCLASS_STYLES.CS_HREDRAW,
                    lpfnWndProc = p,
                    lpszClassName = f,
                    hInstance = hInstance,
                    hCursor = cursor
                };

                var classAtom = PInvoke.RegisterClassEx(windowclass);
                
                windowHandle = PInvoke.CreateWindowEx(
                    0,
                    f,
                    f,
                    WINDOW_STYLE.WS_OVERLAPPEDWINDOW,
                    0, 0, 1280, 720,
                    new HWND(0), new HMENU(0), hInstance, (void*)0);

              
                bool visible = PInvoke.ShowWindow(windowHandle, SHOW_WINDOW_CMD.SW_SHOW);
            }

        }

        public override void Destroy()
        {
            throw new NotImplementedException();
        }


        public override WindowEvent PollEvent()
        {
            var result = PInvoke.PeekMessage(out message, windowHandle, 0, 0, PEEK_MESSAGE_REMOVE_TYPE.PM_REMOVE);
            if (result)
            {
                _ = PInvoke.TranslateMessage(message);
                //The DispatchMessage function tells the operating system to call the window procedure of the window that is the target of the message.
                _ = PInvoke.DispatchMessage(message);
            }
          
            return (WindowEvent)result.Value;
        }
    }
}