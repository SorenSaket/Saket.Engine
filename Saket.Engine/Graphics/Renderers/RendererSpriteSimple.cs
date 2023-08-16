using Saket.ECS;
using Saket.Engine.Collections;
using Saket.Engine.Components;
using Saket.Engine.Graphics;
using Saket.Engine.Math.Geometry;
using Saket.Engine.Typography.TrueType;
using Saket.WebGPU;
using Saket.WebGPU.Native;
using Saket.WebGPU.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;



namespace Saket.Engine.Graphics.Renderers
{
    // Todo
    // Multiple texture support
    // multiple shader support
    // Spritesheet support
    // Sprite Flipping

    // TODO manual sprite rendering without the need of entities
    // 


    /// <summary>
    /// 
    /// </summary>
    public class RendererSpriteSimple
    {
        /// <summary>
        /// Query all entities with Sprite and Transform
        /// </summary>
        static readonly Query query_spriteTransform = new Query().With<(Sprite, Transform2D)>();

        /// <summary>
        /// Query all entities with Sprite and Transform
        /// </summary>
        static readonly Query query_spriteAnimator = new Query().With<(Sprite, SpriteAnimator)>();

        readonly Transform2D[] elements_transform;
        readonly nint buffer_transform;
        readonly ulong size_bufferTransform;

        readonly Sprite[] elements_sprite;
        readonly nint buffer_sprite;
        readonly ulong size_bufferSprite;

        readonly uint batchCount;

        public unsafe RendererSpriteSimple(uint batchCount, Device device)
        {
            this.batchCount = batchCount;

            // Transform buffer 
            fixed(void* ptr_label = "buffer_spriterenderer_transform"u8)
            {
                // Allocate cpu side array
                elements_transform = new Transform2D[batchCount];
                // Cache the size of the array
                size_bufferTransform = (ulong)(batchCount * sizeof(Transform2D));

                // Create the gpu side buffer
                WGPUBufferDescriptor bufferDescriptor = new()
                {
                    usage = WGPUBufferUsage.CopyDst | WGPUBufferUsage.Vertex,
                    size = size_bufferTransform,
                    label = (char*)ptr_label
                };
                buffer_transform = wgpu.DeviceCreateBuffer(device.Handle, bufferDescriptor);

            }

            // Sprite buffer 
            fixed (void* ptr_label = "buffer_spriterenderer_sprite"u8)
            {
                elements_sprite = new Sprite[batchCount];
                size_bufferSprite = (ulong)(batchCount * sizeof(Sprite));

                WGPUBufferDescriptor bufferDescriptor = new()
                {
                    usage = WGPUBufferUsage.CopyDst | WGPUBufferUsage.Vertex,
                    size = size_bufferTransform,
                    label = (char*)ptr_label
                };

                buffer_sprite = wgpu.DeviceCreateBuffer(device.Handle, bufferDescriptor);
            }
        }

        public unsafe void SetbuffersAndDraw(nint Queue, nint RenderPassEncoder, uint instanceCount)
        {
            fixed (void* ptr_transform = elements_transform)
            {
                wgpu.QueueWriteBuffer(Queue, buffer_transform, 0, ptr_transform, (nuint)(instanceCount * sizeof(Transform2D)));
            }
            fixed (void* ptr_sprite = elements_sprite)
            {
                wgpu.QueueWriteBuffer(Queue, buffer_sprite, 0, ptr_sprite, (nuint)(instanceCount * sizeof(Sprite)));
            }

            wgpu.RenderPassEncoderSetVertexBuffer(RenderPassEncoder, 0, buffer_transform, 0, size_bufferTransform);
            wgpu.RenderPassEncoderSetVertexBuffer(RenderPassEncoder, 1, buffer_sprite, 0, size_bufferSprite);

            

            wgpu.RenderPassEncoderDraw(RenderPassEncoder, 6, instanceCount, 0, 0);
        }


        List<Archetype> archetypes = new();

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




#if false
        /// <summary>
        /// The shader to use for rendering
        /// </summary>
        public Shader Shader;
       
        // VAO
        private int _vertexArrayObject;

        // BUFFERS
        // STATIC
        private int _quadBufferObject;
        
        // DYNAMIC
        private int buffer_elements;


        /// <summary>
        /// A Dictionary containing tgindicies and gl buffer indicies 
        /// </summary>
        private Dictionary<TextureAtlas, int> buffer_boxes = new();

        SpriteElement[] Elements;


        public Action<Shader>? ShaderFunction;
        /// <summary>
        ///  How many elements can fit into one batch.
        ///  The higher the value the more memory the buffers will take on cpu and gpu
        ///  But will reduce the number of drawcall
        ///  Adjust this in your game to fit your requirements.
        /// </summary>
        private int batchCount;

        private Entity entity_camera;


