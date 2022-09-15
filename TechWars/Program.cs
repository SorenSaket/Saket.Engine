using OpenTK.Windowing.Desktop;

namespace TechWars
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GameWindowSettings gameWindowSettings = new GameWindowSettings();
            gameWindowSettings.UpdateFrequency = 60;
            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings();
            nativeWindowSettings.Size = new OpenTK.Mathematics.Vector2i(1280, 720);
			
            var game = new Game(gameWindowSettings, nativeWindowSettings);
            game.Run();
        }
    }
}