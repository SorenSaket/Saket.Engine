using Saket.Engine.Platform.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using native = SDL2.SDL;
namespace Saket.Engine.Platform.SDL.Windowing
{
    internal class WindowSDL : Window
    {
        private nint handle;

        public WindowSDL(WindowCreationArgs args)
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

        public override nint GetSurface()
        {
            return  native.SDL_GetWindowSurface(handle);
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
            throw new NotImplementedException();
        }

        public override WindowEvent PollEvent()
        {
            throw new NotImplementedException();
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
    }
}
