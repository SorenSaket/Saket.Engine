using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using Saket.Engine.GeometryD2;
using Saket.Engine.GeometryD2.Shapes;
using Saket.Engine.Types;
using Saket.Serialization;
using StbImageSharp;
using StbImageWriteSharp;
using WebGpuSharp;
using static System.Net.Mime.MediaTypeNames;

namespace Saket.Engine.Graphics
{
    // TODO
    // Convert width/height to int for easier use with c#. 
    // QOI image format
    // Image only supports internal format of bgraunorm8 since thats the only supported surface format anyways

    public class Image : ISerializable
    {
        #region Properties
        public string Name { get { return name; } set { name = value; } }
        public TextureFormat Format { get { return format; }  }
        public int Width { get { return width; } }
        public int Height { get { return height; } }
        public int PixelCount { get { return Width * Height; } }
        public byte[] Data { get { return data; } }


        public int BytesPerPixel { get { return 4; } } // Todo do based of Textureformat

        public Texture? Texture { get { return texture; } set { texture = value;  } }
        public bool IsUploadedToGPU => texture != null;
        #endregion

        #region Variables
        internal string name;
        internal TextureFormat format;
        internal int width;
        internal int height;
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
        public Image(byte[] data, int width, int height, string name = "texture_image", TextureFormat format = TextureFormat.BGRA8Unorm)
        {
            this.name = name;
            this.data = data;
            this.format = format;

            this.width = width;
            this.height = height;
        }
        public Image(int width, int height, string name = "texture_image", TextureFormat format = TextureFormat.BGRA8Unorm)
        {
            this.name = name;
            this.data = new byte[width*height*4];
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
        public void Edit(byte[] data, int width, int height)
        {
            this.data = data;
            this.width = width;
            this.height = height;
        }

        public void ClearData()
        {
            data = null;
        }


        #region Pixel Maniuplation
        public static void Blit(byte[] sourceImage, int sourceWidth, int sourceHeight, byte[] targetImage, int targetWidth, int targetHeight, Direction anchor)
        {
            // Calculate aspect ratio
            float sourceAspect = (float)sourceWidth / sourceHeight;
            float targetAspect = (float)targetWidth / targetHeight;

            // Calculate new dimensions to preserve aspect ratio
            int newWidth, newHeight;
            if (sourceAspect > targetAspect)
            {
                newWidth = targetWidth;
                newHeight = (int)(targetWidth / sourceAspect);
            }
            else
            {
                newWidth = (int)(targetHeight * sourceAspect);
                newHeight = targetHeight;
            }

            // Calculate anchor position
            int xPos, yPos;
            switch (anchor)
            {
                case Direction.NW:
                    xPos = 0;
                    yPos = 0;
                    break;
                case Direction.N:
                    xPos = (targetWidth - sourceWidth) / 2;
                    yPos = 0;
                    break;
                case Direction.NE:
                    xPos = targetWidth - sourceWidth;
                    yPos = 0;
                    break;
                case Direction.W:
                    xPos = 0;
                    yPos = (targetHeight - sourceHeight) / 2;
                    break;
                case Direction.Undefined:
                    xPos = (targetWidth - sourceWidth) / 2;
                    yPos = (targetHeight - sourceHeight) / 2;
                    break;
                case Direction.E:
                    xPos = targetWidth - sourceWidth;
                    yPos = (targetHeight - sourceHeight) / 2;
                    break;
                case Direction.SW:
                    xPos = 0;
                    yPos = targetHeight - sourceHeight;
                    break;
                case Direction.S:
                    xPos = (targetWidth - sourceWidth) / 2;
                    yPos = targetHeight - sourceHeight;
                    break;
                case Direction.SE:
                    xPos = targetWidth - sourceWidth;
                    yPos = targetHeight - sourceHeight;
                    break;
                default:
                    xPos = 0;
                    yPos = 0;
                    break;
            }
            // Blit the source image onto the target image without resizing
            for (int y = 0; y < sourceHeight; y++)
            {
                for (int x = 0; x < sourceWidth; x++)
                {
                    // Make sure the target position is within bounds
                    int targetX = xPos + x;
                    int targetY = yPos + y;

                    if (targetX >= 0 && targetX < targetWidth && targetY >= 0 && targetY < targetHeight)
                    {
                        // Calculate the source and target pixel indices (assuming RGBA format, 4 bytes per pixel)
                        int sourceIndex = (y * sourceWidth + x) * 4;
                        int targetIndex = (targetY * targetWidth + targetX) * 4;

                        // Copy RGBA pixel from source to target
                        targetImage[targetIndex] = sourceImage[sourceIndex];         // R
                        targetImage[targetIndex + 1] = sourceImage[sourceIndex + 1]; // G
                        targetImage[targetIndex + 2] = sourceImage[sourceIndex + 2]; // B
                        targetImage[targetIndex + 3] = sourceImage[sourceIndex + 3]; // A
                    }
                }
            }
        }

    

        public static void Blita(
           byte[] sourceData, int sourceWidth, int sourceHeight, Rectangle sourceBoundingBox,
           byte[] targetData, int targetWidth, int targetHeight, Rectangle targetBoundingBox)
        {
            int bytesPerPixel = 4; // Assuming RGBA format

            // Create transformation matrices for source and target bounding boxes
            Matrix3x2 sourceTransform = sourceBoundingBox.CreateTransformMatrix();

            Matrix3x2 targetTransform = targetBoundingBox.CreateTransformMatrix();
            
            // Compute the inverse transformation matrix from target to source space
            Matrix3x2.Invert(sourceTransform, out Matrix3x2 inverseSourceTransform);
            Matrix3x2 combinedTransform = targetTransform * inverseSourceTransform;

            // Calculate the axis-aligned bounding rectangle of the target bounding box
            BoundingBox2D targetRect = targetBoundingBox.GetBounds();

            // Loop through each pixel in the target bounding box
            for (int y = (int)targetRect.Bottom; y <= targetRect.Top; y++)
            {
                for (int x = (int)targetRect.Left; x <= targetRect.Right; x++)
                {
                    // Target pixel position
                    Vector2 targetPixel = new Vector2(x, y);
                    Vector2 sourcePixel = Vector2.Transform(targetPixel, inverseSourceTransform);
                    
                    if (sourcePixel.X >= 0 && sourcePixel.X < sourceWidth &&
                    sourcePixel.Y >= 0 && sourcePixel.Y < sourceHeight)
                    {
                        // Use nearest-neighbor sampling (you can replace this with bilinear interpolation)
                        int sourceX = (int)MathF.Floor( sourcePixel.X);
                        int sourceY = (int)MathF.Floor(sourcePixel.Y);

                        int sourceIndex = (sourceY * sourceWidth + sourceX) * bytesPerPixel;
                        int targetIndex = (y * targetWidth + x) * bytesPerPixel;

                        // Ensure indices are within array bounds
                        if (sourceIndex >= 0 && (sourceIndex + bytesPerPixel) <= sourceData.Length &&
                            targetIndex >= 0 && (targetIndex + bytesPerPixel) <= targetData.Length)
                        {
                            if (sourceData[sourceIndex + 3] != 0)
                                // Copy the pixel data
                                System.Buffer.BlockCopy(sourceData, sourceIndex, targetData, targetIndex, bytesPerPixel);
                        }
                    }
                }
            }
        }


        public static void Blit(
        byte[] sourceData, int sourceWidth, int sourceHeight, Rectangle sourceBoundingBox,
        byte[] targetData, int targetWidth, int targetHeight, Rectangle targetBoundingBox,
        int bytesPerPixel = 4) // Assuming RGBA format by default
        {
            // Create transformation matrices
            Matrix3x2 sourceTransform = sourceBoundingBox.CreateTransformMatrix();
            Matrix3x2 targetInverseTransform = targetBoundingBox.CreateInverseTransformMatrix();

            var bounds_target = targetBoundingBox.GetBounds();
      
            // Clamp to target image bounds
            int startX = Math.Max((int)Math.Floor(bounds_target.Min.X), 0);
            int endX = Math.Min((int)Math.Ceiling(bounds_target.Max.X), targetWidth - 1);
            int startY = Math.Max((int)Math.Floor(bounds_target.Min.Y), 0);
            int endY = Math.Min((int)Math.Ceiling(bounds_target.Max.Y), targetHeight - 1);

            // Iterate over the pixels within the bounding box
            for (int y_t = startY; y_t <= endY; y_t++)
            {
                for (int x_t = startX; x_t <= endX; x_t++)
                {
                    Vector2 targetPixel = new Vector2(x_t, y_t);

                    // Transform the pixel to the rectangle's local space
                    Vector2 localPos = Vector2.Transform(targetPixel, targetInverseTransform);

                    // Check if the local position is within the rectangle
                    if (Math.Abs(localPos.X) <= 1 && Math.Abs(localPos.Y) <= 1)
                    {
                        // Map local position from target to source space
                        // First, map from [-1, 1] to source rectangle's local space
                        Vector2 sourceLocalPos = localPos;

                        // Transform local position to source pixel position
                        Vector2 sourcePixel = Vector2.Transform(sourceLocalPos, sourceTransform);

                        // Nearest neighbor sampling
                        int x_s_int = (int)Math.Round(sourcePixel.X);
                        int y_s_int = (int)Math.Round(sourcePixel.Y);

                        // Check bounds in the source image
                        if (x_s_int >= 0 && x_s_int < sourceWidth && y_s_int >= 0 && y_s_int < sourceHeight)
                        {
                            int sourceIndex = (y_s_int * sourceWidth + x_s_int) * bytesPerPixel;
                            int targetIndex = (y_t * targetWidth + x_t) * bytesPerPixel;

                            // Copy pixel data
                            Array.Copy(sourceData, sourceIndex, targetData, targetIndex, bytesPerPixel);
                        }
                    }
                }
            }
        }

            public void FillAllPixels(Color color)
        {
            for (int i = 0; i < data.Length; i+=4)
            {
                data[i + 2] = color.R;
                data[i + 1] = color.G;
                data[i] = color.B;
                data[i + 3] = color.A;
            }
        }

        public void SetPixel(int index, Color color)
        {
            int a = index * BytesPerPixel;
            data[a + 2] = color.R;
            data[a + 1] = color.G;
            data[a] = color.B;
            data[a + 3] = color.A;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">The pixel index</param>
        /// <returns></returns>
        public Color GetPixel(int index)
        {
            int a = index * BytesPerPixel;
            return new Color(data[a+2], data[a+1], data[a], data[a+3]); 
        }

        public Vector2 GetPixelPosition(int index)
        {
            return new Vector2(index % width, index / height);
        }

        public bool WithinBounds(int index)
        {
            return index >= 0 && index < (width*height);
        }

        #endregion

        #region File
        /// <summary>
        /// Load image from path
        /// </summary>
        /// <param name="path"></param>
        public Image(string path, bool flipVertically = true)
        {
            var stream = File.ReadAllBytes(path);

            StbImage.stbi_set_flip_vertically_on_load(flipVertically ? 1 : 0);
            
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
            this.width = result.Width;
            this.height = result.Height;
        }

        /// <summary>
        /// Load image from memory
        /// </summary>
        /// <param name="path"></param>
        public Image(byte[] file, string name ="image",  bool flipVertically = true)
        {
            StbImage.stbi_set_flip_vertically_on_load(flipVertically ? 1 : 0);

            ImageResult result = ImageResult.FromMemory(file, StbImageSharp.ColorComponents.RedGreenBlueAlpha);
           
            // convert from rgba to bgra
            for (int i = 0; i < result.Width * result.Height; ++i)
            {
                byte temp = result.Data[i * 4];
                result.Data[i * 4] = result.Data[i * 4 + 2];
                result.Data[i * 4 + 2] = temp;
            }

            this.name = name;
            this.data = result.Data;
            this.format = TextureFormat.BGRA8Unorm;
            this.width = result.Width;
            this.height = result.Height;
        }

        /// <summary>
        /// Save Image to path
        /// </summary>
        /// <param name="path"></param>
        public void SaveToPath(string path, bool flipVertically = true)
        {
            string ext = Path.GetExtension(path);


            byte[] data = new byte[Data.Length];

            // convert back from bgra to rgba
            for (int i = 0; i < data.Length/4; ++i)
            {
                data[i * 4 + 0] = Data[i * 4 + 2];
                data[i * 4 + 1] = Data[i * 4 + 1];
                data[i * 4 + 2] = Data[i * 4 + 0];
                data[i * 4 + 3] = Data[i * 4 + 3];
            }

            // TODO covert back to rgba
            StbImageWriteSharp.StbImageWrite.stbi_flip_vertically_on_write(flipVertically ? 1 : 0);

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
            this.extendsTexture = new Extent3D((uint)width, (uint)height, 1);

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