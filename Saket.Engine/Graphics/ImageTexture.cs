using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Numerics;
using System.Text.Json;
using QoiSharp.Codec;
using QoiSharp;
using Saket.Engine.Formats.Aseprite;
using Saket.Engine.Formats.Pyxel;
using Saket.Engine.GeometryD2.Shapes;
using Saket.Engine.Types;
using Saket.Serialization;
using StbImageSharp;
using WebGpuSharp;

namespace Saket.Engine.Graphics
{
    // TODO
    // Image only supports internal format of bgraunorm8 since thats the only supported surface format anyways

    public class ImageTexture : ISerializable
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
        
        public ImageTexture()
        {
            name = "texture_image";
            format = TextureFormat.BGRA8Unorm;
            width = 0;
            height = 0;
            data = [];
        }
        public ImageTexture(byte[] data, int width, int height, string name = "texture_image", TextureFormat format = TextureFormat.BGRA8Unorm)
        {
            this.name = name;
            this.data = data;
            this.format = format;

            this.width = width;
            this.height = height;
        }
        public ImageTexture(int width, int height, string name = "texture_image", TextureFormat format = TextureFormat.BGRA8Unorm)
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

        /// <summary>
        /// Gets the palette of the image
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static HashSet<Color> GetUniqueColors(byte[] data)
        {
            HashSet<Color> result = new HashSet<Color>();
            for (int i = 0; i < data.Length/4; i++)
            {
                result.Add(new Color(data[i], data[i + 1], data[i + 2], data[i + 3]));
            }
            return result;
        }

        public static byte[] GetThumbnail(byte[] data, int width, int height, out int dimension)
        {
            dimension = Math.Min(width, height);

            if(dimension == width && dimension == height)
            {
                return data;
            }
            Rectangle rect_source = new Rectangle(
               new Vector2(width, height) / 2f,
               new Vector2(dimension, dimension));

            Rectangle rect_target = new Rectangle(
              new Vector2(dimension, dimension) / 2f,
              new Vector2(dimension, dimension));

            var newData = new byte[width * height * 4];

            ImageTexture.Blit(data, width, height, rect_source,
              newData, dimension, dimension, rect_target);

            return newData;
        }

        public void ClearData()
        {
            data = null;
        }




        #region Pixel Maniuplation
        public static void Blit(byte[] sourceImage, int sourceWidth, int sourceHeight, byte[] targetImage, int targetWidth, int targetHeight, Direction anchor = Direction.Undefined)
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

    

