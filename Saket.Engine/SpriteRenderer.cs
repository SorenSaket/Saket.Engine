using OpenTK.Graphics.OpenGL4;
using Saket.ECS;
using Saket.Engine.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Xml.Linq;


namespace Saket.Engine
{
    // Todo
    // Multiple texture support
    // multiple shader support
    // Spritesheet support
    // Sprite Flipping

    // 
    public class SpriteRenderer
    {
        public Shader shader;
       
        // VAO
        private int _vertexArrayObject;

        // BUFFERS
        // STATIC
        private int _quadBufferObject;
        
        // DYNAMIC
        private int buffer_elements;


        private Dictionary<int,int> buffer_boxes = new();


        // CCW winding order
        // TL---TR
        // | \  |
        // |  \ |
        // BL---BR
        const float size = 0.5f;
        float[] quadVertices = new float[]{
			// positions  // UVS
			-size, -size, 0,0, // BL
             size, -size, 1,0, // BR
            -size,  size, 0,1, // TL

            -size, size, 0,1,  // TL
             size, -size, 1,0, // BR
            size,  size,1,1, // TR
        };

        /// <summary>
        /// Query all entities with Sprite and Transform
        /// </summary>
        static readonly Query query = new Query().With<(Sprite, Transform2D)>();

        /// <summary>
        /// Array of elements in batch to be dispatched to gpu
        /// </summary>
        private SpriteElement[] elements;

    
        /// <summary>
        ///  How many elements can fit into one batch.
        ///  The higher the value the more memory the buffers will take on cpu and gpu
        ///  But will reduce the number of drawcall
        ///  Adjust this in your game to fit your requirements.
        /// </summary>
        private int batchCount;

        private Entity entity_camera;


        private Queue<int> nextGroups = new();

        public SpriteRenderer(int batchCount, Entity camera, Shader shader)
        {
            this.entity_camera = camera;
            this.batchCount = batchCount;

            // load the shader
            this.shader = shader;

            elements = new SpriteElement[batchCount];

            // create buffers
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            // location of the specifc buffer matching those defined in the shader
            // incremented each time a buffer is created
            int layoutLocation = 0;

            // QuadVert buffer
            {
                _quadBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, _quadBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * quadVertices.Length, quadVertices, BufferUsageHint.StaticDraw);

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
            GL.VertexAttribPointer(layoutLocation, 2, VertexAttribPointerType.Float, false, SpriteElement.size, 0);
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



            GL.BufferData(BufferTarget.ShaderStorageBuffer, batchCount * sizeof(float) * 4, IntPtr.Zero, BufferUsageHint.DynamicDraw);
        }


        /// <summary>
        ///  
        /// </summary>
        /// <param name="world"></param>
        public void SystemSpriteRenderer(World world)
        {
            var textureGroups = world.GetResource<TextureGroups>();
            if (textureGroups == null)
                return;


            // Use the current shader
            shader.Use();

            // Todo. Camera based rendering
            var cam = entity_camera.Get<CameraOrthographic>();
            shader.SetMatrix4("view", cam.viewMatrix);
            shader.SetMatrix4("projection", cam.projectionMatrix);

            // Query the world for matching enetities
            QueryResult entities = world.Query(query);
            
            nextGroups.Clear();

            ///<summary>The current number of elements in batch</summary> 
            int i = 0;
            ///<summary>The current texture group target</summary> 
            int targetTG = -1;

            void DispatchDrawCall()
            {
                if (i == 0 || targetTG == -1)
                    return;
                // Bind the element buffer
                GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_elements);
                GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, i * SpriteElement.size, elements);

                // Create or Bind Box buffer
                if (!buffer_boxes.ContainsKey(targetTG))
                {
                    int nb = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.ShaderStorageBuffer, nb);
                    GL.BufferData(BufferTarget.ShaderStorageBuffer, textureGroups.groups[targetTG].sheet.rects.Length * Marshal.SizeOf<SheetElement>(), textureGroups.groups[targetTG].sheet.rects, BufferUsageHint.DynamicDraw);
                    GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, nb);
                    buffer_boxes.Add(targetTG, nb);
                }
                else
                {
                    GL.BindBuffer(BufferTarget.ShaderStorageBuffer, buffer_boxes[targetTG]);
                    GL.BufferSubData(BufferTarget.ShaderStorageBuffer, (IntPtr)0, textureGroups.groups[targetTG].sheet.rects.Length * Marshal.SizeOf<SheetElement>(), textureGroups.groups[targetTG].sheet.rects);
                    GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, buffer_boxes[targetTG]);
                }

                GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, i);
            }

            // Loops through all sprites as many times that there different textures
            // Each loop only draws one texture
            do
            {
                foreach (var entity in entities)
                {
                    elements[i].sprite = entity.Get<Sprite>();

                    // decide whether to batch or wait for next iteration
                    if (targetTG == -1)
                    {
                        targetTG = elements[i].sprite.tex;
                    }
                    else if (targetTG != elements[i].sprite.tex)
                    {
                        if (!nextGroups.Contains(elements[i].sprite.tex))
                            nextGroups.Enqueue(elements[i].sprite.tex);
                        continue;
                    }

                    elements[i].transform = entity.Get<Transform2D>();

                    i++;

                    if (i == batchCount)
                    {
                        // Dispatch draw call
                        DispatchDrawCall();
                        i = 0;
                    }
                }
                // Dispatch draw call for the remaining elements
                DispatchDrawCall();
            } while (nextGroups.Count > 0);



        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SpriteElement
        {
            public static readonly int size = 36; // this is prone to error. Just do marshal.sizeof(), sizeof()
            // Size = 32*6 + 32*3
            public Transform2D transform;
            public Sprite sprite;
        }
    }
}