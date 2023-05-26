using OpenTK.Windowing.Desktop;
using Saket.Engine;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Saket.Engine.Example
{
    public static class Program
    {
		[STAThread]
		static void Main()
        {
            GameWindowSettings gameWindowSettings = new GameWindowSettings();

            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings();
            nativeWindowSettings.Size = new OpenTK.Mathematics.Vector2i(1280, 720);

            var game = new ExampleProgram(gameWindowSettings, nativeWindowSettings);
            game.Run();
        }
	}
}