using Saket.Engine.Graphics;
using Saket.WebGPU;
using Saket.WebGPU.Objects;
using System;

namespace Saket.Engine
{
    // TODO
    // Convert to platform angostic image loading
    // QOI image format
    // Unload image from memory uption after upload to gpu, so that it doesn't take ram.

    public class Image 
    {
        public int handle = -1;

        public bool IsLoadedOnGPU => handle != -1;
        public WGPUTextureFormat format;
        
        public byte[] data;
        public int width;
        public int height;

        public Image(int width, int height)
        {

        }

        public Texture UploadToDevice(Device device)
        {
            return device.CreateTexture(new WebGPU.WGPUTextureDescriptor()
            {
                dimension = WebGPU.WGPUTextureDimension._2D,
                format = WebGPU.WGPUTextureFormat.Undefined, // TODO
                size = new WebGPU.WGPUExtent3D(),
                usage = WebGPU.WGPUTextureUsage.TextureBinding,
            }); 
        }
    }
}