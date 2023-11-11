using Saket.Engine.Platform;
using Saket.Engine.Platform.SDL.Windowing;
using native = SDL2.SDL;
namespace Saket.Engine.Platform.SDL
{
    public class Platform_SDL : IPlatform
    {
        public Platform_SDL()
        {
            native.SDL_Init(0);

        }

        public Window CreateWindow(WindowCreationArgs args)
        {
            return new WindowSDL (args);
        }
    }
}