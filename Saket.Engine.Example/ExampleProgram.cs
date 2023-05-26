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
using System.ComponentModel;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        int sdfSize = 64;
        private float fill = 0;
        Texture tex;
        Vector2 offset;
        float scale = 60f / 1468f;
        Shape shape;
        byte[] data;
        Font font;
        public ExampleProgram(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            resources = new ResourceManager();
            resources.databases.Add(new DatabaseEmbedded(Assembly.GetExecutingAssembly()));
            resources.RegisterLoader(new LoaderTexture());
            resources.RegisterLoader(new LoaderShader());
        }

        protected override void OnLoad()
        {
            // Font testing
            var stream = File.Open("C:\\Users\\saket\\data\\projects\\Saket.Engine\\Saket.Engine\\Assets\\Fonts\\OpenSans-Regular.ttf", FileMode.Open);

            font = new Font();
            font.LoadFromOFF(stream);

            




            world = new World();
            var entity_camera = world.CreateEntity();
            entity_camera.Add(new Transform2D());
            entity_camera.Add(new CameraOrthographic(32, 0.1f, 100f));
            controller_ui = new Dear_ImGui_Sample.ImGuiController(1280, 720);
            // Initialize graphics resources
            spriteRenderer = new SpriteRenderer(1000, entity_camera, resources.Load<Shader>("sdf"), (shader) =>
            {
                shader.SetFloat("time", fill);
            });

            data = new byte[sdfSize * sdfSize * 4];

            // ---- Texture Loading
            {
                float ppu = 256f;

                TextureGroups groups = new();
                
                tex = new Texture(data, sdfSize, sdfSize, 
                    TextureMinFilter.Linear, 
                    SizedInternalFormat.R32f, 
                    PixelInternalFormat.R32f, 
                    PixelFormat.Red, 
                    PixelType.Float); 
                tex.LoadToGPU();


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

        void GenerateSDF()
        {
            float size = 64f;
            float half = size / 2f;
            float margin = 6f;

            float radius = 2f;
            float a = 2f;
            /*
            Shape shape = new Shape(new Spline2D[] { 
                new Spline2D(new System.Collections.Generic.List<Vector2>()
                {
                    new Vector2(margin,margin),
                    new Vector2(margin,size/2f),
                    new Vector2(margin,size-margin),
                    new Vector2(size-margin,size-margin),
                    new Vector2(size-margin,margin),
                    new Vector2(margin,margin),
                }),
                new Spline2D(new System.Collections.Generic.List<Vector2>()
                {
                    new Vector2(half,half+radius),
                    new Vector2(half-radius,half+radius),
                    new Vector2(half-radius,half),
                    new Vector2(half-radius,half-radius),
                    new Vector2(half,half-radius),
                    new Vector2(half+radius,half-radius),
                    new Vector2(half+radius,half),
                    new Vector2(half+radius,half+radius),
                })
            });´*/

            /*
            shape = new Shape(new Spline2D[] {
                new Spline2D(new System.Collections.Generic.List<Vector2>()
                {
                    new Vector2(1117,  0),
                    new Vector2(1027, 232),
                    new Vector2(937, 464),
                    new Vector2(644, 464),
                    new Vector2(351, 464),
                    new Vector2(261.5f, 232),
                    new Vector2(172, 0),
                    new Vector2(86, 0),
                    new Vector2(0, 0),
                    new Vector2(286, 734),
                    new Vector2(572, 1468),
                    new Vector2(648.5f, 1468),
                    new Vector2(725, 1468),
                    new Vector2(1009, 734),
                    new Vector2(1293, 0),
                    new Vector2(1205, 0),
                    new Vector2(1117, 0),
                })
            });*/
          /*
            for (int x = 0; x < shape.Count; x++)
            {
                for (int y = 0; y < shape[x].points.Count; y++)
                {
                    shape[x].points[y] *= scale;
                    shape[x].points[y] += offset;
                }
            }*/

             shape = font.glyphs['a'];
            /*
            {
                Vector2 max = new Vector2(float.MinValue, float.MinValue);
                Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
                for (int x = 0; x < shape.Count; x++)
                {
                    for (int y = 0; y < shape[x].points.Count; y++)
                    {
                        if (shape[x].points[y].X > max.X)
                            max.X = shape[x].points[y].X;
                        if (shape[x].points[y].X < min.X)
                            min.X = shape[x].points[y].X;

                        if (shape[x].points[y].Y > max.Y)
                            max.Y = shape[x].points[y].Y;
                        if (shape[x].points[y].Y < min.Y)
                            min.Y = shape[x].points[y].Y;
                    }
                }
                Vector2 scale = new Vector2((sdfSize-8) / Vector2.Distance(min, max));
                Vector2 offset = new Vector2(8, 2);
                for (int x = 0; x < shape.Count; x++)
                {
                    for (int y = 0; y < shape[x].points.Count; y++)
                    {
                        shape[x].points[y] *= scale;

                        shape[x].points[y] += offset;
                    }
                }

            }*/




            float[] color = new float[sdfSize * sdfSize];
            SDFGenerator gen = new SDFGenerator();
            gen.GenerateSDF(
                new SDFGenerator.Settings() { type = SDFGenerator.SDFType.SDF, inverseY = false, inverseX = false },
                color,
                sdfSize,
                shape,
                1f,
                new Vector2(scale, scale),
                offset*10
            );

            {
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
                    color[i] = Mathf.Remap(color[i], -r, r, 0f, 1f);
                }
            }
            data = color.SelectMany(x => BitConverter.GetBytes(x)).ToArray();
            tex.Replace(data);
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
                ImGui.SliderFloat("Fill Percentage", ref fill, -1, 1);
                //ImGui.GetWindowDrawList().AddImage()
                if (ImGui.SliderFloat2("Offset",ref offset, -sdfSize, sdfSize)){
                    GenerateSDF();
                }
                if (ImGui.SliderFloat("Scale", ref scale, 0, 1))
                {
                    GenerateSDF();
                }
                if (shape != null)
                {
                    if (ImGui.BeginTable("table2", 2))
                    {
                        for (int i = 0; i < shape.Count; i++)
                        {
                            for (int row = 0; row < shape[i].points.Count; row++)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.Text($"{row}");
                                ImGui.TableNextColumn();
                                ImGui.Text(shape[i].points[row].ToString());
                            }
                        }
                      
                        ImGui.EndTable();
                    }
                }
           


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