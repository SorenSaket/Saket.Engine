using OpenTK.Compute.OpenCL;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Saket.ECS;
using Saket.Engine;
using Saket.Engine.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TechWars.Shared;

namespace TechWars.Client
{
	static public partial class Systems
    {


        /// <summary>
        /// 
        /// Requires: 
        /// Resource:<see cref="NetClient"/>, 
        /// Resource:<see cref="KeyboardState"/> 
        /// Resource:<see cref="MouseState"/> 
        /// </summary>
        public static void ClientInput(World world)
        {
            var client = world.GetResource<NetClient>();
            if (client == null)
                return;
			
            var keyboardState = world.GetResource<KeyboardState>();
            if (keyboardState == null)
                return;

            var mouseState = world.GetResource<MouseState>();
            if (mouseState == null)
                return;

            if (world.GetEntity(client.localPlayerEntityID).Unwrap(out var entity_player))
            {
                var player_local = entity_player.Get<Player>();
                var transform = entity_player.Get<Transform2D>();

                //Move local player
                Vector2 movement = new Vector2();
                {
                    if (keyboardState.IsKeyDown(Keys.D))
                        movement.X += 1;
                    if (keyboardState.IsKeyDown(Keys.A))
                        movement.X -= 1;
                    if (keyboardState.IsKeyDown(Keys.W))
                        movement.Y += 1;
                    if (keyboardState.IsKeyDown(Keys.S))
                        movement.Y -= 1;

                    if (movement.LengthSquared() != 0)
                    {
                        movement = Vector2.Normalize(movement);
                    }
                }

                float rotation = transform.rx;
                {
                    // Todo implement rotation
					// 1 : convert camera to resource? This is because componetized cameras are an advanced features that only is useful if multiple cameras are present.
					// Get camera
					// Get mouse position
					// convert player position to screen position or mouse position to world position
					// get vector between mouse and player
					// rotate acordingly to vector
				}

                PlayerInput input = new PlayerInput(movement.X, movement.Y, 0, false);

                // If the input is new
                if (/*player_local.input != input*/ true)
                {
                    // Set local representation input
                    player_local.input = input;
                    entity_player.Set<Player>(player_local);

					client.buffer_input.Enqueue(input);

					client.SendPacketSerializable<Packet_P_Input>(
						PacketType.Input, 
						new Packet_P_Input(
							client.tick_remote, 
							client.tick_local, 
							client.buffer_input.ToArray()
						), 
						LiteNetLib.DeliveryMethod.Unreliable
					);
                }
                
            }
        }


        public static void ClientStateBuffer(World world)
        {
            var client = world.GetResource<NetClient>();
            if (client == null)
                return;


            if (world.GetEntity(client.localPlayerEntityID).Unwrap(out var entity_player))
            {
                client.buffer_state.Enqueue(Utils.GetPlayerState(entity_player));
            }
        }

    }
}
