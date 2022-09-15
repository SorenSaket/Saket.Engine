using LiteNetLib.Utils;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TechWars;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using Saket.ECS;
using System.Numerics;
using TechWars.Shared;
using Saket.Engine;
using Saket.Engine.Networking;

namespace TechWars
{
	public class NetServer : INetEventListener
	{
		// Networking
		public Dictionary<int, ServerPlayer> players;

		private NetManager netManager;

		private NetPacketProcessor packetProcessor;
		private NetDataWriter cachedWriter;

		// Logic
		private World world_server;
		private Pipeline pipeline_server;
		private int msPerTick;

		// Misc
		public ushort tick_server;
		protected Stopwatch stopwatch;

		public NetServer(int port, float frequency = 60)
		{
			Debug.WriteLine($"Starting {frequency} fps Server...");
			//
			this.msPerTick = (int)(1000f * (1f / frequency));

			players = new Dictionary<int, ServerPlayer>();

			//
			packetProcessor = new NetPacketProcessor();
			packetProcessor.RegisterNestedType<PlayerState>();
			packetProcessor.RegisterNestedType<PlayerInput>();
			packetProcessor.RegisterNestedType<Packet_S_GameState>();
			packetProcessor.RegisterNestedType<Packet_S_PlayerState>();

			packetProcessor.SubscribeReusable<Packet_P_JoinPacket, NetPeer>(OnJoinReceived);

			cachedWriter = new NetDataWriter();

			netManager = new NetManager(this) { 
				AutoRecycle = true, 
				//SimulatePacketLoss = true, 
				SimulateLatency = true, 
				DisconnectTimeout = int.MaxValue

			};
			netManager.Start(port);

			world_server = new World();
			world_server.SetResource(this);

			pipeline_server = new Pipeline();
			Stage stage = new Stage();
			stage.Add(Server.Systems.ApplyPlayerInput);
			stage.Add(Systems.Player_Move);

			pipeline_server.AddStage(stage);

			this.stopwatch = new Stopwatch();
			Start();
		}

		public void Start()
		{
			Thread t = new Thread(Tick)
			{
				IsBackground = true
			};
			t.Start();
		}
		public void Tick()
		{
			while (netManager.IsRunning)
			{
				stopwatch.Reset();
				stopwatch.Start();
				// Advance the server tick
				tick_server = NetworkCommon.TickAdvance(tick_server);

				// Poll all events
				netManager.PollEvents();

				// Advance server state
				pipeline_server.Update(world_server, 1f / 60f);

				// Construct state packet
				Packet_S_GameState packet_state = new Packet_S_GameState();
				packet_state.tick_server = tick_server;
				packet_state.playerstates = new Packet_S_PlayerState[players.Count];
				for (int i = 0; i < players.Count; i++)
				{
					if (world_server.GetEntity(players[i].entityID).Unwrap(out var entity))
					{
						var player = entity.Get<Player>();
						var transform = entity.Get<Transform2D>();
						//Debug.WriteLine($"{transform.x} + {transform.y}");
						packet_state.playerstates[i] = new Packet_S_PlayerState((ushort)i, 0, new(transform.x, transform.y), player.input);
					}
					else
						throw new Exception("player has no entity");
				}


				// Todo send players who have changed state to other players
				// x = target player
				for (int x = 0; x < players.Count; x++)
				{
					players[x].peer.Send(WriteSerializable(PacketType.State, packet_state), DeliveryMethod.Unreliable);
                    //players[x].peer.Send(WriteSerializable(PacketType.Ack, new Packet_S_ACK (){ tick_ack = players[x].tick_remote }), DeliveryMethod.Unreliable);
                }

				// Sleep the server thread
				stopwatch.Stop();
				Thread.Sleep((int)(MathF.Max(0, msPerTick - stopwatch.ElapsedMilliseconds)));
			}
		}
		public void Stop()
		{
			netManager.Stop();
		}


