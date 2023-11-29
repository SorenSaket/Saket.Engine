using Saket.Engine.Platform.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebGpuSharp;
using native = SDL2.SDL;


namespace Saket.Engine.Platform.SDL.Windowing
{
    internal class WindowSDL : Window, IWebGPUSurfaceSource
    {
        private nint handle;

        public WindowSDL(WindowCreationArgs args) : base(args)
        {
            handle = native.SDL_CreateWindow(
                args.title,
                args.x,
                args.y,
                args.w,
                args.h,
                native.SDL_WindowFlags.SDL_WINDOW_SHOWN);
        }

        public override void Destroy()
        {
            throw new NotImplementedException();
        }


        public override void GetWindowPosition(out int x, out int y)
        {
            throw new NotImplementedException();
        }

        public override void Hide()
        {
            native.SDL_HideWindow(handle);
        }
        public override void Maximize()
        {
            native.SDL_MaximizeWindow(handle);
        }


        public override void Minimize()
        {
            native.SDL_MinimizeWindow(handle);
        }

        public override WindowEvent PollEvent()
        {
            return WindowEvent.None; 
        }

        public override void Raise()
        {
            throw new NotImplementedException();
        }

        public override void SetWindowPosition(int x, int y)
        {
            throw new NotImplementedException();
        }

        public override void Show()
        {
            throw new NotImplementedException();
        }

        public Surface? CreateWebGPUSurface(Instance instance)
        {
            unsafe
            {
                SDL2.SDL.SDL_SysWMinfo info = new();
                native.SDL_GetVersion(out info.version);
                native.SDL_GetWindowWMInfo(handle, ref info);

                if(info.subsystem == native.SDL_SYSWM_TYPE.SDL_SYSWM_WINDOWS)
                {
                    var wsDescriptor = new WebGpuSharp.FFI.SurfaceDescriptorFromWindowsHWNDFFI()
                    {
                        Hinstance = (void*)info.info.win.hinstance,
                        Hwnd = (void*)info.info.win.window,
                        Chain = new ChainedStruct()
                        {
                            SType = SType.SurfaceDescriptorFromWindowsHWND
                        }
                    };
                    SurfaceDescriptor descriptor_surface = new SurfaceDescriptor(ref wsDescriptor);
                    return instance.CreateSurface(descriptor_surface);
                }

                throw new Exception("Platform not supported");
            }
        }
    }
}
