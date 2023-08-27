using Saket.WebGPU;
using Saket.WebGPU.Native;
using Saket.WebGPU.Objects;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Saket.Engine.Graphics
{
    public class TextureAtlas
    {
        public Image image;
        public List<Tile> tiles;

        Texture? gpuTexture;
        TextureView? gpuTextureView;
        Buffer? gpuBuffer;
        nuint gpuBufferSize;

        static BindGroupLayout? layout;
        BindGroup? bindgroup;

        public TextureAtlas(Image image, int initialCapacity = 128)
        {
            this.image = image;
            this.tiles = new List<Tile>(initialCapacity);
        }

        public TextureAtlas(Image image, uint columns, uint rows)
        {
            this.image = image;
            this.tiles = new List<Tile>((int)(columns*rows));
            float w = 1f / (columns);
            float h = 1f / rows;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    tiles.Add(new Tile(w, h, (float)x / columns, (float)y / rows ));
                }
            }
        }

        public WebGPU.Objects.Buffer UploadTilesToDevice(Device device)
        {
            unsafe
            {
                fixed (void* ptr_label = "buffer_tiles"u8)
                {
                    var span = CollectionsMarshal.AsSpan(tiles);

                    gpuBufferSize = (nuint)(sizeof(Tile) * span.Length);

                    var buffer = device.CreateBuffer(
                       new WebGPU.WGPUBufferDescriptor()
                       {
                           size = gpuBufferSize,
                           usage = WebGPU.WGPUBufferUsage.Storage | WGPUBufferUsage.CopyDst,
                           label = (char*)ptr_label
                       });


                    fixed (void* ptr = span)
                    {
                        wgpu.QueueWriteBuffer(wgpu.DeviceGetQueue(device.Handle), buffer.Handle, 0, ptr, gpuBufferSize);
                    }
                    return buffer;
                }
             
            }
        }


        public static BindGroupLayout GetBindGroupLayout(GraphicsContext graphics)
        {
            if (layout == null)
            {
                layout = graphics.device.CreateBindGroupLayout(stackalloc WGPUBindGroupLayoutEntry[] {
                    new()
                    {
                        binding = 0,
                        visibility = WGPUShaderStage.Vertex,
                        buffer = new()
                        {
                            type = WGPUBufferBindingType.ReadOnlyStorage,
                        }
                    },
                    new()
                    {
                        binding = 1,
                        visibility = WGPUShaderStage.Fragment,
                        texture = new()
                        {
                            viewDimension = WGPUTextureViewDimension._2D,
                            sampleType = WGPUTextureSampleType.Float,
                            multisampled = false,
                        }
                    },
                    new()
                    {
                        binding = 2,
                        visibility = WGPUShaderStage.Fragment,
                        sampler = new() { }
                    }
                });
            }
            return layout;
        }

        public BindGroup GetBindGroup(GraphicsContext graphics)
        {
            GetBindGroupLayout(graphics);
            if (bindgroup == null)
            {
                // Check that the image is uploaded as a texture
                if (gpuTexture == null)
                {
                    gpuTexture = image.UploadToGPU(graphics);
                    gpuTextureView = gpuTexture.CreateView(new WGPUTextureViewDescriptor()
                    {
                        format = graphics.applicationpreferredFormat,
                        dimension = WGPUTextureViewDimension._2D,
                        mipLevelCount = 1,
                        arrayLayerCount = 1
                    });
                }

                if (gpuBuffer == null)
                    gpuBuffer = UploadTilesToDevice(graphics.device);

                // Check that the boxes are uploaded as buffer
                // Create a sampler

                bindgroup = graphics.device.CreateBindGroup(layout, stackalloc WGPUBindGroupEntry[] {
                    new()
                    {
                        binding = 0,
                        buffer = gpuBuffer.Handle,
                        size = gpuBufferSize,
                    },
                    new()
                    {
                        binding = 1,
                        textureView = gpuTextureView!.Handle
                    },
                    new()
                    {
                        binding = 2,
                        sampler = graphics.defaultSampler.Handle
                    }
                });
            }
            return bindgroup;
        }
    }
}