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
using Saket.Engine.Graphics;
using Saket.Engine.Graphics.Packing;
using System.Runtime.InteropServices;

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
            world = new World();


            // Font testing
            {

                if (resources.TryGetStream("OpenSans-Regular.ttf", out var stream))
                {
                    font = new Font();
                    font.LoadFromOFF(stream);
                }
                else
                {
                    throw new Exception("Font not found");
                }

                float ppu = 64;
                int size = 512;

                Memory<float> data = new float[size * size];


                // Create the main sdf game texture with float values
                Texture texture = new Texture(size, size,
                       TextureMinFilter.Linear,
                       SizedInternalFormat.R32f,
                       PixelInternalFormat.R32f,
                       PixelFormat.Red,
                       PixelType.Float);

                Atlas atlas = new Atlas(texture, 256);

                // For each character to add to atlas
                for (int i = 'A'; i < 'z'; i++)
                {
                    Glyph g = font.glyphs[(char)i];
                    // A lot of rounding is happening here
                    // TODO: Look into if hinting would produce better quality SDFs
                    atlas.Add(new Tile((int)(g.width * ppu), (int)(g.height * ppu)));
                }

                Packer packer = new Packer();

                bool packed = packer.TryPack(CollectionsMarshal.AsSpan(atlas), atlas.texture.width, atlas.texture.height);

                if (!packed)
                    throw new Exception("Not enough space on atlas!!");

                SDFGenerator generator = new SDFGenerator();

                Span<float> dataAsSpan = data.Span;

                int index_atlas = 0;
                for (int i = 'A'; i < 'z'; i++)
                {
                    Glyph g = font.glyphs[(char)i];

                    generator.GenerateSDF(g, dataAsSpan, size, atlas[index_atlas].Position, atlas[index_atlas].Size, 1, new Vector2(ppu), Vector2.Zero);
                    index_atlas++;
                }

                var handle = data.Pin();
                unsafe
                {
                    texture.Upload((nint)handle.Pointer);
                }
                handle.Dispose();
                // ---- Texture Loading
                {
                    TextureGroups groups = new();

                    var sheet = new Sheet(1, 1, 1);
                    groups.Add(texture, sheet);

                    world.SetResource(groups);
                }


            }



            var entity_camera = world.CreateEntity();
            entity_camera.Add(new Transform2D());
            entity_camera.Add(new CameraOrthographic(32, 0.1f, 100f));
            controller_ui = new Dear_ImGui_Sample.ImGuiController(1280, 720);
            // Initialize graphics resources
            spriteRenderer = new SpriteRenderer(1000, entity_camera, resources.Load<Shader>("sdf"), (shader) =>
            {
                shader.SetFloat("time", 0);
            });



            var teste = world.CreateEntity();
            teste.Add(new Sprite(0, 0, int.MaxValue));
            teste.Add(new Transform2D(0f, 0f, 0, 0, 1, 1));

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
                }/*
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
                }*/
           


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