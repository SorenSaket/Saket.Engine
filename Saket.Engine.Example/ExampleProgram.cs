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
using Saket.UI;
using Saket.ECS;
using Saket.Engine.Graphics.SDF;
using Saket.Graphics;
using Saket.Engine.Math.Geometry;
using System.Collections;
using System.Linq;
using ImGuiNET;

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
            controller_ui = new Dear_ImGui_Sample.ImGuiController(1280, 720);
            // Initialize graphics resources
            spriteRenderer = new SpriteRenderer(1000, entity_camera, resources.Load<Shader>("sdf"));


            float margin = 6f;
            Shape shape = new Shape(new Spline2D[] { new Spline2D(new System.Collections.Generic.List<Vector2>()
            {
                new Vector2(margin,margin),
                new Vector2(margin,32f-margin),
                new Vector2(32f-margin,32f-margin),
                new Vector2(32f-margin,margin),
                new Vector2(margin,margin),
            })});
            

            float[] color = new float[32*32];
            SDFGenerator gen = new SDFGenerator();
            gen.GenerateSDF(
                new SDFGenerator.Settings() { type = SDFGenerator.SDFType.SDF, inverseY = false, inverseX = false },
                color,
                32,
                shape,
                0.5f,
                Vector2.One,
                Vector2.Zero
            );
            // remap values from 0..255
            float min = float.PositiveInfinity;
            float max = float.NegativeInfinity;
            for (int i = 0; i < color.Length; i++)
            {
                if (color[i] < min)
                    min = color[i];
                if (color[i] > max) 
                    max = color[i];
            }

            float r = MathF.Max(MathF.Abs(max), MathF.Abs(min));

            for (int i = 0; i < color.Length; i++)
            {
                color[i] = Mathf.Remap(color[i], min, max, 0, 255);
            }

            byte[] bytes = color.SelectMany(x => new byte[4] { (byte)x, (byte)x, (byte)x, (byte)x }).ToArray();

            // ---- Texture Loading
            {
                float ppu = 256f;

                TextureGroups groups = new();
                
                Texture tex = new Texture(bytes, 32, 32); // resources.Load<Texture>("sdf_circle"); // 
                tex.LoadToGPU();
                tex.UnloadFromCPU();


                var sheet = new Sheet(1, 1, 1);
                groups.Add(tex, sheet);

                world.SetResource(groups);
            }


            var teste = world.CreateEntity();
            teste.Add(new Sprite(0, 0, int.MaxValue));
            teste.Add(new Transform2D(0f, 0f, 0, 0, 10, 10));




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