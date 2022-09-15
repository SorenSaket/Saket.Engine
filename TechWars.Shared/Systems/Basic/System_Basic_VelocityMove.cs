using Saket.ECS;
using Saket.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TechWars.Shared
{
    public partial class Systems
    {
        private static Query query_transform2dvelocity = new Query().With<Transform2D>().With<Velocity>();

        public static void System_Basic_VelocityMove(World world)
        {
            var result = world.Query(query_transform2dvelocity);

            foreach (var item in result)
            {
                var transform = item.Get<Transform2D>();
                var velocity = item.Get<Velocity>();

                transform.x += velocity.x * world.Delta;
                transform.y += velocity.y * world.Delta;

                item.Set(transform);
            }
        }
    }
}