        public static void Blit(
        byte[] sourceData, int sourceWidth, int sourceHeight, Rectangle sourceRect,
        byte[] targetData, int targetWidth, int targetHeight, Rectangle targetRect,
         List<int> includedSourceIndicies = null, int bytesPerPixel = 4) // Assuming RGBA format by default
        {

            // Create transformation matrices
            Matrix3x2 sourceTransform = sourceRect.CreateTransformMatrix();
            Matrix3x2 targetInverseTransform = targetRect.CreateInverseTransformMatrix();

            var bounds_target = targetRect.GetBounds();
      
            // Clamp to target image bo unds
            int startX = Math.Max((int)Math.Floor(bounds_target.Min.X), 0);
            int endX = Math.Min((int)Math.Ceiling(bounds_target.Max.X), targetWidth-1);
            int startY = Math.Max((int)Math.Floor(bounds_target.Min.Y), 0);
            int endY = Math.Min((int)Math.Ceiling(bounds_target.Max.Y), targetHeight-1);

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

                            // If we are excluding a pixel or is alpha = 0
                            if (includedSourceIndicies != null && !includedSourceIndicies.Contains((y_s_int * sourceWidth + x_s_int)))
                                continue;
                            if (sourceData[sourceIndex + 3] == 0)
                                continue;

                            int targetIndex = (y_t * targetWidth + x_t) * bytesPerPixel;
                            // Copy pixel data
                            Array.Copy(sourceData, sourceIndex, targetData, targetIndex, bytesPerPixel);
                        }
                    }
                }
            }
        }


        public static void FlipVertically(ref byte[] data, int width, int height)
        {
            // Calculate the number of bytes per row (width * bytes per pixel)
            int stride = width * 4;
            // Create a new byte array for the flipped image
            byte[] flippedImage = new byte[data.Length];

            // Loop through each row
            for (int row = 0; row < height; row++)
            {
                // Find the position of the current row in the original image
                int originalRowStart = row * stride;

                // Find the position of the corresponding flipped row in the new image
                int flippedRowStart = (height - 1 - row) * stride;

                // Copy the row from the original image to the flipped image
                Array.Copy(data, originalRowStart, flippedImage, flippedRowStart, stride);
            }

            data = flippedImage;
        }
        public static void FlipRedBlue(byte[] data)
        {
            // convert from rgba to bgra
            for (int i = 0; i < data.Length/4; ++i)
            {
                byte temp = data[i * 4];
                data[i * 4] = data[i * 4 + 2];
                data[i * 4 + 2] = temp;
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
        public ImageTexture(string path, bool flipVertically = true)
        {
            string ext = Path.GetExtension(path);

            if(ext == ".pyxel")
            {
                using var stream = new FileStream(path, FileMode.Open);

                using ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Read);


                PyxelDocData docData = new();
                {
                    ZipArchiveEntry? entry = zipArchive.GetEntry("docData.json") ?? throw new Exception("Failed to load");

                    using Stream entrystream = entry.Open();

                    docData = JsonSerializer.Deserialize<PyxelDocData>(entrystream, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                }

                byte[] pixels = new byte[docData.canvas.width * docData.canvas.height * 4];
                Rectangle rectBlit =
                    new Rectangle(new Vector2(docData.canvas.width, docData.canvas.height) / 2f,
                                new Vector2(docData.canvas.width, docData.canvas.height)
                    );
                for (int i = 0; i < docData.canvas.numLayers; i++)
                {
                    {
                        ZipArchiveEntry? entry_layer = zipArchive.GetEntry("layer"+ i +".png") ?? throw new Exception("Failed to load");
                        using Stream compressedStream = entry_layer.Open();
                        // Uncompress it
                        using MemoryStream uncompressedImagestream = new((int)entry_layer.Length);
                        compressedStream.CopyTo(uncompressedImagestream);
                        uncompressedImagestream.Seek(0, SeekOrigin.Begin);
                        StbImage.stbi_set_flip_vertically_on_load(1);
                        ImageResult result = ImageResult.FromStream(uncompressedImagestream, ColorComponents.RedGreenBlueAlpha);

                        ImageTexture.Blit(
                            result.Data, docData.canvas.width, docData.canvas.height, rectBlit,
                            pixels, docData.canvas.width, docData.canvas.height, rectBlit);
                    }
                }
               

                {
                    FlipRedBlue(pixels);
                }
               
                this.width = docData.canvas.width;
                this.height = docData.canvas.height;
                this.format = TextureFormat.BGRA8Unorm;
                this.data = pixels;
            }
            else if(ext == ".ase" || ext == ".aseprite")
            {
                using var stream = new FileStream(path, FileMode.Open);
                var file = new Aseprite();
                file.ReadFromStream(stream);

                this.width = file.Header.WidthInPixels;
                this.height = file.Header.HeightInPixels;
                this.format = TextureFormat.BGRA8Unorm;
                this.data = new byte[this.width * this.height * 4];

                foreach (var chunk in file.Frames[0].Chunks)
                {
                    if (chunk is Chunk_Cel cel)
                    {
                        Vector2 Size = new Vector2(cel.WidthInPixels, cel.HeightInPixels);
                        Vector2 HalfSize = new Vector2(cel.WidthInPixels, cel.HeightInPixels) / 2f;
                        Blit(
                            cel.RawPixelData, cel.WidthInPixels, cel.HeightInPixels, new Rectangle(HalfSize, Size),
                            data, width, height, new Rectangle(new Vector2(cel.Xposition, cel.Yposition) + HalfSize, Size));
                    }
                }
                FlipVertically(ref this.data, this.width, this.height);
                FlipRedBlue(this.data);
            }
            else
            {
                var stream = File.ReadAllBytes(path);
                StbImage.stbi_set_flip_vertically_on_load(flipVertically ? 1 : 0);

                ImageResult result = ImageResult.FromMemory(stream, StbImageSharp.ColorComponents.RedGreenBlueAlpha);

                this.name = Path.GetFileNameWithoutExtension(path);
                this.data = result.Data;
                this.format = TextureFormat.BGRA8Unorm;
                this.width = result.Width;
                this.height = result.Height;

                FlipRedBlue(this.data);
            }
        }

        /// <summary>
        /// Load image from memory
        /// </summary>
        /// <param name="path"></param>
        public ImageTexture(byte[] file, string name = "image",  bool flipVertically = true)
        {
            StbImage.stbi_set_flip_vertically_on_load(flipVertically ? 1 : 0);

            ImageResult result = ImageResult.FromMemory(file, StbImageSharp.ColorComponents.RedGreenBlueAlpha);

            this.name = name;
            this.data = result.Data;
            this.format = TextureFormat.BGRA8Unorm;
            this.width = result.Width;
            this.height = result.Height;

            // convert from rgba to bgra
            FlipRedBlue(this.data);
        }

        /// <summary>
        /// Save Image to path
        /// </summary>
        /// <param name="path"></param>
        public void SaveToPath(string path, bool flipVertically = true)
        {
            string ext = Path.GetExtension(path);


            byte[] data = new byte[Data.Length];

            //Iterate all pixels image and switch r<->b
            for (int i = 0; i < data.Length / 4; ++i)
            {
                data[i * 4 + 0] = Data[i * 4 + 2];
                data[i * 4 + 1] = Data[i * 4 + 1];
                data[i * 4 + 2] = Data[i * 4 + 0];
                data[i * 4 + 3] = Data[i * 4 + 3];
            }

            if(ext == ".pyxel")
            {
                using FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                using ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Update);
                {
                    ZipArchiveEntry? entry = zipArchive.GetEntry("docData.json");

                    PyxelDocData docData = new();
                  
                    if (entry != null)
                    {
                        using var stream_entry = entry.Open();
                        docData = JsonSerializer.Deserialize<PyxelDocData>(stream_entry, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                        stream_entry.Close();
                        entry.Delete();
                    }

                    docData.name = this.name;
                    docData.canvas = new PyxelDocData.Canvas()
                    {
                        width = this.width,
                        height = this.height,
                        tileWidth = this.width,
                        tileHeight = this.height,
                        numLayers = 1,
                        layers = new Dictionary<string, PyxelDocData.Canvas.Layer>
                        {
                            {
                                "0",
                                new PyxelDocData.Canvas.Layer()
                                {
                                    blendMode = "normal",
                                    alpha = 255,
                                    name ="Layer 0",
                                    type ="tile_layer",
                                    parentIndex = -1,
                                    tileRefs = []
                                }
                            }
                        }
                    };

                    {
                        entry = zipArchive.CreateEntry("docData.json");
                        using var stream_entry = entry.Open();
                        JsonSerializer.Serialize(stream_entry, docData, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true, WriteIndented = true });
                    }
                }
                {
                    // Delete previous image
                    ZipArchiveEntry? entry = zipArchive.GetEntry("layer0.png");
                    if (entry != null)
                        entry.Delete();

                    entry = zipArchive.CreateEntry("layer0.png");
                    using var stream_entry = entry.Open();
                    StbImageWriteSharp.StbImageWrite.stbi_flip_vertically_on_write(flipVertically ? 1 : 0);
                    new StbImageWriteSharp.ImageWriter().WritePng(data, (int)this.width, (int)this.height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream_entry);
                    
                }
                
            }
            else if (ext == ".ase" || ext == ".aseprite")
            {
                FlipVertically(ref data, Width, Height);
                var file = new Aseprite();

                file.Header.Frames = 1;
                file.Header.WidthInPixels   = (ushort)width;
                file.Header.HeightInPixels  = (ushort)height;
                if (width > ushort.MaxValue || height > ushort.MaxValue)
                    throw new Exception("File cannot be converted To .aseprite. Too large");
                file.Header.PixelWidth = file.Header.PixelHeight = 1;
                file.Header.Flags |= Header.Header_Flags.LayerOpacityHasValidValue;
                file.Header.ColorDepth = Header.Header_ColorDepth.RGBA;
                file.Frames = [
                    new Frame(){
                        Chunks = [
                            new Chunk_ColorProfile(){
                                ProfileType = Chunk_ColorProfile.Chunk_ColorProfile_ProfileType.sRGBProfile,
                            },
                            new Chunk_Layer(){
                                Blendmode = Chunk_Layer.Chunk_Layer_Blendmode.Normal,
                                LayerName = "Layer 0",
                                LayerType = Chunk_Layer.Chunk_Layer_LayerType.Normal,
                                Opacity = 255,
                                Flags = Chunk_Layer.Chunk_Layer_Flags.Visible | Chunk_Layer.Chunk_Layer_Flags.Editable
                            },
                            new Chunk_Cel(){
                                CelType = Chunk_Cel.Chunk_Cel_CelType.CompressedImage,
                                WidthInPixels = (ushort)width,
                                HeightInPixels = (ushort)height,
                                Xposition = 0,
                                Yposition = 0,
                                RawPixelData = data,
                                OpacityLevel = 255,
                            }
                        ]
                    }
                    ];

                using (Stream stream = File.OpenWrite(path))
                {
                    file.WriteToStream(stream);
                }
            }
            else
            {
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
            if (serializer.IsReader)
            {
                byte[] d = [];
                serializer.Serialize(ref d);
                QoiImage decoded = QoiSharp.QoiDecoder.Decode(d);
                this.data = decoded.Data;
            }
            else
            {
                QoiImage image = new QoiImage(data, width, height, Channels.RgbWithAlpha, ColorSpace.Linear);
                byte[] encoded = QoiSharp.QoiEncoder.Encode(image);
                serializer.Serialize(ref encoded);
            }
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