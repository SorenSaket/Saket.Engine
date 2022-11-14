using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using Saket.ECS;
using System.Diagnostics;
using Saket.Engine;
using System;
using System.Numerics;
using Saket.UI;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using ImGuiNET;
using Saket.Engine.Resources.Databases;
using System.Reflection;
using System.Resources;
using Saket.Engine.Resources.Loaders;
using Saket.Engine.Typography;

namespace Saket.Engine.Example
{

    class ExampleProgram : Application
    {
        float aspectRatio;
        
        World world;

        Document document;

        SpriteRenderer spriteRenderer;

        Pipeline pipeline_update = new Pipeline();
        Pipeline pipeline_render = new Pipeline();

        Dear_ImGui_Sample.ImGuiController controller_ui;

        ResourceManager rm;
        public ExampleProgram(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            rm = new ResourceManager();
            rm.databases.Add(new DatabaseEmbedded(Assembly.GetExecutingAssembly()));
            rm.RegisterLoader(new LoaderFont());

            var font = rm.Load<Font>("WorkSans.ttf");
        }

        protected override void OnLoad()
        {
            // Initialize graphics resources
            //spriteRenderer = new SpriteRenderer(1000, );
            world = new World();
            document = new UI.Document();
            controller_ui = new Dear_ImGui_Sample.ImGuiController(1920, 1080);
            //Stage stage_update = new Stage();

            //pipeline_update.AddStage(stage_update);

            //Stage stage_render = new Stage();
            //stage_render.Add(spriteRenderer.SystemSpriteRenderer);
            //pipeline_render.AddStage(stage_render);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            float delta = (float)args.Time;
            controller_ui.Update(this, delta);
            // ImGui
            {
                

                ImGui.Begin("Window", ImGuiWindowFlags.MenuBar);
                if (ImGui.BeginMenuBar())
                {
                    if (ImGui.BeginMenu("File"))
                    {
                        if (ImGui.MenuItem("Open..", "Ctrl+O")) { /* Do stuff */ }
                        if (ImGui.MenuItem("Save", "Ctrl+S")) { /* Do stuff */ }
                        if (ImGui.MenuItem("Close", "Ctrl+W")) { }
                        ImGui.EndMenu();
                    }
                    ImGui.EndMenuBar();
                }
                ImGui.Text("qweqwe");

                if (ImGui.Button("Cylinder"))
                {

                }
                //ImGui.GetWindowDrawList().AddImage()

                ImGui.End();

                ImGui.Begin("asdd", ImGuiWindowFlags.MenuBar);
                ImGui.Text("ok i pull up");
                ImGui.End();
            }

            //pipeline_update.Update(world, delta);
        }


        protected override void OnRenderFrame(FrameEventArgs args)
        {
            float delta = (float)args.Time;
            GL.ClearColor(0f, 0, 0, 1f);
            GL.ClearStencil(0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.StencilBufferBit | ClearBufferMask.DepthBufferBit);

            //pipeline_render.Update(world, delta);
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