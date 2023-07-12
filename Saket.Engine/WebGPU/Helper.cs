using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Saket.Engine.WebGPU.Helper;

namespace Saket.Engine.WebGPU
{
    public class Helper
    {
        public static unsafe Task<IntPtr> RequestAdapterAsync(nint instance, WGPURequestAdapterOptions* options)
        {
            var tcs = new TaskCompletionSource<IntPtr>();

            WebGPU.WGPURequestAdapterCallback c = (
                WGPURequestAdapterStatus status,
                 IntPtr adapter,
                 char* message,
                 void* userdata) =>
            {
                if (status == WGPURequestAdapterStatus.Success)
                {
                    tcs.TrySetResult(IntPtr.Zero);
                }
                tcs.TrySetException(new Exception("Could not create adapter"));
            };

            wgpu.InstanceRequestAdapter(instance, options, c, null);

            return tcs.Task;
        }

        internal struct UserData
        {
            public IntPtr adapter;
            public bool requestEnded;
        };

        public static unsafe IntPtr RequestAdapter(nint instance, ref WGPURequestAdapterOptions options)
        {
            UserData data;

            WebGPU.WGPURequestAdapterCallback c = (WGPURequestAdapterStatus status, IntPtr adapter,char* message, void* userdata) =>
            {
                UserData* a = (UserData*)userdata;

                if (status == WGPURequestAdapterStatus.Success)
                {
                    a->adapter = adapter;
                }
                a->requestEnded = true;
            };

            fixed(WGPURequestAdapterOptions* ptr = &options)
            {
                wgpu.InstanceRequestAdapter(instance, ptr, c, &data);
            }
           


            return data.adapter;
        }



        public static unsafe IntPtr RequestDevice(nint adapter, ref WGPUDeviceDescriptor descriptor)
        {
            UserData data;

            WebGPU.WGPURequestDeviceCallback c = (WGPURequestDeviceStatus status,
                 IntPtr device,
                 char* message,
                 void* userdata) =>
            {
                UserData* a = (UserData*)userdata;

                if (status == WGPURequestDeviceStatus.Success)
                {
                    a->adapter = device;
                }
                a->requestEnded = true;
            };

            fixed (WGPUDeviceDescriptor* ptr = &descriptor)
            {
                wgpu.AdapterRequestDevice(adapter, ptr, c, &data);
            }

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

            WGPUSurfaceDescriptor surfaceDescriptor = new WGPUSurfaceDescriptor()
            {
                nextInChain = ((WGPUChainedStruct*)&ws)
            };

            nint surface = wgpu.InstanceCreateSurface(instance, ref surfaceDescriptor);

            return surface;
        }

    }
}
