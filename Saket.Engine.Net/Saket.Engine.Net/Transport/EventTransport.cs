using Saket.Engine.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Net.Transport
{
    public struct Event_Transport
    {
        public NetworkEvent Type;
        public IDNet id_client;
        public ArraySegment<byte> data;
        public float time;

        public Event_Transport(NetworkEvent @event, IDNet id_client, ArraySegment<byte> data, float time)
        {
            this.Type = @event;
            this.id_client = id_client;
            this.data = data;
            this.time = time;
        }
    }

}
