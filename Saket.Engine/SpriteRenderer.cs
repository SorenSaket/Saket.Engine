using OpenTK.Graphics.OpenGL4;
using Saket.ECS;

using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Xml.Linq;


namespace Saket.Engine
{
    // From https://github.com/mellinoe/Snake/blob/master/SpriteRenderer.cs
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

        // TL---TR
        // | \  |
        // |  \ |
        // BL---BR
        const float size = 0.5f;
        float[] quadVertices = new float[]{
			// positions  // UVS
			-size, -size, 0,0,
             size, -size, 1,0,
             size,  size, 1,1,

            -size, -size, 0,0,
             size,  size, 1,1,
             -size,  size, 0,1,
        };


        // The spriterenderer is designed to batch 1000 sprites together per batch
        static readonly Query query = new Query().With<Sprite>().With<Transform2D>();

        private spriteElement[] elements;

    
        /// <summary>
        ///  How many elements can fit into one batch.
        ///  The higher the value the more memory the buffers will take on cpu and gpu
        ///  But will reduce the number of drawcall
        ///  Adjust this in your game to fit your requirements.
        /// </summary>
        private int batchCount;

        private Entity entity_camera;

        public SpriteRenderer(int batchCount, Entity camera, Shader shader)
        {
            this.entity_camera = camera;
            this.batchCount = batchCount;

            // load the shader
            this.shader = shader;

            elements = new spriteElement[batchCount];

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
            GL.BufferData(BufferTarget.ArrayBuffer, batchCount * spriteElement.size, IntPtr.Zero, BufferUsageHint.DynamicDraw);


            // POSITIONS
            layoutLocation++;
            GL.VertexAttribPointer(layoutLocation, 2, VertexAttribPointerType.Float, false, spriteElement.size, 0);
            GL.EnableVertexAttribArray(layoutLocation);
            GL.VertexAttribDivisor(layoutLocation, 1);

            // Rotation
            layoutLocation++;
            GL.VertexAttribPointer(layoutLocation, 1, VertexAttribPointerType.Float, false, spriteElement.size, sizeof(float) * 3);
            GL.EnableVertexAttribArray(layoutLocation);
            GL.VertexAttribDivisor(layoutLocation, 1);

            // Scale
            layoutLocation++;
            GL.VertexAttribPointer(layoutLocation, 2, VertexAttribPointerType.Float, false, spriteElement.size, sizeof(float) * 4);
            GL.EnableVertexAttribArray(layoutLocation);
            GL.VertexAttribDivisor(layoutLocation, 1);

            // Color
            layoutLocation++;
            GL.VertexAttribPointer(layoutLocation, 4, VertexAttribPointerType.UnsignedByte, true, spriteElement.size, sizeof(float) * 8);
            GL.EnableVertexAttribArray(layoutLocation);
            GL.VertexAttribDivisor(layoutLocation, 1);
        }


        /// <summary>
        ///  
        /// </summary>
        /// <param name="world"></param>
        public void SystemSpriteRenderer(World world)
        {
            shader.Use();

            var cam = entity_camera.Get<CameraOrthographic>();
            shader.SetMatrix4("view", cam.viewMatrix);
            shader.SetMatrix4("projection", cam.projectionMatrix);

            // Query the world for matching enetities
            QueryResult entities = world.Query(query);

            // Bind the element buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer_elements);
            
            int i = 0;
            void DispatchDrawCall()
            {
                GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, i * spriteElement.size, elements);
                GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, i);
            }

            foreach (var entity in entities)
            {
                elements[i].transform   = entity.Get<Transform2D>();
                elements[i].sprite      = entity.Get<Sprite>();
                i++;
                if(i == batchCount)
                {
                    // Dispatch draw call
                    DispatchDrawCall();
                    i = 0;
                }
            }

            // Dispatch draw call for the last
            DispatchDrawCall();
        }



        [StructLayout(LayoutKind.Sequential)]
        private struct spriteElement
        {
            public static int size = 36;
            // Size = 32*6 + 32*3
            public Transform2D transform;
            public Sprite sprite;
        }
    }
}