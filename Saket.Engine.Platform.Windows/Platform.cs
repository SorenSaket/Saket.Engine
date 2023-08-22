namespace Saket.Engine.Platform.Windows
{
    /// <summary>
    /// Windows Platform
    /// </summary>
    public class Platform : IPlatform
    {
        public Engine.Platform.Window CreateWindow()
        {
            return new Window();
        }
    }
}