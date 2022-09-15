using LiteNetLib;
using LiteNetLib.Utils;
using OpenTK.Compute.OpenCL;
using Saket.ECS;
using Saket.Engine;
using Saket.Engine.Networking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TechWars.Shared;

namespace TechWars.Client
{
	public class NetClient : INetEventListener
	{
		/// <summary> </summary>
		public Dictionary<int, ClientPlayer> players;

		public ClientPlayer localPlayer => players[localPlayerID];


		public int localPlayerEntityID = -1;
		public ushort localPlayerID;


		public Queue<PlayerInput> buffer_input;
        public Queue<PlayerState> buffer_state;

        private NetDataWriter writer;
		private NetManager netManager;
		private NetPacketProcessor packetProcessor;

		private NetPeer server;

		public ushort tick_remote;
		public ushort tick_local;

		private string username;

		private World clientWorld;

		protected Stopwatch stopwatch;
		private int msPerTick;

		private int halfRTT;
		private Pipeline pipeline_fixed;

		public NetClient(World clientWorld, string address = "localhost", int port = 9050, float frequency = 60)
		{
			players = new();
			this.clientWorld = clientWorld;

			this.msPerTick = (int)(1000f * (1f / frequency));

			buffer_input = new();
            buffer_state = new();

			packetProcessor = new NetPacketProcessor();
			packetProcessor.RegisterNestedType<PlayerState>();
			packetProcessor.RegisterNestedType<PlayerInput>();
			packetProcessor.RegisterNestedType<Packet_S_GameState>();
			packetProcessor.RegisterNestedType<Packet_S_PlayerState>();

			packetProcessor.SubscribeReusable<Packet_S_PlayerJoined>(OnPlayerJoined);
			packetProcessor.SubscribeReusable<Packet_S_JoinAccept>(OnJoinAccept);
			packetProcessor.SubscribeReusable<Player_S_DisconnectPacket>(OnPlayerDisconnect);

			writer = new NetDataWriter();

			this.stopwatch = new Stopwatch();
			Random rnd = new Random();

			username = Environment.MachineName + " " + rnd.Next(0, 10) + rnd.Next(0, 10) + rnd.Next(0, 10);


			// ECS init 
			pipeline_fixed = new();
			Stage stage_update = new Stage();

			stage_update.Add(Client.Systems.ClientInput);
			stage_update.Add(Shared.Systems.Player_Move);
            stage_update.Add(Client.Systems.ClientStateBuffer);
            

            pipeline_fixed.AddStage(stage_update);



			netManager = new NetManager(this)
			{
				AutoRecycle = true,
				//SimulatePacketLoss = true,
				SimulateLatency = true,
				DisconnectTimeout = int.MaxValue,
			};
			netManager.Start();
			netManager.Connect(address, port, "");

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

				tick_local = NetworkCommon.TickAdvance(tick_local); 

				netManager.PollEvents();

				// 
				pipeline_fixed.Update(clientWorld, 1f / 60f);

				stopwatch.Stop();
				Thread.Sleep((int)(MathF.Max(0, msPerTick - stopwatch.ElapsedMilliseconds)));
				// Update target msPerTick to be in sync with server
				// slow down if too ahead of server
				// speed up if behind server
                /*
				if (NetworkCommon.SeqDiff(tick_local, tick_remote) > 20)
					Thread.Sleep(3);*/
			}
		}
		public void Stop()
		{
			netManager.Stop();
		}

		public void SendPacketSerializable<T>(PacketType type, T packet, DeliveryMethod deliveryMethod) where T : INetSerializable
		{
			if (server == null)
				return;
			writer.Reset();
			writer.Put((byte)type);
			packet.Serialize(writer);
			server.Send(writer, deliveryMethod);
		}
		public void SendPacket<T>(T packet, DeliveryMethod deliveryMethod) where T : class, new()
		{
			if (server == null)
				return;
			writer.Reset();
			writer.Put((byte)PacketType.Serialized);
			packetProcessor.Write(writer, packet);
			server.Send(writer, deliveryMethod);
		}

