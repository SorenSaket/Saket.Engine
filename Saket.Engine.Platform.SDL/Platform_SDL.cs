using Saket.Engine.Platform;
using Saket.Engine.Platform.Input;
using Saket.Engine.Platform.SDL.Windowing;
using native = SDL2.SDL;

namespace Saket.Engine.Platform.SDL
{
    public class Platform : IDesktopPlatform
    {
        List<WindowSDL> windows = new();


        public Platform()
        {
            native.SDL_Init(SDL2.SDL.SDL_INIT_EVERYTHING);
        }

        public List<Device> Devices => throw new NotImplementedException();

        public event Action<Device> OnDeviceAdded;
        public event Action<Device> OnDeviceRemoved;




        public Window CreateWindow(WindowCreationArgs args)
        {
            var w = new WindowSDL(args);

            windows.Add(w);

            return w;
        }

        public void PollEvent()
        {/*
            while (SDL2.SDL.SDL_PollEvent(out var e) != 0)
            {
                if (e.type == native.SDL_EventType.SDL_QUIT)
                {
                    Terminate();
                }
                if (e.type == native.SDL_EventType.SDL_WINDOWEVENT)
                {
                    windows.First(x => x.windowID ==        e.window.windowID).windowEvents.Push(e);
                }
            }*/
        }

        public void Terminate()
        {
            
        }
    }
}