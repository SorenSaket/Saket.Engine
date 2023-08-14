using System;
using System.Threading.Tasks;
using Saket.WebGPU.Native;

namespace Saket.WebGPU
{
    public class Helper
    {
        internal struct UserData
        {
            public IntPtr adapter;
            public bool requestEnded;
        };

        public static unsafe IntPtr RequestAdapter(nint instance, in WGPURequestAdapterOptions options)
        {
            UserData data;

            static void c(WGPURequestAdapterStatus status, IntPtr adapter, char* message, void* userdata)
            {
                UserData* a = (UserData*)userdata;

                if (status == WGPURequestAdapterStatus.Success)
                {
                    a->adapter = adapter;
                }
                a->requestEnded = true;
            }

            Native.wgpu.InstanceRequestAdapter(instance, options, c, &data);
            
            return data.adapter;
        }
        public static unsafe IntPtr RequestDevice(nint adapter, in WGPUDeviceDescriptor descriptor)
        {
            UserData data;

            static void c(WGPURequestDeviceStatus status,
                 IntPtr device,
                 char* message,
                 void* userdata)
            {
                UserData* a = (UserData*)userdata;

                if (status == WGPURequestDeviceStatus.Success)
                {
                    a->adapter = device;
                }
                a->requestEnded = true;
            }


            wgpu.AdapterRequestDevice(adapter, descriptor, c, &data);
            

            return data.adapter;
        }
        public static unsafe IntPtr CreateSurfaceWin(nint instance, nint hInstance, nint windowHandle)
        {
            WGPUSurfaceDescriptorFromWindowsHWND ws = new()
            {
                chain = new WGPUChainedStruct()
                {
                    next = null,
                    sType = WGPUSType.SurfaceDescriptorFromWindowsHWND
                },
                hinstance = hInstance,
                hwnd = windowHandle,
            };

            WGPUSurfaceDescriptor surfaceDescriptor = new ()
            {
                nextInChain = ((WGPUChainedStruct*)&ws)
            };

            nint surface = wgpu.InstanceCreateSurface(instance, surfaceDescriptor);

            return surface;
        }

    }
}
