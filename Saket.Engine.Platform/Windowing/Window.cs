using Saket.Engine.Platform.Windowing;


namespace Saket.Engine.Platform
{

    public struct WindowCreationArgs
    {
        public string title;
        public int x;
        public int y;
        public int w;
        public int h;

        public WindowCreationArgs(string title, int x, int y, int w, int h)
        {
            this.title = title;
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class Window
    {
        /// <summary>
        /// Width in pixels
        /// </summary>
        public uint width;
        /// <summary>
        /// Height in pixels
        /// </summary>
        public uint height;


        public bool Hidden;
        public bool Resizable;
        public bool AlwaysOnTop;
        public bool Borderd;

        protected Window(WindowCreationArgs args)
        {
            width = (uint) args.w;
            height = (uint) args.h;



        }


        public abstract void SetWindowPosition(int x, int y);
        public abstract void GetWindowPosition(out int x, out int y);


        public abstract void Show();
        public abstract void Hide();
        public abstract void Raise();
        public abstract void Minimize();
        public abstract void Maximize();


        public abstract nint GetSurface();
        
        public abstract void Destroy();

        public abstract WindowEvent PollEvent();
    }

    [Flags]
    public enum WindowFlags
    {
        Fullscreen,
        Borderless,
        Shown,
        Hidden,
        AlwaysOnTop,
    }
}