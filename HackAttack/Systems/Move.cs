using HackAttack.Components;
using Saket.ECS;
using Saket.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackAttack
{
    internal static partial class Systems
    {
        static Query query = new Query().With<(PathFindingAgent, Transform2D)>();
        public static void Move(World world)
        {
            QueryResult a = world.Query(query);

            foreach (var entity in a)
            {
                Transform2D transform =  entity.Get<Transform2D>();

                transform.Position.X += world.Delta * 1000000f * MathF.Sin(world.Time);

                entity.Set(transform);
            }
        }
    }
}
