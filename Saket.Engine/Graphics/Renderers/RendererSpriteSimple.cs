using Saket.ECS;
using Saket.Engine.Collections;
using Saket.Engine.Components;
using Saket.Engine.Graphics;
using Saket.Engine.Math.Geometry;
using WebGpuSharp;

using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WebGpuSharp.FFI;

namespace Saket.Engine.Graphics.Renderers;

// TODO(Soren):
// manual sprite rendering without the need of entities
// Multiple texture support
// multiple shader support
// Spritesheet support
// Sprite Flipping


/// <summary>
/// A Renderer that can render simple rects with textures aka. Sprites.  
/// </summary>
public class RendererSpriteSimple
{
    #region Variables

    /// <summary>
    /// Query all entities with Sprite and Transform
    /// </summary>
    static protected readonly Query query_spriteTransform = new Query().With<(Sprite, Transform2D)>();
    /// <summary>
    /// Query all entities with Sprite and Transform
    /// </summary>
    static protected readonly Query query_spriteAnimator = new Query().With<(Sprite, SpriteAnimator)>();

    protected readonly Transform2D[] elements_transform;
    protected readonly WebGpuSharp.Buffer buffer_transform;
    protected readonly ulong size_bufferTransform;

    protected readonly Sprite[] elements_sprite;
    protected readonly WebGpuSharp.Buffer buffer_sprite;
    protected readonly ulong size_bufferSprite;

    protected readonly uint batchCount;

    protected uint currentCount;
    protected GraphicsContext graphics;

    #endregion

    #region Constructors

    /// <param name="graphics"></param>
    /// <param name="maximumBatchCount"> The maximum number of sprites to allocate for a single batch. Increasing this number will increase memory usage but can lower drawcalls. Set according to your games largest batch. </param>
    public unsafe RendererSpriteSimple(GraphicsContext graphics, uint maximumBatchCount = 1000)
    {
        this.batchCount = maximumBatchCount;
        this.graphics = graphics;

        // Transform buffer 
        {
            // Allocate cpu side array
            elements_transform = new Transform2D[maximumBatchCount];
            // Cache the size of the array
            size_bufferTransform = (ulong)(maximumBatchCount * sizeof(Transform2D));

            // Create the gpu side buffer
            BufferDescriptor bufferDescriptor = new()
            {
                Usage = BufferUsage.CopyDst | BufferUsage.Vertex,
                Size = size_bufferTransform,
                Label = "buffer_spriterenderer_transform"
            };
            buffer_transform = graphics.device.CreateBuffer(bufferDescriptor);

        }

        // Sprite buffer 
        {
            elements_sprite = new Sprite[maximumBatchCount];
            size_bufferSprite = (ulong)(maximumBatchCount * sizeof(Sprite));

            BufferDescriptor bufferDescriptor = new()
            {
                Usage = BufferUsage.CopyDst | BufferUsage.Vertex,
                Size = size_bufferTransform,
                Label = "buffer_spriterenderer_sprite"
            };

            buffer_sprite = graphics.device.CreateBuffer(bufferDescriptor);
        }
    }

    #endregion

    #region ECS Integration

    protected List<Archetype> archetypes = new();

    public unsafe void SetbuffersAndDraw(Queue Queue, RenderPassEncoder RenderPassEncoder, uint instanceCount)
    {
        fixed (void* ptr_transform = elements_transform)
        {
            graphics.queue.WriteBuffer(buffer_transform, 0, ptr_transform, (nuint)(instanceCount * sizeof(Transform2D)));
        }
        fixed (void* ptr_sprite = elements_sprite)
        {
            WebGPU_FFI.QueueWriteBuffer( graphics.queue., buffer_sprite, 0, ptr_sprite, (nuint)(instanceCount * sizeof(Sprite)));
        }

        RenderPassEncoder.SetVertexBuffer( 0, buffer_transform, 0, size_bufferTransform);
        RenderPassEncoder.SetVertexBuffer( 1, buffer_sprite, 0, size_bufferSprite);
        RenderPassEncoder.Draw( 6, instanceCount, 0, 0);
    }

