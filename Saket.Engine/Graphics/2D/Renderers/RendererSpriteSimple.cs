using Saket.ECS;
using Saket.Engine.Collections;
using Saket.Engine.Components;
using Saket.Engine.Graphics;
using WebGpuSharp;

using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WebGpuSharp.FFI;

namespace Saket.Engine.Graphics.D2.Renderers;

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
            buffer_transform = graphics.device.CreateBuffer(bufferDescriptor) ?? throw new Exception("");

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
        //fixed (void* ptr_transform = elements_transform)
        {
            graphics.queue.WriteBuffer(buffer_transform, 0, elements_transform.AsSpan().Slice(0, (int)instanceCount));
        }
        //fixed (void* ptr_sprite = elements_sprite)
        {
            graphics.queue.WriteBuffer(buffer_sprite, 0, elements_sprite.AsSpan().Slice(0, (int)instanceCount));
        }

        RenderPassEncoder.SetVertexBuffer( 0, buffer_transform, 0, size_bufferTransform);
        RenderPassEncoder.SetVertexBuffer( 1, buffer_sprite, 0, size_bufferSprite);
        RenderPassEncoder.Draw(6, instanceCount, 0, 0);
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

        unsafe
        {
            foreach (var archetype in archetypes)
            {
                Transform2D* transforms = archetype.GetUnsafe<Transform2D>(0);
                Sprite* sprites = archetype.GetUnsafe<Sprite>(0);

                foreach (var index_entity in archetype)
                {
                    elements_transform[currentCount] = transforms[index_entity];
                    elements_sprite[currentCount] = sprites[index_entity];
                    currentCount++;

                    // We've reached the max capacity in the cpu buffer. render batch early. this will result in one or more batches
                    if (currentCount >= batchCount)
                    {
                        RenderAction(currentCount);
                        currentCount = 0;
                    }
                }
            }
        }
        // Render remaining
        if (currentCount > 0)
        {
            RenderAction(currentCount);
        }
    }

    #endregion

    #region Immediate mode API

    // TODO(Soren): create draw overloads that doesn't use propriatary structs

    public void Draw(Sprite sprite, Transform2D transform)
    {
        //if (currentCount >= batchCount)
         //   SubmitBatch();
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
            
            RenderPassDescriptor renderPassDesc = new()
            {
                ColorAttachments = new RenderPassColorAttachment[]
                {
                    new()
                    {
                        View = target,
                        ResolveTarget = default,
                        LoadOp = LoadOp.Load,
                        StoreOp = StoreOp.Store,
                    }
                },
                DepthStencilAttachment = null,
            };

            // Command Encoder
            var commandEncoder = graphics.device.CreateCommandEncoder( new() { });

            var RenderPassEncoder = commandEncoder.BeginRenderPass(renderPassDesc);

            // Set the pipline for the renderpass
            RenderPassEncoder.SetPipeline(renderPipeline);

            // Set system bind group
            // TODO
            RenderPassEncoder.SetBindGroup(0, graphics.systemBindGroup);
            // Set atlas/sampler bindgroup
            RenderPassEncoder.SetBindGroup(1, atlas.GetBindGroup(graphics));


            // set vertex buffers and Submit actual draw comand
            SetbuffersAndDraw(graphics.queue, RenderPassEncoder, currentCount);

            // Finish Rendering
            RenderPassEncoder.End();

            var commandBuffer = commandEncoder.Finish( new() { });

            graphics.queue.Submit(commandBuffer);

            currentCount = 0;
        }
    }

    public void ClearBatch()
    {
        currentCount = 0;
    }

    #endregion
}