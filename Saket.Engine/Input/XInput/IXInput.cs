using System;

namespace Saket.Engine.XInput
{

    public struct Vibration
    {
        UInt16 wLeftMotorSpeed;
        UInt16 wRightMotorSpeed;
    }

    /// <summary>
    /// Represents the state of a controller.
    /// </summary>
    /// <remarks>The dwPacketNumber member is incremented only if the status of the controller has changed since the controller was last polled.</remarks>  
    public struct State
    {
        /// <summary>
        /// State packet number. The packet number indicates whether there have been any changes in the state of the controller. If the dwPacketNumber member is the same in sequentially returned XINPUT_STATE structures, the controller state has not changed.
        /// </summary>
        UInt32 dwPacketNubmer;
        /// <summary>
        /// XINPUT_GAMEPAD structure containing the current state of an Xbox 360 Controller.
        /// </summary>
        Gamepad Gamepad;
    }

    public struct Gamepad
    {
        UInt16 wButtons;
        Byte bLeftTrigger;
        Byte bRightTrigger;
        Int16 sThumbLX;
        Int16 sThumbLY;
        Int16 sThumbRX;
        Int16 sThumbRY;
    }

    public struct BatteryInformation
    {
        BatteryType BatteryType;
        Byte BatteryLevel;
    }

    /// <summary>
    /// Specifies keystroke data returned by <see cref="IXInput.XInputGetKeystroke"/> .
    /// </summary>
    public struct Keystroke
    {
        /// <summary>
        /// Virtual-key code of the key, button, or stick movement. See XInput.h for a list of valid virtual-key (VK_xxx) codes. Also, see Remarks.
        /// </summary>
        UInt32 VirualKey;
        /// <summary>
        /// This member is unused and the value is zero.
        /// </summary>
        Char Unicode;
        /// <summary>
        /// Flags that indicate the keyboard state at the time of the input event. This member can be any combination of the following flags:
        /// </summary>
        UInt32 Flags;
        /// <summary>
        /// Index of the signed-in gamer associated with the device. Can be a value in the range 0–3.
        /// </summary>
        Byte UserIndex;
        /// <summary>
        /// HID code corresponding to the input. If there is no corresponding HID code, this value is zero.
        /// </summary>
        Byte HidCode;
    }

    public struct Capabilities
    {
        Byte Type;
        Byte SubType;
        UInt16 Flags;
        Gamepad Gamepad;
        Vibration vibration;
    }


    public enum Capability
    {
        XINPUT_CAPS_VOICE_SUPPORTED =  0x0004,
    }

    public enum BatteryType : Byte
    {
        BATTERY_TYPE_DISCONNECTED = 0,
        BATTERY_TYPE_WIRED = 1,
        BATTERY_TYPE_ALKALINE = 2,
        BATTERY_TYPE_NIMH = 3,
        BATTERY_TYPE_UNKNOWN = Byte.MaxValue,
    }

    public enum BatteryDeviceType : Byte
    {
        BATTERY_DEVTYPE_GAMEPAD = 0,
        BATTERY_DEVTYPE_HEADSET = 1,
    }

    public enum DeviceQueryType : UInt32
    {
        XINPUT_FLAG_GAMEPAD = 1,
    }


    /// <summary>
    /// Common interface for XInput to allow using XInput 1.4, 1.3 or 9.1.0.
    /// </summary>
    internal interface IXInput
    {
        /// <summary>
        /// Sends data to a connected controller. This function is used to activate the vibration function of a controller.
        /// </summary>
        /// <param name="dwUserIndex">Index of the user's controller. Can be a value from 0 to 3.</param>
        /// <param name="vibrationRef"><see cref="Vibration"> structure containing the vibration information to send to the controller.</param>
        /// <returns></returns>
        int XInputSetState(int dwUserIndex, Vibration vibrationRef);

        int XInputGetState(int dwUserIndex, out State stateRef);
        int XInputGetAudioDeviceIds(int dwUserIndex,
            System.IntPtr renderDeviceIdRef,
            System.IntPtr renderCountRef,
            System.IntPtr captureDeviceIdRef,
            System.IntPtr captureCountRef);

        /// <summary>
        /// Sets the reporting state of XInput.
        /// </summary>
        /// <param name="enable"></param>
        void XInputEnable(int enable);
        /// <summary>
        /// Retrieves the battery type and charge status of a wireless controller.
        /// </summary>
        /// <param name="dwUserIndex"></param>
        /// <param name="devType"></param>
        /// <param name="batteryInformationRef"></param>
        /// <returns></returns>
        int XInputGetBatteryInformation(int dwUserIndex, BatteryDeviceType devType, out BatteryInformation batteryInformationRef);

        /// <summary>
        /// Retrieves a gamepad input event.
        /// </summary>
        /// <param name="dwUserIndex">Index of the signed-in gamer associated with the device. Can be a value in the range 0–XUSER_MAX_COUNT − 1, or XUSER_INDEX_ANY to fetch the next available input event from any user.</param>
        /// <param name="dwReserved">Reserved</param>
        /// <param name="keystrokeRef">Pointer to an XINPUT_KEYSTROKE structure that receives an input event.</param>
        /// <returns></returns>
        int XInputGetKeystroke(int dwUserIndex, int dwReserved, out Keystroke keystrokeRef);
        int XInputGetCapabilities(int dwUserIndex, DeviceQueryType dwFlags, out Capabilities capabilitiesRef);
    }
}