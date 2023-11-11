using Saket.Engine.Platform.Input;
using Saket.Engine.Platform.MSWindows.Windowing;
using Saket.Engine.Platform.Windowing;
using Windows.Win32.Foundation;

namespace Saket.Engine.Platform.MSWindows;

/// <summary>
/// Windows Platform
/// </summary>
public partial class Platform : IDesktopPlatform
{
    // subscribe to all window callback functions
    // listen to input gobally across all windows and combine to a single output with the current window as parameter?

    // The first window created will be considered the main window?


    List<Window> windows = new List<Window>();


    public event Action<Window> OnWindowCreated;

    public Platform()
    {
        OnPlatformCreated();
    }

    public partial void OnPlatformCreated();
    public Engine.Platform.Window CreateWindow(WindowCreationArgs args)
    {
        var window = new Saket.Engine.Platform.MSWindows.Windowing.Window(args);
        
        windows.Add(window);
        OnWindowCreated?.Invoke(window);

        return window;
    }

    public void PollEvent()
    {
        throw new NotImplementedException();
    }
}