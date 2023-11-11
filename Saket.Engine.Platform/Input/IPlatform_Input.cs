
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Saket.Engine.Platform.Input;

// Joystick vs gamepad naming

/// <summary>
/// The platform implements Gamepad functionality
/// </summary>
public interface IPlatform_Gamepad
{
    // Input
    public int GamepadCount();
    public Gamepad[] GetGamepads();
    public Gamepad GetGamepads(int ID);
}

/// <summary>
/// 
/// </summary>
public interface IPlatform_HID
{
    public List<Device> Devices { get; }
    public event Action<Device> OnDeviceAdded;
    public event Action<Device> OnDeviceRemoved;
}

public class Device
{
    public Guid Id { get; set; }
    public HIDType DeviceType { get; set; }
    
}
public enum HIDType
{
    Unknown = 0,
    Other,
    Mouse,
    Keyboard,
    Gamepad,
    Joystick,
}
public struct DeviceInput
{
    public string identifier;
    public float value;

    // public float lastValue;
    /*
    public ButtonState State {
        get
        {
            if (lastValue == 0)
            {
                if(value != 0)
                    return ButtonState.JustPressed;
                if (value == 0)
                    return ButtonState.Released;
            }
            else
            {
                if (value != 0)
                    return ButtonState.Pressed;
                if (value == 0)
                    return ButtonState.JustReleased;
            }
            // Should never hit this
            return ButtonState.Released;
        }
    }*/
}