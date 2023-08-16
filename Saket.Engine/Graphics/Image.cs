using Saket.WebGPU;
using Saket.WebGPU.Native;
using Saket.WebGPU.Objects;

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
        public WGPUTextureFormat format;
        
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
            unsafe
            {
                fixed (void* ptr_label = "texture_image"u8)
                fixed (void* ptr_data = data)
                {
                    var formats = stackalloc WGPUTextureFormat[1]
                    {
                        WGPUTextureFormat.BGRA8UnormSrgb
                    };

                    Texture tex = graphics.device.CreateTexture(new WebGPU.WGPUTextureDescriptor()
                    {
                        dimension = WebGPU.WGPUTextureDimension._2D,
                        format = WebGPU.WGPUTextureFormat.BGRA8UnormSrgb, // TODO
                        size = new WebGPU.WGPUExtent3D(width, height, 1),
                        usage = WebGPU.WGPUTextureUsage.TextureBinding | WGPUTextureUsage.CopyDst,
                        viewFormatCount = 1,
                        viewFormats = formats,
                        label = (char*)ptr_label
                    });

                    wgpu.QueueWriteTexture(
                        graphics.queue.Handle, 
                        new WGPUImageCopyTexture() { 
                            texture = tex.Handle,
                        }, 
                        ptr_data, 
                        (nuint)data.Length, 
                        new WGPUTextureDataLayout() {
                            bytesPerRow = 4*width,
                            rowsPerImage = height,
                        }, 
                        new WGPUExtent3D() { width = width, height = height, depthOrArrayLayers = 1}
                        );
                    return tex;
                }
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