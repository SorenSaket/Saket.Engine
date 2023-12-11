using HackAttack.Components;
using Saket.ECS;
using Saket.Engine;
using Saket.Engine.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackAttack;

internal static partial class Systems
{
    static Query query_enemy = new Query().With<Enemy>();

    public static void Enemy(World world)
    {
        QueryResult entities = world.Query(query_enemy);
        foreach (var entity in entities)
        {
            Transform2D transform = entity.Get<Transform2D>();
            Velocity velocity = entity.Get<Velocity>();

            Sprite sprite = entity.Get<Sprite>();


            transform.rx = MathF.Atan2(velocity.Y, velocity.X) + MathF.Sin(world.Time * velocity.Value.Length() * 10) * 0.1f;
            sprite.spr = 16 + (int)((world.Time * velocity.Value.Length() * 5) % 4);


            entity.Set(transform);
            entity.Set(sprite);
        }
    }

    public static void Enemy_Navigation(World world)
    {
        if(!world.TryGetResource(out GameState gameState))
            return;
        


        QueryResult entities = world.Query(query_enemy);
        foreach (var entity in entities)
        {
            Transform2D transform = entity.Get<Transform2D>();
            Velocity velocity = entity.Get<Velocity>();

            int x = (int)MathF.Round( transform.Position.X);
            int y = (int)MathF.Round(transform.Position.Y);

            if((x >= 0 && x <gameState.mapData.Width ) && 
                (y >= 0 && y < gameState.mapData.Height))
                velocity.Value = gameState.field[x, y];

            entity.Set(velocity);
        }
    }

}
