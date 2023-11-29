

namespace Saket.Engine.Platform.Windowing;

// Minimal shared windowevent between platforms
// https://wiki.libsdl.org/SDL2/SDL_WindowEventID

public enum WindowEvent
{
    None = 0,
    Create,
    Destroy,
    Move,
    Resize,
    Activate,
    SetFocus,
    KillFocus,
    Enable,
    SetRedraw,
    SetText,
    GetText,
    GetTextLength,
    Paint,
    Close,
    Exit,
    Quit = 18
}
