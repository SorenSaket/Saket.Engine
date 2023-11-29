using System;
using WebGpuSharp;

namespace Saket.Engine.Graphics
{
    // TODO
    // Convert to platform angostic image loading
    // QOI image format
    // Unload image from memory uption after upload to gpu, so that it doesn't take ram.

    public class Image 
    {
        public int handle = -1;

        public bool IsLoadedOnGPU => handle != -1;
        public TextureFormat format;
        
        public byte[] data;
        public uint width;
        public uint height;



        public Image(byte[] data, uint width, uint height)
        {
            this.data = data;
            this.width = width;
            this.height = height;
        }

        public Texture UploadToGPU(GraphicsContext graphics)
        {
            {
                Span<TextureFormat> formats = stackalloc TextureFormat[1]
                {
                    TextureFormat.BGRA8UnormSrgb
                };

                Texture tex = graphics.device.CreateTexture(new TextureDescriptor()
                {
                    Dimension = TextureDimension._2D,
                    //only BGRA8Unorm is supported for webgpu dawn
                    Format = TextureFormat.BGRA8Unorm, // TODO
                    Size = new Extent3D(width, height, 1),
                    ViewFormats = formats,
                    Label = "texture_image"u8,
                    // Usage are required
                    Usage = TextureUsage.CopyDst | TextureUsage.CopySrc | TextureUsage.RenderAttachment | TextureUsage.TextureBinding,
                    SampleCount = 1, /*SampleCount over 0 is required*/
                    MipLevelCount = 1, /*MipLevelCount over 0 is required*/
                })!;

                graphics.queue.WriteTexture(
                    new ImageCopyTexture()
                    {
                        Texture = tex,
                    },
                    data,
                    new TextureDataLayout()
                    {
                        BytesPerRow = 4 * width,
                        RowsPerImage = height,
                    },
                    new Extent3D(width, height, 1)
                    ); ;
                return tex;
                
            }
        }


        /*
        public static Image FromPNG(IReader reader)
        {
        }*/

        /*
        public static Image FromPNG(IReader reader)
        {
            
            // Read signature
            {
            
                byte[] signature = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };

                var bytes = reader.ReadBytes(signature.Length);

                for (int i = 0; i < signature.Length; i++)
                {
                    if (bytes[i] != signature[i])
                        throw new Exception($"Invalid PNG in header byte: {i}");
                }
            }

            // Load Chunks
            {
                while (true)
                {
                    uint length = BinaryPrimitives.ReverseEndianness(reader.Read<uint>());

                    var chunkType = reader.ReadBytes(4);

                    if (chunkType[0] == )
                    {

                    }


                } 
            
            }


            return new Image(256,256);
        }*/
    }
}