using HackAttack.Components;
using Saket.ECS;
using Saket.Engine;
using Saket.Engine.GUI;
using Saket.Engine.Input;
using SDL2;
using System.Numerics;


namespace HackAttack;

internal static partial class Systems
{
    static Query query_playerTransform = new Query().With<(Player, Transform2D)>();
    
    /// <summary>
    /// Gets input from Keyboardstate and sets
    /// </summary>
    /// <param name="world"></param>
    public static void ClientInput(World world)
    {
        if (!world.TryGetResource<KeyboardState>(out var keyboardState))
            throw new Exception("");

        if (!world.TryGetResource<MouseState>(out var mouseState))
            throw new Exception("");
        
        if (!world.TryGetResource<WindowInfo>(out var windowInfo))
            throw new Exception("");


        var entities = world.Query(query_playerTransform);
        foreach (var entity_player in entities)
        {
             var player_local =  entity_player.Get<Player>();
             var transform = entity_player.Get<Transform2D>();

            //Move local player
            Vector2 movement = new Vector2();
            {
                if (keyboardState.IsKeyDown((int)SDL.SDL_Scancode.SDL_SCANCODE_D))
                    movement.X += 1;
                if (keyboardState.IsKeyDown((int)SDL.SDL_Scancode.SDL_SCANCODE_A))
                    movement.X -= 1;
                if (keyboardState.IsKeyDown((int)SDL.SDL_Scancode.SDL_SCANCODE_W))
                    movement.Y += 1;
                if (keyboardState.IsKeyDown((int)SDL.SDL_Scancode.SDL_SCANCODE_S))
                    movement.Y -= 1;

                if (movement.LengthSquared() != 0)
                {
                    movement = Vector2.Normalize(movement);
                }
            }

            float rotation = transform.rx;
            Vector2 mousePosWorld = Vector2.Zero;

            {
                // Todo implement rotation
                // 1 : convert camera to resource? This is because componetized cameras are an advanced features that only is useful if multiple cameras are present.
                // Get camera
                if (world.TryGetResource<Camera>(out var res_cam))
                {
                    if(world.TryGetEntity(res_cam.camera, out var entity_camera))
                    {
                        var cam = entity_camera.Get<CameraOrthographic>();
                        var transform_cam = entity_camera.Get<Transform2D>();
                        // Get mouse position

                        // convert player position to screen position or mouse position to world position
                        Matrix4x4.Invert(cam.viewMatrix*cam.projectionMatrix , out var inv);

                        mousePosWorld = Vector2.Transform(
                            new Vector2(((mouseState.Position.X) / windowInfo.width) * 2f - 1f,
                                        ((windowInfo.height - mouseState.Position.Y) / windowInfo.height) * 2f - 1f
                                        ), inv);
                        // get vector between mouse and player
                        Vector2 rel = mousePosWorld - transform.Position;

                        // rotate acordingly to vector
                        rotation = MathF.Atan2(rel.Y, rel.X);
                    }

                   
                }
            }


            bool shooting = mouseState.IsButtonDown(MouseButton.Left);

            ClientInput input = new ClientInput()
            {
                targetX = (int)(mousePosWorld.X + 0.5f),
                targetY = (int)(mousePosWorld.Y + 0.5f),
                axis_x = movement.X,
                axis_y = movement.Y,
                rotation = 0,
                shooting = shooting
            };

            player_local.input = input;
            entity_player.Set(player_local);
        }


    }
    private static Query query_colliderTransform = new Query().With<(Collider2DBox, Transform2D)>();
    public static void PlayerMove(World world)
    {
        var entities = world.Query(query_playerTransform);
        foreach (var entity_player in entities)
        {
            var player = entity_player.Get<Player>();
            var velocity =  entity_player.Get<Velocity>();
            var transform_player = entity_player.Get<Transform2D>();
            var collider_player = entity_player.Get<Collider2DBox>();


            velocity.X = player.input.axis_x;
            velocity.Y = player.input.axis_y;

            if (velocity.Value.Length() > 0.1f)
                transform_player.rx = MathF.Atan2(velocity.Y, velocity.X);

            velocity.Value *= 3;

            // Query colliders in the world
            var colliders = world.Query(query_colliderTransform);
            foreach (var item in colliders)
            {
                if (item.EntityPointer == entity_player.EntityPointer)
                    continue;

                var collider = item.Get<Collider2DBox>();

                if (collider.IsTrigger)
                    continue;

                var transform_collider = item.Get<Transform2D>();

                if (Collider2DBox.IntersectsWith(collider_player, transform_player, collider, transform_collider))
                {
                    if (collider.Size.X == collider.Size.Y)
                    {
                        Vector2 relativePosition = (transform_collider.Position - player.lastPosition);
                        velocity.Value -= (Vector2.Normalize(relativePosition) * 1f);
                    }
                    else
                    {
                        Vector2 relativePosition = (transform_collider.Position - player.lastPosition) / (collider.Size / 2f);
                        Vector2 normal = relativePosition.RectNormal();
                        velocity.Value -= normal * 1f;
                    }

                }
            }

            player.lastPosition = transform_player.Position;

            entity_player.Set(player);
            entity_player.Set(velocity);
            entity_player.Set(transform_player);
        }
    }

    public static void PlayerBuild(World world)
    {
        var entities = world.Query(query_playerTransform);
        foreach (var entity_player in entities)
        {
            var player = entity_player.Get<Player>();

            if (world.TryGetEntity(player.buildobj, out var entity_bobj))
            {
                var transform = entity_bobj.Get<Transform2D>();
                transform.Position.X = player.input.targetX;
                transform.Position.Y = player.input.targetY;
                entity_bobj.Set(transform);
            }

            if (player.input.shooting)
            {
                if (world.TryGetResource(out GameState gameState))
                {
                    if ((player.input.targetX >= 0 && player.input.targetX < gameState.mapData.Width) &&
                        (player.input.targetY >= 0 && player.input.targetY < gameState.mapData.Height))
                    {
                        gameState.mapData.Flags[player.input.targetX, player.input.targetY] |= Saket.Navigation.MapFlags.Blocking;
                        gameState.SetDirty();
                    }
                        
                }
                world.CreateEntity()
                    .Add(new Transform2D(player.input.targetX, player.input.targetY, -1))
                    .Add(new Sprite() { color = Saket.Engine.Graphics.Color.White, spr = 53 })
                    .Add(new Collider2DBox(Vector2.One))
                ;
            }
        }
    }



    public static void PlayerAnimation(World world)
    {
        var entities = world.Query(query_playerTransform);
        foreach (var entity_player in entities)
        {
            var sprite = entity_player.Get<Sprite>();
            var velocity = entity_player.Get<Velocity>();

            sprite.spr = (int)(((world.Time )*velocity.Value.Length() * 5) % 7);

            entity_player.Set(sprite);
        }
    }
}