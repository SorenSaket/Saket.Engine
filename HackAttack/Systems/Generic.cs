using HackAttack.Components;
using Saket.ECS;
using Saket.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HackAttack;

internal static partial class Systems
{

    static Query query_VelocityTransform = new Query().With<(Velocity, Transform2D)>();

    public static void Move(World world)
    {
        QueryResult entities = world.Query(query_VelocityTransform);
        foreach (var entity in entities)
        {
            Transform2D transform =  entity.Get<Transform2D>();
            Velocity velocity =  entity.Get<Velocity>();

            if(velocity.Value.Length() > 0f)
            {

            }
            transform.Position += world.Delta * velocity.Value;

            entity.Set(transform);
        }
    }



    static Query query_rotateTransform = new Query().With<(RotateTowardsTarget, Transform2D)>();

    public static void RotateTowardsTarget(World world)
    {
        QueryResult entities = world.Query(query_rotateTransform);
        foreach (var entity in entities)
        {
            RotateTowardsTarget rtt = entity.Get<RotateTowardsTarget>();
            if (rtt.rotationSpeed == 0)
                continue;

            if (world.TryGetEntity(rtt.target, out var entity_target))
            {
                Transform2D transform = entity.Get<Transform2D>();
                Transform2D transformTarget = entity_target.Get<Transform2D>();

                // Get the target angle
                var diff = transformTarget.Position - transform.Position;
                float targetAngle = MathF.Atan2(diff.Y, diff.X);

                transform.rx = Mathf.LerpAngle(transform.rx, targetAngle, world.Delta * rtt.rotationSpeed);

                entity.Set(transform);
            }
        }


    }


    static Query query_moveTowardsTargetTransform = new Query().With<(MoveTowardsTarget, Transform2D)>();
    public static void MoveTowardsTarget(World world)
    {
        QueryResult entities = world.Query(query_moveTowardsTargetTransform);
        foreach (var entity in entities)
        {
            MoveTowardsTarget mtt = entity.Get<MoveTowardsTarget>();
            if (mtt.moveSpeed == 0)
                continue;

            if (world.TryGetEntity(mtt.target, out var entity_target))
            {
                Transform2D transform = entity.Get<Transform2D>();
                Transform2D transformTarget = entity_target.Get<Transform2D>();

                // Get the dir
                //var diff = transformTarget.Position - transform.Position;
                //diff = Vector2.Normalize(diff);
                transform.Position = Vector2.Lerp(transform.Position, transformTarget.Position, world.Delta * mtt.moveSpeed);

                entity.Set(transform);
            }
        }


    }
}
