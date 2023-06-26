using System;
using System.Numerics;
using System.Reflection;
using System.Resources;
using System.Diagnostics;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;

using Saket.Engine;

using Saket.Engine.Resources.Databases;
using Saket.Engine.Resources.Loaders;
using Saket.Engine.Typography;

using Saket.ECS;
using Saket.Engine.Graphics.SDF;
using Saket.Graphics;
using Saket.Engine.Math.Geometry;
using System.Collections;
using System.Linq;
using System.IO;
using Saket.Engine.Graphics;
using Saket.Engine.Graphics.Packing;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Saket.Engine.Example
{

    class UIProgram : Application
    {
        float aspectRatio;

        World world;


        RendererSpriteSimple spriteRenderer;

        Pipeline pipeline_update = new Pipeline();
        Pipeline pipeline_render = new Pipeline();

        Dear_ImGui_Sample.ImGuiController controller_ui;

        ResourceManager resources;

        float fill = -0.5f;
        Font font;

        public UIProgram(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            resources = new ResourceManager();
            resources.databases.Add(new DatabaseEmbedded(Assembly.GetExecutingAssembly()));
            resources.RegisterLoader(new LoaderTexture());
            resources.RegisterLoader(new LoaderShader());
        }

        protected override void OnLoad()
        {
            world = new World();


            var window = world.CreateEntity();








           
            var entity_camera = world.CreateEntity();
            entity_camera.Add(new Transform2D());
            entity_camera.Add(new CameraOrthographic(32, 0.1f, 100f));
            controller_ui = new Dear_ImGui_Sample.ImGuiController(1280, 720);
            // Initialize graphics resources
            spriteRenderer = new RendererSpriteSimple(1000, entity_camera, resources.Load<Shader>("sdf"), (shader) =>
            {
                shader.SetFloat("time", fill);
            });

            var teste = world.CreateEntity();
            teste.Add(new Sprite(0, 0, int.MaxValue));
            teste.Add(new Transform2D(0f, 0f, 0, 0, 20, 20));

            Stage stage_update = new Stage();
            //stage_update.Add()
            pipeline_update.AddStage(stage_update);

            Stage stage_render = new Stage();
            stage_render.Add(spriteRenderer.SystemSpriteRenderer);
            pipeline_render.AddStage(stage_render);
        }



        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            float delta = (float)args.Time;
            controller_ui.Update(this, delta);
            pipeline_update.Update(world, delta);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            float delta = (float)args.Time;
            GL.ClearColor(0f, 0, 0, 1f);
            GL.ClearStencil(0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.StencilBufferBit | ClearBufferMask.DepthBufferBit);

            pipeline_render.Update(world, delta);
            controller_ui.Render();
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            aspectRatio = (float)e.Width / (float)e.Height;
            GL.Viewport(0, 0, e.Width, e.Height);
            controller_ui.WindowResized(e.Width, e.Height);
        }
    }
}