		private void OnPlayerJoined(Packet_S_PlayerJoined packet)
		{
			Debug.WriteLine("[C] Player Joined: " + packet.Username);
			//
			Entity e = clientWorld.CreateEntity();
			e.Add(new BundlePlayer(new(), new(packet.InitialPlayerState.positionX, packet.InitialPlayerState.positionY, 0, 0, 0.5f, 0.5f)));

			if (packet.clientID == localPlayerID)
			{
				localPlayerEntityID = e.ID;
			}
			// Spawn new player
			players.Add(packet.clientID, new ClientPlayer(packet.clientID, packet.Username, e.ID));
		}

		/// <summary>
		/// When we get accepted to joint the server
		/// </summary>
		/// <param name="packet"></param>
		private void OnJoinAccept(Packet_S_JoinAccept packet)
		{
			Debug.WriteLine("[C] Join Acepted, got assigned id: " + packet.clientID);
			tick_remote = tick_local = packet.ServerTick;
			localPlayerID = packet.clientID;
		}

		private void OnPlayerDisconnect(Player_S_DisconnectPacket packet)
		{
			players.Remove(packet.clientID);
		}

		// When we recevied input from another player
		private void OnStateReceived(NetPacketReader reader, NetPeer peer)
		{
			Packet_S_GameState packet = new Packet_S_GameState();
			packet.Deserialize(reader);
			tick_remote = packet.tick_server;

            int localadvantage = NetworkCommon.SeqDiff(tick_local, packet.tick_server);

            if(localadvantage < 0)
            {

            }

            // Drop packets before latest server ack
            while (buffer_input.Count > localadvantage && buffer_input.Count > 0 && localadvantage > 0)
            {
                buffer_input.Dequeue();
                buffer_state.Dequeue();
            }

            for (int i = 0; i < packet.playerstates.Length; i++)
			{
               Packet_S_PlayerState state = packet.playerstates[i];

                if (players.Count <= i)
                    break;
				ClientPlayer client = players[state.id_network];
				
				
				// If id_network is equal us.
				// if packet is older check if its

				if (clientWorld.GetEntity(players[state.id_network].entityID).Unwrap(out var e))
				{
					Transform2D transform = e.Get<Transform2D>();
					transform.x = state.state.positionX;
					transform.y = state.state.positionY;
					e.Set(transform);


					// If is local player
					if (packet.playerstates[i].id_network == localPlayerID)
					{   
                        if (buffer_state.Count > 0 && !buffer_state.Peek().Equals(Utils.GetPlayerState(e)))
                        {
                            Debug.WriteLine($"[C] {tick_local} Client misspredict");
                            // For each stored input since last ack
                            foreach (var item in buffer_input.Reverse())
                            {
                                // Reapply new input
                                Shared.Systems.ApplyInputToPlayerEntity(e, item, 1f / 60f);
                            }
                        }
					}
				}
			}
		}


        private void OnAckReceived(NetPacketReader reader, NetPeer peer)
        {
            Packet_S_ACK packet = new();
            packet.Deserialize(reader);

            int localadvantage = NetworkCommon.SeqDiff(tick_local, packet.tick_ack);

            // Drop packets before latest server ack
            while (buffer_input.Count > localadvantage && buffer_input.Count > 0)
            {
                buffer_input.Dequeue();
            }
        }

        #region Network events
        void INetEventListener.OnConnectionRequest(ConnectionRequest request)
		{
		    request.Reject();
		}
		void INetEventListener.OnNetworkError(IPEndPoint endPoint, SocketError socketError)
		{
			Debug.WriteLine("[C] NetworkError: " + socketError);
		}
		void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency)
		{
			halfRTT = latency;
		}
		void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
		{
			byte packetType = reader.GetByte();
			PacketType pt = (PacketType)packetType;
			switch (pt)
			{
				case PacketType.State:
					OnStateReceived(reader, peer);
					break;
                case PacketType.Ack:
                    OnAckReceived(reader, peer);
                    break;
                case PacketType.Serialized:
					packetProcessor.ReadAllPackets(reader);
					break;
				default:
					Debug.WriteLine("Unhandled packet: " + pt);
					break;
			}
		}
		void INetEventListener.OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
		{

		}
		void INetEventListener.OnPeerConnected(NetPeer peer)
		{
			Debug.WriteLine("[C] Connected to server: " + peer.EndPoint);
			server = peer;
			SendPacket(new Packet_P_JoinPacket { UserName = username }, DeliveryMethod.ReliableOrdered);
		}
		void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
		{
			Debug.WriteLine("[C] Disconnected from server: " + disconnectInfo.Reason);
		}
		#endregion
	}
}
