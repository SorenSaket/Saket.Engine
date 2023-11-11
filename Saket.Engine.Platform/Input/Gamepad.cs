using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Platform.Input;

public abstract class Gamepad : Device
{
    public int ID { get; set; }
    public string Name { get; set; }
    public bool Connected { get; set; }

    Features features;

    Trigger Trigger_Left;
    Trigger Trigger_Right;

    Button Button_ShoulderLeft;
    Button Button_ShoulderRight;

    //
    Button Button_FaceDown;
    Button Button_FaceRight;
    Button Button_FaceUp;
    Button Button_FaceLeft;

    Button Button_Down;
    Button Button_Right;
    Button Button_Up;
    Button Button_Left;

    [Flags]
    public enum Features
    {
        Wireless,
    }
}

public struct Trigger
{
    public float value;
}

public struct Button
{
    public ButtonState state;
}

public enum ButtonState
{
    Released,
    JustPressed,
    Pressed,
    JustReleased
}