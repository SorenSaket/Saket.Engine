using OpenTK.Compute.OpenCL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Saket.ECS;
using Saket.Engine;
using Saket.Engine.Resources.Databases;
using Saket.Engine.Resources.Loaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TechWars.Client;
using TechWars.Shared;

namespace TechWars
{
    internal class Game : Application
    {
        World world;

        SpriteRenderer spriteRenderer;

        Pipeline pipeline_update;
        Pipeline pipeline_render;

        Entity entity_camera;
        ResourceManager resourceManager;


        bool hasStarted = false;
        NetServer server;
        NetClient client;

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            
        }
        
        protected override void OnLoad()
        {
            // Initialize graphics resources
            resourceManager = new ResourceManager();
            resourceManager.databases.Add(new DatabaseEmbedded(Assembly.GetExecutingAssembly()));
            resourceManager.RegisterLoader<Shader>(new LoaderShader());
            
            world = new World();
            world.SetResource(KeyboardState);
            world.SetResource(MouseState);
			world.SetResource(new LocalInputBuffer<TechWars.PlayerInput>());
			world.SetResource(new WindowInfo(base.Size.X, base.Size.Y));

			entity_camera = world.CreateEntity();
            entity_camera.Add(new Transform2D());
            entity_camera.Add(new CameraOrthographic(10, 0.1f, 100f));

            spriteRenderer = new SpriteRenderer(1000, entity_camera, resourceManager.Load<Shader>("sprite") );
			
			// Update pipeline
			pipeline_update = new();
			Stage stage_update = new ();
            stage_update.Add(Camera.CameraSystem2D);
            stage_update.Add(Camera.CameraSystemOrthographic);
            pipeline_update.AddStage(stage_update);

			// Render pipeline
			pipeline_render = new();
			Stage stage_render = new ();
            stage_render.Add(spriteRenderer.SystemSpriteRenderer);
            pipeline_render.AddStage(stage_render);
		}


        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            float delta = (float)args.Time;

            if (!hasStarted)
            {
                // Start host
                if (KeyboardState.IsKeyDown(Keys.H))
                {
                    server = new NetServer(9050);
                    client = new NetClient(world);
                    hasStarted = true;
                }
                else if (KeyboardState.IsKeyDown(Keys.C))
                {
                    client = new NetClient(world);
                    hasStarted = true;
                }
                world.SetResource(client);
                return;
            }

			pipeline_update.Update(world, delta);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            float delta = (float)args.Time;
            GL.ClearColor(1f, 0, 0, 1f);
            GL.ClearStencil(0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.StencilBufferBit | ClearBufferMask.DepthBufferBit);

            pipeline_render.Update(world, delta);

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {   
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }
}
