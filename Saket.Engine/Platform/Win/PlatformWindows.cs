using Saket.Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Platform.Win
{
    internal class Platform_Windows : IPlatform
    {
        void ThrowWin32Error()
        {
            var err = Platform_Windows_PInvoke.GetLastError();
            if (err != 0)
                throw new Exception();
        }

        public Window CreateWindow(GraphicsContext graphicsContext)
        {
            return new Window_Windows(graphicsContext);
        }
    }
}
