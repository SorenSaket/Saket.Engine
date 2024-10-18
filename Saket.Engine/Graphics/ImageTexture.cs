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
        public int PixelCount { get { return Width * Height; } }


        public int BytesPerPixel { get { return 4; } } // Todo do based of Textureformat

        public Texture? Texture { get { return texture; } set { texture = value;  } }
        public bool IsUploadedToGPU => texture != null;
        #endregion

        #region Variables
        public string Name;
        public TextureFormat Format;
        public int Width;
        public int Height;
        public byte[] Data;

        // GPU
        internal Texture? texture;
        internal Extent3D extendsTexture;
        #endregion
        
        public ImageTexture()
        {
            Name = "texture_image";
            Format = TextureFormat.BGRA8Unorm;
            Width = 0;
            Height = 0;
            Data = [];
        }
        public ImageTexture(byte[] data, int width, int height, string name = "texture_image", TextureFormat format = TextureFormat.BGRA8Unorm)
        {
            this.Name = name;
            this.Data = data;
            this.Format = format;

            this.Width = width;
            this.Height = height;
        }
        public ImageTexture(int width, int height, string name = "texture_image", TextureFormat format = TextureFormat.BGRA8Unorm)
        {
            this.Name = name;
            this.Data = new byte[width*height*4];
            this.Format = format;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Call this if you want to change the data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Edit(byte[] data, int width, int height)
        {
            this.Data = data;
            this.Width = width;
            this.Height = height;
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

            Blitter.Blit(new Blitter.BlitOp()
            {
                sourceData = data,
                sourceWidth = width,
                sourceHeight = height,
                sourceRect = rect_source,
                targetData = newData,
                targetWidth = dimension,
                targetHeight = dimension,
                targetRect = rect_target,
                Sampler = Blitter.Sample_Bilinear,
            });

            return newData;
        }

        public void ClearData()
        {
            Data = null;
        }




        #region Pixel Maniuplation

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
            for (int i = 0; i < Data.Length; i+=4)
            {
                Data[i + 2] = color.R;
                Data[i + 1] = color.G;
                Data[i] = color.B;
                Data[i + 3] = color.A;
            }
        }

        public void SetPixel(int index, Color color)
        {
            int a = index * BytesPerPixel;
            Data[a + 2] = color.R;
            Data[a + 1] = color.G;
            Data[a] = color.B;
            Data[a + 3] = color.A;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">The pixel index</param>
        /// <returns></returns>
        public Color GetPixel(int index)
        {
            int a = index * BytesPerPixel;
            return new Color(Data[a+2], Data[a+1], Data[a], Data[a+3]); 
        }

        public Vector2 GetPixelPosition(int index)
        {
            return new Vector2(index % Width, index / Height);
        }

        public bool WithinBounds(int index)
        {
            return index >= 0 && index < (Width*Height);
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

                        Blitter.Blit(new Blitter.BlitOp()
                        {
                            sourceData = result.Data,
                            sourceWidth = docData.canvas.width,
                            sourceHeight = docData.canvas.height,
                            sourceRect = new Rectangle(new Vector2(docData.canvas.width, docData.canvas.height)/2f, new Vector2(docData.canvas.width, docData.canvas.height)),
                            targetData = pixels,
                            targetWidth = docData.canvas.width,
                            targetHeight = docData.canvas.height,
                            targetRect = new Rectangle(new Vector2(docData.canvas.width, docData.canvas.height) / 2f, new Vector2(docData.canvas.width, docData.canvas.height)),
                            Sampler = Blitter.Sample_NearestNeighbor,
                        });

                    }
                }
               

                {
                    FlipRedBlue(pixels);
                }
               
                this.Width = docData.canvas.width;
                this.Height = docData.canvas.height;
                this.Format = TextureFormat.BGRA8Unorm;
                this.Data = pixels;
            }
            else if(ext == ".ase" || ext == ".aseprite")
            {
                using var stream = new FileStream(path, FileMode.Open);
                var file = new Aseprite();
                file.ReadFromStream(stream);

                this.Width = file.Header.WidthInPixels;
                this.Height = file.Header.HeightInPixels;
                this.Format = TextureFormat.BGRA8Unorm;
                this.Data = new byte[this.Width * this.Height * 4];

                for (int i = 0; i < file.Frames[0].Chunks.Length; i++)
                {
                    if (file.Frames[0].Chunks[i] is Chunk_Cel cel)
                    {
                        Vector2 Size = new Vector2(cel.WidthInPixels, cel.HeightInPixels);
                        Vector2 HalfSize = new Vector2(cel.WidthInPixels, cel.HeightInPixels) / 2f;

                        Blitter.Blit(new Blitter.BlitOp()
                        {
                            sourceData = cel.RawPixelData,
                            sourceWidth = cel.WidthInPixels,
                            sourceHeight = cel.HeightInPixels,
                            sourceRect = new Rectangle(HalfSize, Size),
                            targetData = Data,
                            targetWidth = Width,
                            targetHeight = Height,
                            targetRect = new Rectangle(new Vector2(cel.Xposition, cel.Yposition) + HalfSize, Size),
                            Sampler = Blitter.Sample_NearestNeighbor,
                            blendMode = BlendMode.Normal
                        });
                    }
                }
                FlipVertically(ref this.Data, this.Width, this.Height);
                FlipRedBlue(this.Data);
            }
            else
            {
                var stream = File.ReadAllBytes(path);
                StbImage.stbi_set_flip_vertically_on_load(flipVertically ? 1 : 0);

                ImageResult result = ImageResult.FromMemory(stream, StbImageSharp.ColorComponents.RedGreenBlueAlpha);

                this.Name = Path.GetFileNameWithoutExtension(path);
                this.Data = result.Data;
                this.Format = TextureFormat.BGRA8Unorm;
                this.Width = result.Width;
                this.Height = result.Height;

                FlipRedBlue(this.Data);
            }
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

                    docData.name = this.Name;
                    docData.canvas = new PyxelDocData.Canvas()
                    {
                        width = this.Width,
                        height = this.Height,
                        tileWidth = this.Width,
                        tileHeight = this.Height,
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
                    new StbImageWriteSharp.ImageWriter().WritePng(data, (int)this.Width, (int)this.Height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream_entry);
                    
                }
                
            }
            else if (ext == ".ase" || ext == ".aseprite")
            {
                FlipVertically(ref data, Width, Height);
                var file = new Aseprite();

                file.Header.Frames = 1;
                file.Header.WidthInPixels   = (ushort)Width;
                file.Header.HeightInPixels  = (ushort)Height;
                if (Width > ushort.MaxValue || Height > ushort.MaxValue)
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
                                WidthInPixels = (ushort)Width,
                                HeightInPixels = (ushort)Height,
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
                            w.WritePng(data, (int)this.Width, (int)this.Height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream);
                            break;
                        case ".jpeg":
                        case ".jpg":
                            w.WriteJpg(data, (int)this.Width, (int)this.Height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream, 100);
                            break;
                        case ".bmp":
                            w.WriteBmp(data, (int)this.Width, (int)this.Height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream);
                            break;
                        case ".tga":
                            w.WriteTga(data, (int)this.Width, (int)this.Height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream);
                            break;
                        case ".hdr":
                            w.WriteHdr(data, (int)this.Width, (int)this.Height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream);
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
                throw new Exception("Image " + Name + " already exsists on gpu");
            }
            this.extendsTexture = new Extent3D((uint)Width, (uint)Height, 1);

            Texture tex = graphics.device.CreateTexture(new TextureDescriptor()
            {
                Dimension = TextureDimension.D2,
                Format = Format,
                Size = extendsTexture,
                ViewFormats = [Format],
                Label = Name,
                Usage = TextureUsage.CopyDst | TextureUsage.CopySrc | TextureUsage.RenderAttachment | TextureUsage.TextureBinding,
                SampleCount = 1,
                MipLevelCount = 1,
            }) ?? throw new Exception("Texture Creation for image " + Name + " failed");

            this.texture = tex;

            return tex;
        }

        public void GPUWriteTexture(GraphicsContext graphics)
        {
            if(this.extendsTexture.Width != Width || this.extendsTexture.Height != Height)
            {
                // The texture requires resizing
                throw new Exception("");
            }
            if(this.Data == null)
            {
                throw new Exception("Data is null, cannot upload to gpu");
            }


            graphics.queue.WriteTexture(
                new ImageCopyTexture()
                {
                    Texture = texture ?? throw new ArgumentNullException(nameof(texture)),
                },
                Data,
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
            serializer.Serialize(ref Name);
            serializer.Serialize(ref Format);
            serializer.Serialize(ref Width);
            serializer.Serialize(ref Height);
            if (serializer.IsReader)
            {
                byte[] d = [];
                serializer.Serialize(ref d);
                QoiImage decoded = QoiSharp.QoiDecoder.Decode(d);
                this.Data = decoded.Data;
            }
            else
            {
                QoiImage image = new QoiImage(Data, Width, Height, Channels.RgbWithAlpha, ColorSpace.Linear);
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