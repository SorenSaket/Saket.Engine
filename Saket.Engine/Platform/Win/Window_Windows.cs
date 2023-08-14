using Saket.Engine.Platform.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Platform.Win
{
    /// <summary>
    /// https://learn.microsoft.com/en-us/windows/win32/winmsg/using-messages-and-message-queues
    /// 
    /// 
    /// </summary>


    public class Window_Windows : Window
    {
        public nint hInstance;
        public nint windowHandle;
        WindowMessage message;
        public Window_Windows()
        {
            hInstance = System.Diagnostics.Process.GetCurrentProcess().Handle;

            // The function to be called every frame
            Platform_Windows_PInvoke.del_WindowProc p = (IntPtr hwnd, WindowMessage uMsg, IntPtr wParam, IntPtr lParam) =>
            {
                return Platform_Windows_Del.DefWindowProc(hwnd, uMsg, wParam, lParam);
            };


            string className = "mainclass";
            var windowclass = new WNDCLASSEX()
            {
                cbSize = (uint)Marshal.SizeOf<WNDCLASSEX>(),
                style =
                ClassStyles.VerticalRedraw |
                ClassStyles.HorizontalRedraw,
                lpfnWndProc = Marshal.GetFunctionPointerForDelegate(p),
                lpszClassName = className,
                hInstance = hInstance,
            };

            var classAtom = Platform_Windows_PInvoke.RegisterClassExW(ref windowclass);

            windowHandle = Platform_Windows_PInvoke.CreateWindowExW(
                0,
                className,
                className,
                WindowStyles.WS_OVERLAPPEDWINDOW,
                0, 0, 1280, 720,
                IntPtr.Zero, IntPtr.Zero, hInstance, IntPtr.Zero);

            bool visible = Platform_Windows_PInvoke.ShowWindow(windowHandle, 5);
        }

        public override void Destroy()
        {
            throw new NotImplementedException();
        }

        public override WindowEvent PollEvent()
        {
            return WindowEvent.Exit;
            while (true)
            {
                var result = Platform_Windows_PInvoke.GetMessage(out message, 0, 0, 0);

                if (message == WindowMessage.DESTROY)
                    break;

                if (result > 0)
                {
                    Platform_Windows_PInvoke.TranslateMessage(ref message);
                    Platform_Windows_PInvoke.DispatchMessage(ref message);
                }
                else
                {
                    break;
                }
            }
        }
    }
}