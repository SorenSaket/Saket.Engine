using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine
{
    public struct ApplicationInfo
    {
        public float x = 0;
        public float y = 0;
        public float w = 1920;
        public float h = 1080;
        public string title = "Application";

        public ApplicationInfo()
        {
        }
    }

    /// <summary>
    /// Derive you application entry point from here
    /// </summary>
    public abstract class Application : GameWindow
    {
        protected Application(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {

        }
    }
}