        private Queue<int> nextGroups = new();
        private Stack<int> renderedGroups = new();

        public RendererSpriteSimple(int batchCount, Entity camera, Shader shader, Action<Shader>? shaderFunction = null)
        {
            this.entity_camera = camera;
            this.batchCount = batchCount;

            // load the shader
            this.Shader = shader;
            this.ShaderFunction = shaderFunction;
            Elements = new SpriteElement[batchCount];

            // create buffers
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            // location of the specifc buffer matching those defined in the shader
            // incremented each time a buffer is created
            int layoutLocation = 0;

            // QuadVert buffer
            {
                // Is there a way to do this would a buffer?
                // Maybe directly in the shader


                // CCW winding order
                // TL---TR
                // | \  |
                // |  \ |
                // BL---BR
                const float size = 0.5f;
                ReadOnlySpan<float> quadVertices = stackalloc float[]{
			        // positions  // UVS
			        -size, -size, 0,0, // BL
                     size, -size, 1,0, // BR
                    -size,  size, 0,1, // TL

                    -size, size, 0,1,  // TL
                     size, -size, 1,0, // BR
                    size,  size,1,1, // TR
                };
                unsafe
                {
                    fixed (void* ptr = quadVertices)
                    {
                        _quadBufferObject = GL.GenBuffer();
                        GL.BindBuffer(BufferTarget.ArrayBuffer, _quadBufferObject);
                        GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * quadVertices.Length, (nint)ptr, BufferUsageHint.StaticDraw);
                    }
                }

                // POS
                GL.VertexAttribPointer(layoutLocation, 2, VertexAttribPointerType.Float, false, sizeof(float) * 4, 0);
                GL.EnableVertexAttribArray(layoutLocation);

                // UV
                layoutLocation++;
                GL.VertexAttribPointer(layoutLocation, 2, VertexAttribPointerType.Float, false, sizeof(float) * 4, sizeof(float) * 2);
                GL.EnableVertexAttribArray(layoutLocation);
            }
           
            

              
           
            
            
            buffer_elements = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_elements);
            //  create and initialize a buffer object's data store
            GL.BufferData(BufferTarget.ArrayBuffer, batchCount * SpriteElement.size, IntPtr.Zero, BufferUsageHint.DynamicDraw);


            // POSITIONS
            layoutLocation++;
            GL.VertexAttribPointer(layoutLocation, 3, VertexAttribPointerType.Float, false, SpriteElement.size, 0);
            GL.EnableVertexAttribArray(layoutLocation);
            GL.VertexAttribDivisor(layoutLocation, 1);

            // Rotation
            layoutLocation++;
            GL.VertexAttribPointer(layoutLocation, 1, VertexAttribPointerType.Float, false, SpriteElement.size, sizeof(float) * 3);
            GL.EnableVertexAttribArray(layoutLocation);
            GL.VertexAttribDivisor(layoutLocation, 1);

            // Scale
            layoutLocation++;
            GL.VertexAttribPointer(layoutLocation, 2, VertexAttribPointerType.Float, false, SpriteElement.size, sizeof(float) * 4);
            GL.EnableVertexAttribArray(layoutLocation);
            GL.VertexAttribDivisor(layoutLocation, 1);

            // Box Index
            layoutLocation++;
            GL.VertexAttribIPointer(layoutLocation, 1, VertexAttribIntegerType.Int, SpriteElement.size, new IntPtr(sizeof(float) * 7));
            GL.EnableVertexAttribArray(layoutLocation);
            GL.VertexAttribDivisor(layoutLocation, 1);

            // Color
            layoutLocation++;
            GL.VertexAttribPointer(layoutLocation, 4, VertexAttribPointerType.UnsignedByte, true, SpriteElement.size, sizeof(float) * 8);
            GL.EnableVertexAttribArray(layoutLocation);
            GL.VertexAttribDivisor(layoutLocation, 1);



