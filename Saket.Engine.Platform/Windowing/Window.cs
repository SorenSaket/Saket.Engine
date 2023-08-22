using Saket.Engine.Platform.Windowing;
using Saket.WebGPU.Objects;
using Saket.WebGPU;

namespace Saket.Engine.Platform
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Window
    {
        public Surface surface;
        public WGPUTextureFormat preferredFormat;
        public Swapchain swapchain;

        /// <summary>
        /// Width in pixels
        /// </summary>
        public uint width;
        /// <summary>
        /// Height in pixels
        /// </summary>
        public uint height;

        public Window() 
        {

        }
        
        public  TextureView GetCurretTextureView() => swapchain.GetCurrentTextureView();
        
        public abstract void Destroy();

        public abstract WindowEvent PollEvent();
    }
}