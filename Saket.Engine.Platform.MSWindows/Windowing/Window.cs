using Saket.Engine.Platform.Windowing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WebGpuSharp;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Saket.Engine.Platform.MSWindows.Windowing
{
    /// <summary>
    /// https://learn.microsoft.com/en-us/windows/win32/winmsg/using-messages-and-message-queues
    /// 
    /// 
    /// </summary>

    public class Window : Engine.Platform.Window , IWebGPUSurfaceSource
    {
        public nint WindowHandle => windowHandle;
        public nint HInstance => hInstance;

        internal HINSTANCE hInstance;
        internal HWND windowHandle;

        MSG message = new();

        internal event WNDPROC windowProcedure;
        
        
        //https://learn.microsoft.com/en-us/windows/win32/learnwin32/writing-the-window-procedure
        // The function to be called every frame
        LRESULT wp (HWND w, uint uMsg, WPARAM wParam, LPARAM lParam)
        {
           // windowProcedure?.Invoke(w, uMsg, wParam, lParam);

            return PInvoke.DefWindowProc(w, uMsg, wParam, lParam);
        }


        internal unsafe Window(WindowCreationArgs args) : base(args)
        {
            fixed (char* f = "mainclass\0")
            {
                hInstance = new HINSTANCE(System.Diagnostics.Process.GetCurrentProcess().Handle);

                var cursor = PInvoke.LoadCursor(new HINSTANCE(0), PInvoke.IDC_ARROW);

                /*
                var err = Marshal.GetLastWin32Error();
                if (err != 0)
                    Console.WriteLine($"Error code : {Convert.ToString(err, 16)} ");*/

                var windowclass = new WNDCLASSEXW()
                {
                    cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
                    //https://learn.microsoft.com/en-us/windows/win32/winmsg/window-class-styles
                    style =
                    WNDCLASS_STYLES.CS_VREDRAW |
                    WNDCLASS_STYLES.CS_HREDRAW ,
                    lpfnWndProc = PInvoke.DefWindowProc,
                    lpszClassName = f,
                    hInstance = hInstance,
                    hCursor = cursor
                   // hbrBackground = new HBRUSH((nint)(SYS_COLOR_INDEX.COLOR_BTNFACE + 1))
                };

                var classAtom = PInvoke.RegisterClassEx(windowclass);

                windowHandle = PInvoke.CreateWindowEx(
                    0,
                    f,
                    f,
                    // https://learn.microsoft.com/en-us/windows/win32/winmsg/window-styles
                    WINDOW_STYLE.WS_OVERLAPPEDWINDOW | WINDOW_STYLE.WS_VISIBLE ,
                    args.x, args.y, 
                    args.w, args.h,
                    new HWND(0), new HMENU(0), hInstance, (void*)0);


                bool visible = PInvoke.ShowWindow(windowHandle, SHOW_WINDOW_CMD.SW_SHOW);
                PInvoke.UpdateWindow(windowHandle);
            }
        }

        public override void Destroy()
        {
            throw new NotImplementedException();
        }


        public override WindowEvent PollEvent()
        {
            // Peek message is the non blocking version
            var result = PInvoke.PeekMessage(out message, default, 0, 0, PEEK_MESSAGE_REMOVE_TYPE.PM_REMOVE);

           // BOOL result = PInvoke.GetMessage(out message, default, 0, 0);

            switch (result.Value)
            {
                case -1:
                    throw new Exception("Window done goofed");
                case 0:
                    return WindowEvent.Quit;
                default:
                    _ = PInvoke.TranslateMessage(message);
                    //The DispatchMessage function tells the operating system to call the window procedure of the window that is the target of the message.
                    _ = PInvoke.DispatchMessage(message);
                    UInt16 re = ((UInt16)message.message);
                    return (WindowEvent)re;
            }
        }

        public override void SetWindowPosition(int x, int y)
        {
            PInvoke.SetWindowPos(windowHandle, (HWND)0, x, y, (int) width, (int) height, 0);
        }

        public override void GetWindowPosition(out int x, out int y)
        {
            throw new NotImplementedException();
        }

        public override void Show()
        {
            throw new NotImplementedException();
        }

        public override void Hide()
        {
            throw new NotImplementedException();
        }

        public override void Raise()
        {
            throw new NotImplementedException();
        }

        public override void Minimize()
        {
            throw new NotImplementedException();
        }

        public override void Maximize()
        {
            throw new NotImplementedException();
        }


        public Surface? CreateWebGPUSurface(Instance instance)
        {
            unsafe
            {
                var wsDescriptor = new WebGpuSharp.FFI.SurfaceDescriptorFromWindowsHWNDFFI()
                {
                    Hinstance = (void*)HInstance,
                    Hwnd = (void*)WindowHandle,
                    Chain = new ChainedStruct()
                    {
                        SType = SType.SurfaceDescriptorFromWindowsHWND
                    }
                };
                SurfaceDescriptor descriptor_surface = new SurfaceDescriptor(ref wsDescriptor);
                return instance.CreateSurface(descriptor_surface);
            }
        }

    }
}