            //GL.BufferData(BufferTarget.ShaderStorageBuffer, batchCount * sizeof(float) * 4, IntPtr.Zero, BufferUsageHint.DynamicDraw);
        }

        public void ReadyShader(Shader shader, ICamera camera)
        {
            this.Shader = shader;
            // Use the current shader
            Shader.Use();
            // Enable depth testing
            GL.DepthMask(true);
            GL.Enable(EnableCap.DepthTest);
            GL.ColorMask(true, true, true, true);
            Shader.SetMatrix4("view", camera.ViewMatrix);
            Shader.SetMatrix4("projection", camera.ProjectionMatrix);
        }

        /// <summary>
        /// Draw a Spritebatch 
        /// </summary>
        /// <param name="targetTG"></param>
        /// <param name="texture"></param>
        /// <param name="atlas"></param>
        /// <param name="count"></param>
        /// <exception cref="ArgumentException"></exception>
        public void DrawBatch(SpriteElement[] sprites, TextureAtlas atlas, int count)
        {
            if (count == 0|| atlas == null )
                throw new ArgumentException();

            // Bind the element buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_elements);
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, SpriteElement.size, sprites);
        

            ReadOnlySpan<Tile> tiles = CollectionsMarshal.AsSpan(atlas.tiles);

            unsafe
            {
                fixed (void* ptr = tiles)
                {
                    // Create or Bind Box buffer
                    if (!buffer_boxes.ContainsKey(atlas))
                    {
                        int nb = GL.GenBuffer();
                        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, nb);
                        GL.BufferData(BufferTarget.ShaderStorageBuffer, atlas.tiles.Count * Marshal.SizeOf<Tile>(), (nint)ptr , BufferUsageHint.DynamicDraw);
                        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, nb);
                        buffer_boxes.Add(atlas, nb);
                    }
                    else
                    {
                        // TODO: FIX buffers cant expand? the sheet.rects can be longer than it was when intially created
                        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, buffer_boxes[atlas]);
                        GL.BufferSubData(BufferTarget.ShaderStorageBuffer, (IntPtr)0, atlas.tiles.Count * Marshal.SizeOf<Tile>(), (nint)ptr);
                        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, buffer_boxes[atlas]);
                    }
                }
            }
           

            GL.BindTexture(TextureTarget.Texture2D, atlas.texture.handle);

            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, count);
        }


        public void SystemSpriteAnimation(World world)
        {

            Animations animations = world.GetResource<Animations>();
            if (animations == null)
                return;

            var entities = world.Query(query_spriteAnimator);

            foreach (var entity in entities)
            {
                var animator = entity.Get<SpriteAnimator>();
                var sprite = entity.Get<Sprite>();

                animator.timer += animator.speed * world.Delta;

                sprite.spr = animations.animations[animator.animation][(int)(animator.timer % animations.animations[animator.animation].Length)];

                entity.Set(sprite);
                entity.Set(animator);
            }
        }


        /// <summary>
        ///  
        /// </summary>
        /// <param name="world"></param>
        public void SystemSpriteRenderer(World world)
        {
            var textureGroups = world.GetResource<List<TextureAtlas>>();
            if (textureGroups == null)
                return;

            // Use the current shader
            Shader.Use();

            //GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            //GL.BlendEquation(BlendEquationMode.);
            //GL.BlendFuncSeparate(BlendingFactorSrc.One, BlendingFactorDest.Zero, BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            
            // Enable depth testing
            GL.DepthMask(true);
            GL.Enable(EnableCap.DepthTest);
            GL.ColorMask(true, true, true, true);
            // Todo. Camera based rendering
            var cam = entity_camera.Get<CameraOrthographic>();
            Shader.SetMatrix4("view", cam.viewMatrix);
            Shader.SetMatrix4("projection", cam.projectionMatrix);

            ShaderFunction?.Invoke(Shader);
            
            // Query the world for matching enetities
            QueryResult entities = world.Query(query_spriteTransform);
            
            nextGroups.Clear();
            renderedGroups.Clear();
            ///<summary>The current number of elements in batch</summary> 
            int i = 0;
            ///<summary>The current texture group target</summary> 
            int targetTG = -1;

            // Loops through all sprites as many times that there different textures
            // Each loop only draws one texture
            // This is probably stupid. It incurs a lot of chache misses since it has to loop through memory N times.
            // Instead build up a data structure that stores the individual batches and only loop once
            do
            {
                i = 0;
                if (nextGroups.TryDequeue(out var nextGroup))
                    targetTG = nextGroup;

                foreach (var entity in entities)
                {
                    Elements[i].sprite = entity.Get<Sprite>();

                    // decide whether to batch or wait for next iteration
                    if (targetTG == -1)
                    {
                        targetTG = Elements[i].sprite.tex;
                    }
                    else if (targetTG != Elements[i].sprite.tex)
                    {
                        if (!nextGroups.Contains(Elements[i].sprite.tex) && !renderedGroups.Contains(Elements[i].sprite.tex))
                            nextGroups.Enqueue(Elements[i].sprite.tex);
                        continue;
                    }

                    Elements[i].transform = entity.Get<Transform2D>();

                    i++;

                    if (i == batchCount)
                    {
                        // Dispatch draw call
                        renderedGroups.Push(targetTG);
                        DrawBatch(Elements, textureGroups[targetTG], i);
                        i = 0;
                    }
                }
                if(i > 0)
                { 
                    // Dispatch draw call for the remaining elements
                    renderedGroups.Push(targetTG);
                    DrawBatch(Elements, textureGroups[targetTG], i);
                }
               
            } while (nextGroups.Count > 0);
        }

#endif

    }


}