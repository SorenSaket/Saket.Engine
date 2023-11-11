using Saket.Engine.Platform.Input;
using Saket.Engine.Platform.MSWindows.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32.UI.Input;
using Windows.Win32.Foundation;
using Windows.Win32;
using Saket.Engine.Platform.MSWindows.Input.RawInput;

namespace Saket.Engine.Platform.MSWindows;

//https://github.com/MicrosoftDocs/win32/tree/docs/desktop-src/inputdev
public partial class Platform : IPlatform_HID
{
    public List<Device> Devices { get; private set; } = new List<Device>() ;

    public event Action<Device> OnDeviceAdded;
    public event Action<Device> OnDeviceRemoved;
    //https://learn.microsoft.com/en-us/windows-hardware/drivers/hid/hid-architecture#hid-clients-supported-in-windows
    public partial void OnPlatformCreated()
    {
        var a = new RAWINPUTDEVICE[]{
            // Gamepad
            new RAWINPUTDEVICE()
            {
                usUsagePage = 0x01,
                usUsage = 0x05,
                dwFlags = 0,
                // A handle to the target window. If NULL it follows the keyboard focus.
                hwndTarget = new HWND(0),
            },
            // Mouse
            new RAWINPUTDEVICE()
            {
                usUsagePage = 0x01,
                usUsage = 0x02,
                dwFlags = RAWINPUTDEVICE_FLAGS.RIDEV_NOLEGACY,
                hwndTarget = new HWND(0),
            },
            // Keyboard
            new RAWINPUTDEVICE()
            {
                usUsagePage = 0x01,
                usUsage = 0x06,
                dwFlags = RAWINPUTDEVICE_FLAGS.RIDEV_NOLEGACY,
                hwndTarget = new HWND(0),
            }
        };
        // Register that we want to recive input from these devices
        RawInput.RegisterRawInputDevices(a);

        // Get all currently connected devices and add them to the device list
        var devs = RawInput.GetRawInputDevices();
        for (int i = 0; i < devs.Length; i++)
        {

        }


        OnWindowCreated += (window) => {
            ((MSWindows.Windowing.Window)window).windowProcedure += (window, uMsg, wparam, lparam) =>
            {
                switch (uMsg)
                {
                    case 0x00FF: // WM_INPUT 



                        break;
                    case 0x00FE: // WM_INPUT_DEVICE_CHANGE 
                                 //https://learn.microsoft.com/en-us/windows/win32/inputdev/wm-input-device-change
                        if (wparam == 1) // GIDC_ARRIVAL
                        {

                        }
                        else if (wparam == 2) // GIDC_REMOVAL
                        {

                        }
                        break;
                    default:
                        break;
                }

                return new Windows.Win32.Foundation.LRESULT();
            };


        };
     
    }

}
