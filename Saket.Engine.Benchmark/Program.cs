using BenchmarkDotNet.Running;
using System.Diagnostics;
using System.Numerics;
using Saket.Engine;
using Saket.Engine.Typography;

public static class Program
{
    [STAThread]
    static void Main()
    { //System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        var stream = File.Open("C:\\Users\\saket\\data\\projects\\Saket.Engine\\Saket.Engine\\Assets\\Fonts\\OpenSans-Regular.ttf", FileMode.Open);

        var font = new Font();
        font.LoadFromOFF(stream);

        Console.ReadKey();
    }
}

