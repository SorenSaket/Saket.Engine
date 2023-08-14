using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Saket.Engine.Platform;

//https://gist.github.com/AlexanderBaggett/d1504da93727a1778e8b5b3453946fc1
namespace Saket.Engine.Example
{
    internal class Program_DInvoke
    {


        public Program_DInvoke()

        {
            



            /* var windowClass = new User32.WNDCLASSEX()
             {

             };*/

            //short a = User32.RegisterClassEx(ref windowClass);


            //terminate game



            //


            //PInvoke.User32.RegisterClass(ref windowClass);

            //PInvoke.User32.CreateWindow("as", "w", User32.WindowStyles.WS_VISIBLE, 0, 0, 1280, 720, 0, 0, 0, 0);



            // Load the user32 dll in which all windowing functions are located
            //var ptr = FuncLoader.LoadLibrary("user32");

            // FuncLoader.LoadFunction<>(ptr, "CreateWindowW")
        }
    }
}
