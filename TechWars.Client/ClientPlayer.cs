using LiteNetLib;
using Saket.Engine.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechWars.Client
{
    public class ClientPlayer
    {        
        /// <summary> Network PeerID. Used to identify players across networks </summary>
        public int id;
        /// <summary> Player Username </summary>
        public string username;
        /// <summary> ID corresponding to the locally run ECS </summary>
        public int entityID;
        public ClientPlayer(int id, string username,  int entityID)
        {
            this.id = id;
            this.username = username;
            this.entityID = entityID;
        }
    }
}