		private void OnJoinReceived(Packet_P_JoinPacket joinPacket, NetPeer peer)
		{
			Debug.WriteLine("[S] Join packet received: " + joinPacket.UserName);

			// Create the new player
			Entity e = world_server.CreateEntity();
			e.Add(new BundlePlayer(new(), new(2f,2f,0f,0f,0.5f,0.5f)));

			var player = new ServerPlayer(joinPacket.UserName, peer, e.EntityPointer.ID);

			players.Add(peer.Id, player);

			// 
			PlayerState intitialStatePacket = new PlayerState(2f, 2f);


			//Send join accept
			Packet_S_JoinAccept joinAcceptPacket = new Packet_S_JoinAccept { clientID = (byte)peer.Id, ServerTick = tick_server };
			peer.Send(WritePacket(joinAcceptPacket), DeliveryMethod.ReliableOrdered);

			//Send to old players info about new player
			Packet_S_PlayerJoined playerJoinedPacket = new Packet_S_PlayerJoined
			{
				Username = joinPacket.UserName,
				NewPlayer = true,
				InitialPlayerState = intitialStatePacket,
				ServerTick = tick_server,
				clientID = peer.Id
			};
			netManager.SendToAll(WritePacket(playerJoinedPacket), DeliveryMethod.ReliableOrdered);
			
			//Send to new player info about old players
			playerJoinedPacket.NewPlayer = false;
			foreach (var otherPlayer in players)
			{
				if (otherPlayer.Key == peer.Id) // Skip current player
					continue;
				playerJoinedPacket.clientID = otherPlayer.Key;
				playerJoinedPacket.Username = otherPlayer.Value.Name;
				playerJoinedPacket.InitialPlayerState = intitialStatePacket;
				peer.Send(WritePacket(playerJoinedPacket), DeliveryMethod.ReliableOrdered);
			}
		}

		/// <summary>
		/// Invoked when received input from a player
		/// </summary>
		private void OnInputReceived(NetPacketReader reader, NetPeer peer)
		{
			if (peer.Tag == null)
				throw new Exception("tag is null");

			var _cachedCommand = new Packet_P_Input();
			_cachedCommand.Deserialize(reader);

			if (_cachedCommand.inputs == null)
				return;
			var player = (ServerPlayer)peer.Tag;

            player.tick_remote = _cachedCommand.tick_player;
            player.inputs[0] = _cachedCommand.inputs[^1];

            /*
            // Maintain buffer of inputs
            // 0 = oldset
            // inputs.Length -1 = newest
            for (int i = 0; i < _cachedCommand.inputs.Length; i++)
			{
				// If this 
				//if (i > player.inputs.Capacity)
				//	break;
				// The tick the input command was issued
				ushort t = NetworkCommon.TickAdvance(_cachedCommand.tick_server, i);
				// If the tick is too late; discard
				// TODO this might not work
				//if (NetworkCommon.SeqDiff(tick_server, t) < 0)
				//	continue;

				// CHeck if inputs fits in ringbuffer
				// the 0 element tick in the input buffer is always equals the tick_server 
				int diff = NetworkCommon.SeqDiff(tick_server, t);

				if (diff > 0 && diff < player.inputs.Capacity)
					player.inputs[diff] = _cachedCommand.inputs[i];
			}*/
        }

		// Writers
		private NetDataWriter WriteSerializable<T>(PacketType type, T packet) where T : struct, INetSerializable
		{
			cachedWriter.Reset();
			cachedWriter.Put((byte)type);
			packet.Serialize(cachedWriter);
			return cachedWriter;
		}
		private NetDataWriter WritePacket<T>(T packet) where T : class, new()
		{
			cachedWriter.Reset();
			cachedWriter.Put((byte)PacketType.Serialized);
			packetProcessor.Write(cachedWriter, packet);
			return cachedWriter;
		}

		// Network events
		void INetEventListener.OnPeerConnected(NetPeer peer)
		{
			Debug.WriteLine("[S] Player connected: " + peer.EndPoint);
		}
		void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
		{
			Debug.WriteLine("[S] Player disconnected: " + disconnectInfo.Reason);

			// if the peer object exsists 
			// remove check?
			if (peer.Tag != null)
			{
				if (players.Remove(peer.Id)) // if the player ID
				{
					var disconnectPacket = new Player_S_DisconnectPacket { clientID = (byte)peer.Id };
					netManager.SendToAll(WritePacket(disconnectPacket), DeliveryMethod.ReliableOrdered);
				}
			}
		}
		void INetEventListener.OnNetworkError(IPEndPoint endPoint, SocketError socketError)
		{
			Debug.WriteLine("[S] NetworkError: " + socketError);
		}
		void INetEventListener.OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
		{
			// trash
		}
		void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency)
		{
			if (peer.Tag != null)
			{
				var p = (ServerPlayer)peer.Tag;
				p.Ping = latency;
			}
		}
		void INetEventListener.OnConnectionRequest(ConnectionRequest request)
		{
			// Insert authentication here

			request.Accept();
		}

        void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            byte packetType = reader.GetByte();
            PacketType pt = (PacketType)packetType;
            switch (pt)
            {
                case PacketType.Input:
                    OnInputReceived(reader, peer);
                    break;
                case PacketType.Serialized:
                    packetProcessor.ReadAllPackets(reader, peer);
                    break;
                default:
                    Debug.WriteLine("Unhandled packet: " + pt);
                    break;
            }
        }
    }
}