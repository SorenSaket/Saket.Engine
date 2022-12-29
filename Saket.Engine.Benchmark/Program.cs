using BenchmarkDotNet.Running;
using System.Diagnostics;
using System.Numerics;
using Saket.Engine.Filetypes.Font.TrueType;
using Saket.Engine;

public static class Program
{
    [STAThread]
    static void Main()
    {
        var stream = File.Open("C:/Users/Soren/Documents/data/projects/Saket/Saket.Engine/Saket.Engine/Assets/OpenSans-Regular.ttf", FileMode.Open);


        var font = new OpenFont(stream);

        Console.ReadKey();
    }
}

