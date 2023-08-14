using Saket.Engine.Platform.Win;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Saket.Engine.Platform
{
    public static class Platform
    {
        private static IPlatform native;

        static Platform()
        {
            native = new Platform_Windows();
        }


        public static Window CreateWindow() => native.CreateWindow();

    }
}
