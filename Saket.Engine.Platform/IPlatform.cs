namespace Saket.Engine.Platform
{
    /// <summary>
    /// Platform abstraction layer
    /// </summary>
    public interface IPlatform
    {
        
        public Window CreateWindow ();


        // Event management
        //public void PumpEvents();
        //public void PollEvent();


       


    }

    // The platform implements joystick functionality
    public interface IJoystickPlatform
    {
        // Input
        public int JoystickCount();
        public string JoystickName(int index);

    }

    // The platform implements joystick functionality
    public interface IPlatformInput
    {
        // Input
        public int InputDeviceCount();


        public string JoystickName(int index);




    }
}