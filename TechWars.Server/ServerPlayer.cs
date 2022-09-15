using LiteNetLib;
using Saket.Engine.Collections;
using Saket.Engine.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TechWars
{
	public class ServerPlayer
	{
        public string Name => username;

        /// <summary>
        /// Current ping of player
        /// </summary>
        public int Ping;
        /// <summary>
        /// 
        /// </summary>
        public int entityID;

        public int id;

        public NetPeer peer;

        private string username;

        public ushort tick_remote;
		//
		// [0] The current command
        public RingBuffer<PlayerInput?> inputs;



        public bool dirty = false;


		/// <summary>
		/// average packetloss over 100 packets
		/// Not acutally xsfxfdxf
		/// </summary>
		public float packetloss_avg;


		public ServerPlayer(string username, NetPeer peer, int entityID)
		{
            this.username = username;
            this.peer = peer;
			id = peer.Id;
			peer.Tag = this;
            this.entityID = entityID;
			inputs = new(20);
		}
    }
}
