using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.Intrinsics.X86;

namespace Saket.Engine.Platform.MSWindows.Input.XInput
{

    public struct Vibration
    {
        ushort wLeftMotorSpeed;
        ushort wRightMotorSpeed;
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
        uint dwPacketNubmer;
        /// <summary>
        /// XINPUT_GAMEPAD structure containing the current state of an Xbox 360 Controller.
        /// </summary>
        Gamepad Gamepad;
    }

    public struct Gamepad
    {
        ushort wButtons;
        byte bLeftTrigger;
        byte bRightTrigger;
        short sThumbLX;
        short sThumbLY;
        short sThumbRX;
        short sThumbRY;
    }

    public struct BatteryInformation
    {
        BatteryType BatteryType;
        byte BatteryLevel;
    }

    /// <summary>
    /// Specifies keystroke data returned by <see cref="IXInput.XInputGetKeystroke"/> .
    /// </summary>
    public struct Keystroke
    {
        /// <summary>
        /// Virtual-key code of the key, button, or stick movement. See XInput.h for a list of valid virtual-key (VK_xxx) codes. Also, see Remarks.
        /// </summary>
        uint VirualKey;
        /// <summary>
        /// This member is unused and the value is zero.
        /// </summary>
        char Unicode;
        /// <summary>
        /// Flags that indicate the keyboard state at the time of the input event. This member can be any combination of the following flags:
        /// </summary>
        uint Flags;
        /// <summary>
        /// Index of the signed-in gamer associated with the device. Can be a value in the range 0–3.
        /// </summary>
        byte UserIndex;
        /// <summary>
        /// HID code corresponding to the input. If there is no corresponding HID code, this value is zero.
        /// </summary>
        byte HidCode;
    }

    public struct Capabilities
    {
        byte Type;
        byte SubType;
        ushort Flags;
        Gamepad Gamepad;
        Vibration vibration;
    }


    public enum Capability
    {
        XINPUT_CAPS_VOICE_SUPPORTED = 0x0004,
    }

    public enum BatteryType : byte
    {
        BATTERY_TYPE_DISCONNECTED = 0,
        BATTERY_TYPE_WIRED = 1,
        BATTERY_TYPE_ALKALINE = 2,
        BATTERY_TYPE_NIMH = 3,
        BATTERY_TYPE_UNKNOWN = byte.MaxValue,
    }

    public enum BatteryDeviceType : byte
    {
        BATTERY_DEVTYPE_GAMEPAD = 0,
        BATTERY_DEVTYPE_HEADSET = 1,
    }

    public enum DeviceQueryType : uint
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

        /// <summary>
        /// Retrieves the current state of the specified controller.
        /// </summary>
        /// <param name="dwUserIndex">Index of the user's controller. Can be a value from 0 to 3. For information about how this value is determined and how the value maps to indicators on the controller, see Multiple Controllers.</param>
        /// <param name="stateRef">Pointer to an XINPUT_STATE structure that receives the current state of the controller.</param>
        /// <returns>If the function succeeds, the return value is ERROR_SUCCESS.  If the controller is not connected, the return value is ERROR_DEVICE_NOT_CONNECTED. If the function fails, the return value is an error code defined in Winerror.h.The function does not use SetLastError to set the calling thread's last-error code.</returns>
        int XInputGetState(int dwUserIndex, out State stateRef);

        /// <summary>
        /// Gets the sound rendering and sound capture device GUIDs that are associated with the headset connected to the specified controller.
        /// </summary>
        /// <param name="dwUserIndex"></param>
        /// <param name="renderDeviceIdRef"></param>
        /// <param name="renderCountRef"></param>
        /// <param name="captureDeviceIdRef"></param>
        /// <param name="captureCountRef"></param>
        /// <returns></returns>
        int XInputGetAudioDeviceIds(int dwUserIndex,
            nint renderDeviceIdRef,
            nint renderCountRef,
            nint captureDeviceIdRef,
            nint captureCountRef);

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

        /// <summary>
        /// Retrieves the capabilities and features of a connected controller.
        /// </summary>
        /// <param name="dwUserIndex"></param>
        /// <param name="dwFlags"></param>
        /// <param name="capabilitiesRef"></param>
        /// <returns></returns>
        int XInputGetCapabilities(int dwUserIndex, DeviceQueryType dwFlags, out Capabilities capabilitiesRef);
    }
}