using Saket.Engine.Platform.Input;
using Saket.Engine.Platform.Windowing;

namespace Saket.Engine.Platform;

/// <summary>
/// Platform abstraction layer
/// </summary>
public interface IPlatform
{
    public void Terminate();


    //public Display[] GetDisplays();



    // Event management
    //public void PumpEvents();
    public void PollEvent();

    // Sets the process Icon
    public virtual void SetIcon() {}
    // set the application progress
    public virtual void SetProgress() { }
}

public interface IDesktopPlatform : IPlatform, IPlatform_Windowing, IPlatform_HID
{

}
