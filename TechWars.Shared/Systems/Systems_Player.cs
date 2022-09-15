
using Saket.ECS;
using Saket.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace TechWars.Shared
{
    public partial class Systems
    {
        public static Query query_playerTransform = new Query().With<Player>().With<Transform2D>();
        public static void Player_Move(World world)
        {
            var entities = world.Query(query_playerTransform);

            foreach (var entity in entities)
            {
                Player player           = entity.Get<Player>();
				ApplyInputToPlayerEntity(entity, player.input, world.Delta);
			}
        }


		public static void ApplyInputToPlayerEntity(Entity entity, PlayerInput input, float delta)
		{
			Transform2D transform = entity.Get<Transform2D>();

			Vector2 direction = new Vector2(((float)input.axis_x), ((float)input.axis_y)) / short.MaxValue;
			if (direction.LengthSquared() > MathF.Sqrt(2))
				direction = Vector2.Normalize(direction);

			transform.x += (direction.X) * delta * 5f;
			transform.y += (direction.Y) * delta * 5f;

			entity.Set(transform);

		}
    }
}
