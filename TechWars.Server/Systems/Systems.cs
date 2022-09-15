using Saket.ECS;
using Saket.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechWars.Shared;

namespace TechWars.Server
{
	public static partial class Systems
	{
		public static Query query_player = new Query().With<Player>();
		public static void ApplyPlayerInput(World world)
		{
			var entities = world.Query(query_player);

			var server = world.GetResource<NetServer>();

			foreach (var entity in entities)
			{
				var player = server.players.FirstOrDefault(x => x.Value.entityID == entity.ID).Value;

				if(player == null)
					throw new Exception("client entity does not exsist");

				// Get the input for current tick
				if (player.inputs[0].Unwrap(out var input))
				{
					Player p = entity.Get<Player>();
					p.input = input;
					entity.Set(p);
					player.packetloss_avg -= 0.01f;
					//player.packetloss_avg.Clamp(0, float.MaxValue);
				}
				else
				{
					// If no input is present expand the player input buffer
					// quickly make buffer smaller when possible
					// but do n
					player.packetloss_avg += 0.1f;
				}
				// advance buffer
				player.inputs.RemoveFromStart(1);
			}
		}
	}
}
