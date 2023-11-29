using Saket.Engine.Platform;
using Saket.Engine.Platform.Input;
using Saket.Engine.Platform.SDL.Windowing;
using native = SDL2.SDL;

namespace Saket.Engine.Platform.SDL
{
    public class Platform : IDesktopPlatform
    {

        Stack<native.SDL_Event> stacky;

        public Platform()
        {
            native.SDL_Init(SDL2.SDL.SDL_INIT_EVERYTHING);
        }

        public List<Device> Devices => throw new NotImplementedException();

        public event Action<Device> OnDeviceAdded;
        public event Action<Device> OnDeviceRemoved;

        public Window CreateWindow(WindowCreationArgs args)
        {
            return new WindowSDL (args);
        }

        public void PollEvent()
        {
            native.SDL_PollEvent(out var _event);
            stacky.Push(_event);
        }
    }
}