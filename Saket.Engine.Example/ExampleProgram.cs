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
        
        //Dear_ImGui_Sample.ImGuiController controller_ui;

        ResourceManager resources;

        public ExampleProgram(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            resources = new ResourceManager();
            resources.databases.Add(new DatabaseEmbedded(Assembly.GetExecutingAssembly()));
            resources.RegisterLoader(new LoaderTexture());
            resources.RegisterLoader(new LoaderShader());

           

        }

        protected override void OnLoad()
        {
            world = new World();
            var entity_camera = world.CreateEntity();
            entity_camera.Add(new Transform2D());
            entity_camera.Add(new CameraOrthographic(32, 0.1f, 100f));

            // Initialize graphics resources
            spriteRenderer = new SpriteRenderer(1000, entity_camera, resources.Load<Shader>("msdf"));
    
            
            // ---- Texture Loading
            {
                float ppu = 256f;

                TextureGroups groups = new();

                var tex = resources.Load<Texture>("msdffont");
                tex.LoadToGPU();
                tex.UnloadFromCPU();
                var sheet = new Sheet(1, 1, 1);
                groups.Add(tex, sheet);

                world.SetResource(groups);
            }

            var teste = world.CreateEntity();
            teste.Add(new Sprite(0, 0, int.MaxValue));
            teste.Add(new Transform2D(0f, 0f,0,0,100,100));




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
            /*controller_ui.Update(this, delta);
            ImGui
            {
                

                ImGui.Begin("Window", ImGuiWindowFlags.MenuBar);
                if (ImGui.BeginMenuBar())
                {
                    if (ImGui.BeginMenu("File"))
                    {
                        if (ImGui.MenuItem("Open..", "Ctrl+O")) { }
                        if (ImGui.MenuItem("Save", "Ctrl+S")) {}
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
            */
            pipeline_update.Update(world, delta);
        }


        protected override void OnRenderFrame(FrameEventArgs args)
        {
            float delta = (float)args.Time;
            GL.ClearColor(0f, 0, 0, 1f);
            GL.ClearStencil(0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.StencilBufferBit | ClearBufferMask.DepthBufferBit);

            pipeline_render.Update(world, delta);
            //controller_ui.Render();
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            aspectRatio = (float)e.Width / (float)e.Height;
            GL.Viewport(0, 0, e.Width, e.Height);
            //controller_ui.WindowResized(e.Width, e.Height);
        }
    }
}