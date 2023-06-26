using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
//using PInvoke;





namespace Saket.Engine.Platform.Windows
{
    internal class Platform_Windows : IPlatform
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr CreateWindow(
           string lpClassName,
           string lpWindowName,
           uint dwStyle,
           int x,
           int y,
           int nWidth,
           int nHeight,
           IntPtr hWndParent,
           IntPtr hMenu,
           IntPtr hInstance,
           IntPtr lpParam);

        void IPlatform.CreateWindow()
        {
                 
        }
    }
}
