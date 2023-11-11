using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32;
using Windows.Win32.UI.Input;

//https://learn.microsoft.com/en-us/windows/win32/inputdev/user-input
//https://learn.microsoft.com/en-us/windows/win32/inputdev/raw-input

namespace Saket.Engine.Platform.MSWindows.Input.RawInput;

internal static class RawInput
{
    internal static RAWINPUTDEVICELIST[] GetRawInputDevices()
    {
        unsafe
        {
            uint cbSize = (uint)Marshal.SizeOf<RAWINPUTDEVICELIST>();
            uint c = 0;
            PInvoke.GetRawInputDeviceList((RAWINPUTDEVICELIST*)IntPtr.Zero, ref c, cbSize);

            var devices = new RAWINPUTDEVICELIST[c];
            fixed(RAWINPUTDEVICELIST* ptr = devices)
            {

                PInvoke.GetRawInputDeviceList(ptr, ref c, cbSize);
            }
            return devices;
        }
    }

    /// <inheritdoc cref="PInvoke.RegisterRawInputDevices" />
    internal static void RegisterRawInputDevices(Span<RAWINPUTDEVICE> devices)
    {
        uint cbSize = (uint)Marshal.SizeOf<RAWINPUTDEVICE>();
        PInvoke.RegisterRawInputDevices(devices, cbSize);
    }

    /*

    static void Handle()
    {
        RAWINPUT raw = new();

        if(raw.header.dwType == (uint)RID_DEVICE_INFO_TYPE.RIM_TYPEMOUSE)
        {
            raw.data.mouse.
        }
    }*/
}