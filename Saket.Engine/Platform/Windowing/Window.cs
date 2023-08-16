using Saket.Engine.Platform.Windowing;
using Saket.WebGPU.Objects;
using Saket.WebGPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saket.Engine.Graphics;

namespace Saket.Engine.Platform
{
    public abstract class Window
    {
        public Surface surface;
        public WGPUTextureFormat preferredFormat;
        public Swapchain swapchain;

        public GraphicsContext graphics;

        public Window(Graphics.GraphicsContext graphicsContext) 
        { 
            graphics = graphicsContext;
        }
        
        public  TextureView GetCurretTextureView() => swapchain.GetCurrentTextureView();
        
        public abstract void Destroy();

        public abstract WindowEvent PollEvents();
    }
}