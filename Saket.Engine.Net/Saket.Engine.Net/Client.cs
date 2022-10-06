
using Saket.Engine.Net;
using Saket.Engine.Net.Snapshotting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Net
{
	public class Client<PlayerInput>
    {
        public int id_network;

        /// <summary>  The associated entity with the player </summary>
        public int id_entity;


        /// <summary> The last recived tick of the client </summary>
        public ushort tick_remote;

        /// <summary> The last client tick the server simulated</summary>
        public ushort tick_lastSim;
        
        /// <summary>  Current ping of player in ms</summary>
        public int Ping;

        /// <summary>
        /// Input Buffer
        /// TODO convert to faster datastructure
        /// </summary>
        public Dictionary<ushort, PlayerInput> inputs = new();

		/// <summary>
		/// Many many input packets lost over 100 packets
		/// </summary>
		public float packetloss_avg = 0;

        /// <summary>
        /// Last acknowledged Snapshot
        /// </summary>
        public Snapshot snapshot_previous = new();

        /// <summary>
        /// 
        /// </summary>
        public Snapshot snapshot_next = new();

        public Client(int network_id, int entityID)
		{
            this.id_network = network_id;
            this.id_entity = entityID;
		}
    }
}