    /// <summary>
    ///  
    /// </summary>
    /// <param name="world"></param>
    public void IterateForRender(World world, Action<uint> RenderAction, Query? query = null)
    {
        if (query != null)
        {
            if (!query.Inclusive.Contains(typeof(Transform2D)) || !query.Inclusive.Contains(typeof(Sprite)))
            {
                throw new Exception("Query needs to contain Transform2D and Sprite");
            }
            // Query the world for matching enetities
            world.QueryArchetypes(query, ref archetypes, out _);
        }
        else
        {
            // Query the world for matching enetities
            world.QueryArchetypes(query_spriteTransform, ref archetypes, out _);
        }

        uint count = 0;
        unsafe
        {
            foreach (var archetype in archetypes)
            {
                Transform2D* transforms = archetype.GetUnsafe<Transform2D>(0);
                Sprite* sprites = archetype.GetUnsafe<Sprite>(0);

                foreach (var index_entity in archetype)
                {
                    elements_transform[count] = transforms[index_entity];
                    elements_sprite[count] = sprites[index_entity];
                    count++;

                    if (count >= batchCount)
                    {
                        RenderAction(count);
                        count = 0;
                    }
                }
            }
        }
        // Render remaining
        if (count > 0)
        {
            RenderAction(count);
        }
    }

    #endregion

    #region Immediate mode API

    // TODO(Soren): create draw overloads that doesn't use propriatary structs

    public void Draw(Sprite sprite, Transform2D transform)
    {
        elements_sprite[currentCount] = sprite;
        elements_transform[currentCount] = transform;
        currentCount++;
    }

    /// <summary>
    /// Submit batch to specfic TextureView
    /// </summary>
    /// <param name="target"></param>
    public void SubmitBatch(TextureView target, RenderPipeline renderPipeline, TextureAtlas atlas)
    {
        unsafe
        {
            WGPURenderPassColorAttachment renderPassColorAttachment = new()
            {
                view = target.Handle,
                resolveTarget = 0,
                loadOp = WGPULoadOp.Load,
                storeOp = WGPUStoreOp.Store,
            };
            WGPURenderPassDescriptor renderPassDesc = new()
            {
                colorAttachmentCount = 1,
                colorAttachments = &renderPassColorAttachment,
                depthStencilAttachment = null,
                timestampWriteCount = 0,
                timestampWrites = null,
                nextInChain = null,
            };

            // Command Encoder
            nint commandEncoder = wgpu.DeviceCreateCommandEncoder(graphics.device.Handle, new() { });

            nint RenderPassEncoder = wgpu.CommandEncoderBeginRenderPass(commandEncoder, renderPassDesc);

            // Set the pipline for the renderpass
            wgpu.RenderPassEncoderSetPipeline(RenderPassEncoder, renderPipeline.Handle);

            // Set system bind group
            // TODO
            wgpu.RenderPassEncoderSetBindGroup(RenderPassEncoder, 0, graphics.systemBindGroup.Handle, 0, (uint*)0);
            // Set atlas/sampler bindgroup
            wgpu.RenderPassEncoderSetBindGroup(RenderPassEncoder, 1, atlas.GetBindGroup(graphics).Handle, 0, (uint*)0);
            // set vertex buffers and Submit actual draw comand
            SetbuffersAndDraw(graphics.queue.Handle, RenderPassEncoder, currentCount);

            // Finish Rendering
            wgpu.RenderPassEncoderEnd(RenderPassEncoder);

            nint commandBuffer = wgpu.CommandEncoderFinish(commandEncoder, new() { });

            wgpu.QueueSubmit(graphics.queue.Handle, 1, new IntPtr(&commandBuffer));

            wgpu.RenderPassEncoderRelease(RenderPassEncoder);
            wgpu.CommandEncoderRelease(commandEncoder);

            currentCount = 0;
        }
    }

    public void ClearBatch()
    {
        currentCount = 0;
    }

    #endregion



}