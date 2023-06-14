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
using System.IO;
using Saket.Engine.Graphics;
using Saket.Engine.Graphics.Packing;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Saket.Engine.Example
{

    class ExampleProgram : Application
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
                float ppu = 64;
                int tilePadding = 1;
                var settings_packer = new Packer.SettingsPacker()
                {
                    padding =0,
                    margin = 0,
                };

                int size = 512;
                char from = 'A';
                char to = 'z';

                // Atlas creation
                float[] data = new float[size * size];
                Span<float> dataAsSpan = new Span<float>(data);

                // Create the main sdf game texture with float values
                Texture texture = new Texture(size, size,
                       TextureMinFilter.Linear,
                       SizedInternalFormat.R32f,
                       PixelInternalFormat.R32f,
                       PixelFormat.Red,
                       PixelType.Float);

                TextureAtlas atlas = new TextureAtlas(texture, 256);


                // Font loading
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
                    // For each character to add to atlas
                    for (int i = from; i < to; i++)
                    {
                        Glyph g = font.glyphs[(char)i];
                        // A lot of rounding is happening here
                        // TODO: Look into if hinting would produce better quality SDFs
                        atlas.tiles.Add(
                            new Tile(
                                ((g.width * ppu)),
                                ((g.height * ppu) )

                            ));
                    }
                }
                
                // packing
                {
                    Packer packer = new Packer();

                    bool packed = packer.TryPack(CollectionsMarshal.AsSpan(atlas.tiles), size,size, settings_packer);

                    if (!packed)
                        throw new Exception("Not enough space on atlas!!");
                }
             
                // Generate sdf
                {
                    int index_atlas = 0;
                    SDFGenerator generator = new SDFGenerator();


                    float increment = 1f / ((int)to - from);
                    float v = increment;

                    for (int i = from; i < to; i++)
                    {/*
                        {
                            for (int y = 0; y < atlas[index_atlas].Size.Y; y++)
                            {
                                for (int x = 0; x < atlas[index_atlas].Size.X; x++)
                                {
                                    dataAsSpan[atlas[index_atlas].Position.X + x + (atlas[index_atlas].Position.Y + y) * size] = v;
                                }
                            }
                        }*/
                        Glyph g = font.glyphs[(char)i];

                        generator.GenerateSDF(g.Shape, dataAsSpan, size, (atlas.tiles[index_atlas].Position).RoundToInt2(), (atlas.tiles[index_atlas].Size ).RoundToInt2(), 1, new Vector2(ppu), Vector2.Zero);


                        v += increment;
                        index_atlas++;
                    }
                }

                // Upload
                {
                    var a = GL.GetError();
                    
                    unsafe
                    {
                        fixed(void* ptr = dataAsSpan)
                        {

                            texture.Create();
                            texture.Upload((nint)ptr);
                        }
                    }

                    // ---- Texture Loading
                    {
                        List<TextureAtlas> groups = new()
                        {
                            new (texture, 1,1)
                        };

                        world.SetResource(groups);
                    }
                }
            }



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
                /*if (ImGui.SliderFloat2("Offset",ref offset, -sdfSize, sdfSize)){
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