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
                    tiles.Add(new Tile( w, h, (float)x / columns, (float)y / rows ));
                }
            }
        }

        public WebGPU.Objects.Buffer UploadTilesToDevice(Device device)
        {
            unsafe
            {
                var span = CollectionsMarshal.AsSpan(tiles);

                nuint size = (nuint)(sizeof(Tile) * span.Length);

                var buffer = device.CreateBuffer(
                   new WebGPU.WGPUBufferDescriptor()
                   {
                       size = size,
                       usage = WebGPU.WGPUBufferUsage.Uniform
                   });


                fixed (void* ptr = span)
                {
                    wgpu.QueueWriteBuffer(wgpu.DeviceGetQueue(device.Handle), buffer.Handle, 0, ptr, size);
                }
                return buffer;
            }
        }


        public static BindGroupLayout GetBindGroupLayout(Graphics graphics)
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
                            type = WGPUBufferBindingType.Uniform,
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
                            multisampled = true,
                        }
                    },
                    new()
                    {
                        binding = 2,
                        visibility = WGPUShaderStage.Fragment,
                        sampler = new()
                    }
                });
            }
            return layout;
        }

        public BindGroup GetBindGroup(Graphics graphics)
        {
            GetBindGroupLayout(graphics);
            if (bindgroup == null)
            {
                // Check that the image is uploaded as a texture
                if (gpuTexture == null)
                {
                    gpuTexture = image.UploadToDevice(graphics.device);
                    gpuTextureView = gpuTexture.CreateView(new WGPUTextureViewDescriptor());
                }

                if (gpuBuffer == null)
                    gpuBuffer = UploadTilesToDevice(graphics.device);

                // Check that the boxes are uploaded as buffer
                // Create a sampler

                bindgroup = graphics.device.CreateBindGroup(layout, stackalloc WGPUBindGroupEntry[] {
                    new()
                    {
                        binding = 0,
                        textureView = gpuTextureView!.Handle
                    },
                    new()
                    {
                        binding = 1,
                        buffer = gpuBuffer.Handle
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