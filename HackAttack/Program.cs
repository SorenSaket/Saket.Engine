//using Saket.Engine.Platform.MSWindows;
using Saket.Engine.Platform.Windowing;
using SDL2;

namespace HackAttack;

internal class Program
{
    static void Main(string[] args)
    {
        var hackattack = new Application();
        hackattack.Run();
    }
}