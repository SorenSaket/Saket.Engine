using System;
using System.Drawing;
using WebGpuSharp;

namespace Saket.Engine.Graphics
{
    // TODO
    // Convert to platform angostic image loading
    // QOI image format
    // Unload image from memory uption after upload to gpu, so that it doesn't take ram.

    public class Image
    {
        public string name;
        public byte[] data;
        public TextureFormat format;
        public uint width;
        public uint height;

        public bool IsUploadedToGPU => texture != null;
        public Texture? texture;
        internal Extent3D extendsTexture;

        public Image(byte[] data, uint width, uint height, string name = "texture_image", TextureFormat format = TextureFormat.BGRA8Unorm)
        {
            this.name = name;
            this.data = data;
            this.format = format;

            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Call 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Edit(byte[] data, uint width, uint height)
        {
            this.data = data;
            this.width = width;
            this.height = height;
        }


        public Texture? GPUCreateTexture(GraphicsContext graphics)
        {
            if(texture != null)
            {
                throw new Exception("Image " + name + " already exsists on gpu");
            }

            Texture tex = graphics.device.CreateTexture(new TextureDescriptor()
            {
                Dimension = TextureDimension.D2,
                Format = format,
                Size = extendsTexture,
                ViewFormats = [format],
                Label = name,
                Usage = TextureUsage.CopyDst | TextureUsage.CopySrc | TextureUsage.RenderAttachment | TextureUsage.TextureBinding,
                SampleCount = 1,
                MipLevelCount = 1,
            }) ?? throw new Exception("Texture Creation for image " + name + " failed");

            this.texture = tex;
            this.extendsTexture = new Extent3D(width, height, 1);

            return tex;
        }

        public void GPUWriteTexture(GraphicsContext graphics)
        {
            if(this.extendsTexture.Width != width || this.extendsTexture.Height != height)
            {
                // The texture requires resizing
                throw new Exception("");
            }


            graphics.queue.WriteTexture(
                new ImageCopyTexture()
                {
                    Texture = texture ?? throw new ArgumentNullException(nameof(texture)),
                },
                data,
                new TextureDataLayout()
                { // TODO get layout from format
                    BytesPerRow = 4 * extendsTexture.Width,
                    RowsPerImage = extendsTexture.Height,
                },
                extendsTexture
            );
        }

        public void GPUDestroyTexture()
        {
            texture?.Destroy();
            texture = null;
        }
    }
}