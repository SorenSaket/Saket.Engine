using Saket.Engine.Platform.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Platform
{
    public abstract class Window
    {
        public abstract void Destroy();

        public abstract WindowEvent PollEvent();
    }
}
