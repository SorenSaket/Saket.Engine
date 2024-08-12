namespace TestApp;

internal class Program
{
    static void Main(string[] args)
    {
        //GC.TryStartNoGCRegion(1000000000);
        var app = new Application();
        app.Run();
    }
}
