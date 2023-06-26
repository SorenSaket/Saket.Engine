using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PInvoke;
using Saket.Engine.Platform;



namespace Saket.Engine.Example
{
    internal class Program_DInvoke
    {/*
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr d_sdl_creatergbsurfacefrom(IntPtr pixels, int width, int height, int depth, int pitch, uint rMask, uint gMask, uint bMask, uint aMask);
        private static d_sdl_creatergbsurfacefrom SDL_CreateRGBSurfaceFrom = FuncLoader.LoadFunction<d_sdl_creatergbsurfacefrom>(NativeLibrary, "SDL_CreateRGBSurfaceFrom");
        */


        

        public Program_DInvoke()
        {
            //User32.MessageBox(0, "Testing123", "asdasd", User32.MessageBoxOptions.MB_ICONERROR);
            // 
            /*byte[] bytes = Encoding.UTF8.GetBytes("asdasd");

            Span<char> s = stackalloc char[] { "adsd" };
            // string str = "asdasd";
            "My sample source string".AsSpan();
            //str.ToCharArray()
            var windowClass = new User32.WNDCLASSEX()
            {
                
            };

            short a = User32.RegisterClassEx(ref windowClass);*/
            //terminate game



            //
            

            //PInvoke.User32.RegisterClass(ref windowClass);

            PInvoke.User32.CreateWindow("as", "w", User32.WindowStyles.WS_VISIBLE, 0, 0, 1280, 720, 0, 0, 0, 0);
            


            // Load the user32 dll in which all windowing functions are located
            //var ptr = FuncLoader.LoadLibrary("user32");

            // FuncLoader.LoadFunction<>(ptr, "CreateWindowW")
        }
    }
}
