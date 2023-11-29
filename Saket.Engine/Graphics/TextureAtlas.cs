using WebGpuSharp;
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

        public WebGpuSharp.Buffer UploadTilesToDevice(Device device)
        {
            unsafe
            {
                gpuBufferSize = (nuint)(sizeof(Tile) * tiles.Count);

                var buffer = device.CreateBuffer(
                    new WebGpuSharp.BufferDescriptor()
                    {
                        Size = gpuBufferSize,
                        Usage = BufferUsage.Storage | BufferUsage.CopyDst,
                        Label = "buffer_tiles"
                    });

                device.GetQueue().WriteBuffer(buffer, 0, tiles);
                
                return buffer;
            }
        }


        public static BindGroupLayout GetBindGroupLayout(GraphicsContext graphics)
        {
            if (layout == null)
            {
                layout = graphics.device.CreateBindGroupLayout(new BindGroupLayoutDescriptor() { Entries = stackalloc BindGroupLayoutEntry[] {
                        new()
                        {
                            Binding = 0,
                            Visibility = ShaderStage.Vertex,
                            Buffer = new()
                            {
                                Type = BufferBindingType.ReadOnlyStorage,
                            }
                        },
                        new()
                        {
                            Binding = 1,
                            Visibility = ShaderStage.Fragment,
                            Texture = new()
                            {
                                ViewDimension = TextureViewDimension._2D,
                                SampleType = TextureSampleType.Float,
                                Multisampled = false,
                            }
                        },
                        new()
                        {
                            Binding = 2,
                            Visibility = ShaderStage.Fragment,
                            Sampler = new() { 
                                //Most have a SamplerBindingType aka not Undefined
                                Type = SamplerBindingType.Filtering 
                            }
                        }
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
                    gpuTextureView = gpuTexture.CreateView(new TextureViewDescriptor()
                    {
                        Format = graphics.applicationpreferredFormat,
                        Dimension = TextureViewDimension._2D,
                        MipLevelCount = 1,
                        ArrayLayerCount = 1
                    });
                }

                if (gpuBuffer == null)
                    gpuBuffer = UploadTilesToDevice(graphics.device);

                // Check that the boxes are uploaded as buffer
                // Create a sampler
                var a = new BindGroupEntry[] {
                    new()
                    {
                        Binding = 0,
                        Buffer = gpuBuffer,
                        Size = gpuBufferSize,
                    },
                    new()
                    {
                        Binding = 1,
                        TextureView = gpuTextureView!
                    },
                    new()
                    {
                        Binding = 2,
                        Sampler = graphics.defaultSampler
                    }
                };
                bindgroup = graphics.device.CreateBindGroup(new BindGroupDescriptor(){
                    Layout = layout, 
                    Entries = a
                });
            }
            return bindgroup;
        }
    }
}