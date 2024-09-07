using System;
using System.IO;
using Saket.Serialization;
using StbImageSharp;
using StbImageWriteSharp;
using WebGpuSharp;

namespace Saket.Engine.Graphics
{
    // TODO
    // QOI image format
    // Unload image from memory uption after upload to gpu, so that it doesn't take ram.

    public class Image : ISerializable
    {
        #region Properties
        public string Name { get { return name; } set { name = value; } }
        public TextureFormat Format { get { return format; }  }
        public uint Width { get { return width; } }
        public uint Height { get { return height; } }
        public byte[] Data { get { return data; } }
        public Texture? Texture { get { return texture; } set { texture = value;  } }
        public bool IsUploadedToGPU => texture != null;
        #endregion

        #region Variables
        internal string name;
        internal TextureFormat format;
        internal uint width;
        internal uint height;
        internal byte[] data;

        // GPU
        internal Texture? texture;
        internal Extent3D extendsTexture;
        #endregion
        
        public Image()
        {
            name = "texture_image";
            format = TextureFormat.BGRA8Unorm;
            width = 0;
            height = 0;
            data = [];
        }
        public Image(byte[] data, uint width, uint height, string name = "texture_image", TextureFormat format = TextureFormat.BGRA8Unorm)
        {
            this.name = name;
            this.data = data;
            this.format = format;

            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Call this if you want to change the data
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

        public void ClearData()
        {
            data = null;
        }

        #region File
        /// <summary>
        /// Load image from path
        /// </summary>
        /// <param name="path"></param>
        public Image(string path)
        {
            var stream = File.ReadAllBytes(path);

            StbImage.stbi_set_flip_vertically_on_load(1);
            
            ImageResult result = ImageResult.FromMemory(stream, StbImageSharp.ColorComponents.RedGreenBlueAlpha);

            // convert from rgba to bgra
            for (int i = 0; i < result.Width * result.Height; ++i)
            {
                byte temp = result.Data[i * 4];
                result.Data[i * 4] = result.Data[i * 4 + 2];
                result.Data[i * 4 + 2] = temp;
            }

            this.name = Path.GetFileNameWithoutExtension(path);
            this.data = result.Data;
            this.format = TextureFormat.BGRA8Unorm;
            this.width = (uint)result.Width;
            this.height = (uint)result.Height;
        }
        /// <summary>
        /// Save Image to path
        /// </summary>
        /// <param name="path"></param>
        public void SaveToPath(string path)
        {
            string ext = Path.GetExtension(path);

            // TODO covert back to rgba

            using (Stream stream = File.OpenWrite(path))
            {
                var w = new StbImageWriteSharp.ImageWriter();
                switch (ext)
                {
                    case ".png":
                        w.WritePng(data, (int)this.width, (int)this.height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream);
                        break;
                    case ".jpeg":
                    case ".jpg":
                        w.WriteJpg(data, (int)this.width, (int)this.height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream, 100);
                        break;
                    case ".bmp":
                        w.WriteBmp(data, (int)this.width, (int)this.height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream);
                        break;
                    case ".tga":
                        w.WriteTga(data, (int)this.width, (int)this.height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream);
                        break;
                    case ".hdr":
                        w.WriteHdr(data, (int)this.width, (int)this.height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream);
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region GPU
        public Texture? GPUCreateTexture(GraphicsContext graphics)
        {
            if(texture != null)
            {
                throw new Exception("Image " + name + " already exsists on gpu");
            }
            this.extendsTexture = new Extent3D(width, height, 1);

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

            return tex;
        }

        public void GPUWriteTexture(GraphicsContext graphics)
        {
            if(this.extendsTexture.Width != width || this.extendsTexture.Height != height)
            {
                // The texture requires resizing
                throw new Exception("");
            }
            if(this.data == null)
            {
                throw new Exception("Data is null, cannot upload to gpu");
            }


            graphics.queue.WriteTexture(
                new ImageCopyTexture()
                {
                    Texture = texture ?? throw new ArgumentNullException(nameof(texture)),
                },
                data,
                new TextureDataLayout()
                { 
                    // TODO get layout from format
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
        #endregion

        #region Serialization
        public void Serialize(ISerializer serializer)
        {
            serializer.Serialize(ref name);
            serializer.Serialize(ref format);
            serializer.Serialize(ref width);
            serializer.Serialize(ref height);
            serializer.Serialize(ref data);
        }

        // This is basically video compression betweena a I-frame
        // Todo later for now we upload the whole picture

        public Span<byte> ComputeDelta(ISerializer source, ISerializer dest)
        {
            throw new NotImplementedException();
        }

        public void ApplyDelta(ISerializer delta)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}