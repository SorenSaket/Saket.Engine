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
using Saket.Engine.GUI;
using Saket.Engine.GUI.Styling;
using Saket.Engine.Graphics.Text;

namespace Saket.Engine.Example
{

    class UIProgram : Application
    {
        World world;
        Document document;


        float aspectRatio;



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
            // Create a new world
            // This is where everything is stored
            world = new World();

            // We have to give the document reference to the world since GUI elements are regular entities
            // Optionally can give the document a seperate world so that it's seperate from gameplay entities
            document = new Document(world);

            // Load a font and add it to the document


            // Root entity that will act as our window
            // a window will automatically be created for each root entity?
            var window = document.CreateGUIEntity(new GUIEntityInfo(default, "window_main", new()
            {
                Width = new(1280),
                Height = new(720)
            }));
            
            // Create a gui entity that will act as a button
            // There are no buildt in components by default 
            // Everything can act as a button
            var button = document.CreateGUIEntity(new GUIEntityInfo(window.EntityPointer, "button_exitapp", new Style()
            {
                background_color = Color.Red,
                color = Color.White,
                Width = new ElementValue(128, Measurement.Pixels),
                Height = new(48, Measurement.Pixels),
            }));

            // Add text to the button
            document.AddText(button, "Exit The Application");


            // TODO Add on hover effects            
            // All animation can be created through the engines animation system since GUIelements are part of the ECS system
            //


            // This gets invoked every time an event occurs
            // Do all logic in here
            document.OnGUIEvent += (@event, entity) =>
            {
                if (@event != EventType.Click)
                    return;
                // GetID is static so id indexes are consistent across documents
                //
                if (entity.Get<Widget>().id != Document.GetID("button_exitapp"))
                    return;

                // perform logic
                Environment.Exit(0);
            };






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