#if !WINDOWS_UWP

using System;
using System.Runtime.InteropServices;

namespace Saket.Engine.XInput
{
    /// <summary>	
    /// Functions	
    /// </summary>	
    /// <!-- No matching elements were found for the following include tag --><include file="Documentation\CodeComments.xml" path="/comments/comment[@id='SharpDX.XInput.XInput']/*" />	
    internal class XInput13 : IXInput
    {
        public int XInputSetState(int dwUserIndex, Vibration vibrationRef)
        {
            return Native.XInputSetState(dwUserIndex, vibrationRef);
        }

        public int XInputGetState(int dwUserIndex, out State stateRef)
        {
            return Native.XInputGetState(dwUserIndex, out stateRef);
        }

        public int XInputGetAudioDeviceIds(int dwUserIndex, IntPtr renderDeviceIdRef, IntPtr renderCountRef, IntPtr captureDeviceIdRef, IntPtr captureCountRef)
        {
            throw new NotSupportedException("Method not supported on XInput1.3");
        }

        public void XInputEnable(int enable)
        {
            Native.XInputEnable(enable);
        }

        public int XInputGetBatteryInformation(int dwUserIndex, BatteryDeviceType devType, out BatteryInformation batteryInformationRef)
        {
            return Native.XInputGetBatteryInformation(dwUserIndex, devType, out batteryInformationRef);
        }

        public int XInputGetKeystroke(int dwUserIndex, int dwReserved, out Keystroke keystrokeRef)
        {
            return Native.XInputGetKeystroke(dwUserIndex, dwReserved, out keystrokeRef);
        }

        public int XInputGetCapabilities(int dwUserIndex, DeviceQueryType dwFlags, out Capabilities capabilitiesRef)
        {
            return Native.XInputGetCapabilities(dwUserIndex, dwFlags, out capabilitiesRef);
        }

        private static class Native
        {
            [DllImport("xinput1_3.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "XInputGetKeystroke")]
            public static extern int XInputGetKeystroke(int dwUserIndex, int dwReserved, out Keystroke keystrokeRef);

            [DllImport("xinput1_3.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "XInputGetBatteryInformation")]
            public static extern int XInputGetBatteryInformation(int dwUserIndex, BatteryDeviceType devType, out BatteryInformation batteryInformationRef);

            public static unsafe int XInputSetState(int dwUserIndex, Vibration vibrationRef)
            {
                return XInputSetState_(dwUserIndex, (void*)(&vibrationRef));
            }

            [DllImport("xinput1_3.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "XInputSetState")]
            private static extern unsafe int XInputSetState_(int arg0, void* arg1);

            [DllImport("xinput1_3.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "XInputGetState")]
            public static extern int XInputGetState(int dwUserIndex, out State stateRef);

            [DllImport("xinput1_3.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "XInputEnable")]
            public static extern void XInputEnable(int arg0);

            [DllImport("xinput1_3.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "XInputGetCapabilities")]
            public static extern int XInputGetCapabilities(int dwUserIndex, DeviceQueryType dwFlags, out Capabilities capabilitiesRef);
        }
    }
}
#endif