using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using Saket.ECS;
using System.Diagnostics;
using System.Drawing;
using Saket.Engine;
using System;
using System.Numerics;
using Saket.UI;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;

namespace Saket.Engine.Example
{
    struct Velocity
    {
        public float x;
        public float y;

        public Velocity(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator Velocity(Vector2 v) => new Velocity(v.X,v.Y);

    }

    struct Player
    {
        public Vector2 movement;
    }


    class ExampleProgram : Application
    {
        float aspectRatio;
        
        World world;

        Document document;

        SpriteRenderer spriteRenderer;

        Pipeline pipeline_update = new Pipeline();
        Pipeline pipeline_render = new Pipeline();

        public ExampleProgram(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            // Initialize graphics resources
           // spriteRenderer = new SpriteRenderer(1000);
            world = new World();
            document = new UI.Document();

            Stage stage_update = new Stage();
            stage_update.Add(PlayerSystem);
            stage_update.Add(MoveSystem);

            pipeline_update.AddStage(stage_update);

            Stage stage_render = new Stage();
            stage_render.Add(spriteRenderer.SystemSpriteRenderer);
            pipeline_render.AddStage(stage_render);
            
            //
            //e
           /* { 
                // Define styles
                StyleSheet StyleSheet = new StyleSheet();
            
                StyleSheet.Add(
                    new (
                        new ("menu"), // Selector
                        new() // styles
                        {
                            Width = "100%",
                            Height = "32px"
                        } 
                    )
                );

                Entity e = document.CreateWidget();
                e.Add()
            }*/
            

            Random rnd = new Random();

            Transform2D spawnPoint = new Transform2D(0, 0, 0, 0, 0.2f, 0.2f);

            for (int i = 0; i < 1; i++)
            {
                Entity e = world.CreateEntity();
                e.Add(spawnPoint);
                e.Add(new Velocity(1f - (float)rnd.NextDouble()*2f, 1f - (float)rnd.NextDouble() * 2f));
                e.Add(new Sprite(1, 0, (uint)(rnd.NextDouble()*uint.MaxValue))); //0b10000000_10000000_10000000_10000000
                e.Add(new Player());
            }
        }

        Query query = new Query().With<Transform2D>().With<Velocity>();


        Query query_player = new Query().With<Velocity>().With<Player>();

        protected void MoveSystem(World world)
        {
            var result = world.Query(query);

            foreach (var item in result)
            {
                var transform   = item.Get<Transform2D>();
                var velocity    = item.Get<Velocity>();

                transform.x += velocity.x * world.Delta;
                transform.y += velocity.y * world.Delta;

                item.Set(transform);
            }
        }

        protected void PlayerSystem(World world)
        {
            var result = world.Query(query_player);

            foreach (var item in result)
            {
                var player = item.Get<Player>();

                Vector2 movement = new Vector2();

                if (KeyboardState.IsKeyDown(Keys.D))
                    movement.X += 1;
                if (KeyboardState.IsKeyDown(Keys.A))
                    movement.X -= 1;
                if (KeyboardState.IsKeyDown(Keys.W))
                    movement.Y += 1;
                if (KeyboardState.IsKeyDown(Keys.S))
                    movement.Y -= 1;
                
                if (movement.LengthSquared() != 0)
                    player.movement = Vector2.Normalize(movement);
                else
                    player.movement = movement;


                Velocity velocity = player.movement;

                item.Set(player);
                item.Set(velocity);
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            float delta = (float)args.Time;

            pipeline_update.Update(world, delta);
        }


        protected override void OnRenderFrame(FrameEventArgs args)
        {
            float delta = (float)args.Time;
            GL.ClearColor(0f, 0, 0, 1f);
            GL.ClearStencil(0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.StencilBufferBit | ClearBufferMask.DepthBufferBit);
            spriteRenderer.shader.Use();
            spriteRenderer.shader.SetMatrix4("view", Matrix4x4.Identity);
            spriteRenderer.shader.SetMatrix4("projection", Matrix4x4.CreateOrthographic(10, 10 / aspectRatio, 0.1f, 100f));
            pipeline_render.Update(world, delta);

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            aspectRatio = (float)e.Width / (float)e.Height;
            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }
}