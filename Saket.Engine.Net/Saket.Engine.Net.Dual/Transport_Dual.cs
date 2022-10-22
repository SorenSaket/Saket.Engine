using Saket.Engine.Net.Transport;
using System.Net;
using System.Net.Sockets;

namespace Saket.Engine.Net.Dual
{
    /// <summary>
    /// Dual transport uses the built in TCPclient and UDPclient from system.Net to send packets
    /// TODO impmenent UDP
    /// </summary>
    public class Transport_Dual : NetworkTransport
    {
        public class Client
        {
            public TcpClient connection_tcp;
        }

        public Dictionary<IDNet, Client> clients = new();

        public Stack<IDNet> avaliableIDs = new();

        // Used for sending and reciving udp packets
       // UdpClient udpClient;

        // Used for sending tcp stream
        public TcpClient tcpClient;

        // Used for reciving tcp stream
        public TcpListener tcplistener;

        private IPEndPoint tcpEndpoint;

        //private IPEndPoint udpEndpoint;

        // Buffer for reading data
        Byte[] bytes = new Byte[256];

        public Transport_Dual(IPEndPoint listenIP)
        {
            tcpEndpoint = listenIP;
        }

        public override IDNet ServerClientId => 0;

        public override void DisconnectLocalClient()
        {
            tcpClient?.Close();
            tcpClient?.Dispose();
        }

        public override void DisconnectRemoteClient(IDNet clientId)
        {
            if (clients.ContainsKey(clientId))
            {
                clients[clientId].connection_tcp?.Close();
                clients[clientId].connection_tcp?.Dispose();
                clients.Remove(clientId);
                avaliableIDs.Push(clientId);
            }
        }

        public override ulong GetCurrentRTT(IDNet clientId)
        {
            throw new NotImplementedException();
        }

        public override Event_Transport PollEvent()
        {
            if(tcplistener != null)
            {
                // Add all new tcp clients
                while (tcplistener.Pending())
                {
                    IDNet id = GetNextID();

                    TcpClient client = tcplistener.AcceptTcpClient();

                    clients.Add(id, new Client() { connection_tcp = client });

                    return new Event_Transport(NetworkEvent.Connect, id, null, 0);
                }

                foreach (var client in clients)
                {
                    Stream stream = client.Value.connection_tcp.GetStream();

                    int avaliableBytes;
                    //This can only run once since it returns
                    while ((avaliableBytes = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        return new Event_Transport(NetworkEvent.Data, client.Key, new ArraySegment<byte>(bytes, 0, avaliableBytes), 0);
                    }
                }
            }
            else
            { 
                // Poll event from server
                while (tcpClient.Available > 0)
                {
                    Stream stream = tcpClient.GetStream();
                    int avaliableBytes;
                    while ((avaliableBytes = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        return new Event_Transport(NetworkEvent.Data, ServerClientId, new ArraySegment<byte>(bytes, 0, avaliableBytes), 0);
                    }
                }
            }
          

            return new Event_Transport(NetworkEvent.Nothing, 0, null, 0);
        }

        uint nextID;

        private IDNet GetNextID()
        {
            if(nextID >= uint.MaxValue)
            {
                return avaliableIDs.Pop();
            }
            else
            {
                nextID++;
                return nextID;
            }
        }


        public override void Send(IDNet clientId, ArraySegment<byte> payload, NetworkDelivery networkDelivery)
        {
            if (tcplistener != null)
            {
                // Send to client
                var stream = clients[clientId].connection_tcp.GetStream();
                if(payload != null && payload.Array != null)
                    stream.Write(payload.Array, payload.Offset, payload.Count);
            }
            else
            {
                // send to server
                var stream= tcpClient.GetStream();
                if (payload != null && payload.Array != null)
                    stream.Write(payload.Array, payload.Offset, payload.Count);
            }
        }

        public override void Shutdown()
        {
            if (tcplistener != null)
            {
                foreach (var client in clients)
                {
                    client.Value.connection_tcp?.Close();
                    client.Value.connection_tcp?.Dispose();
                }
                tcplistener.Stop();
            }
            tcpClient?.Close();
            tcpClient?.Dispose();
        }

        public override bool StartClient()
        {
            try
            {
                tcpClient =  new TcpClient();
                tcpClient.Connect(tcpEndpoint);
                
                return true;
            }
            catch
            {
                return false;
            }
            
        }
        public override bool StartServer()
        {
            try
            {
                tcplistener = new TcpListener(new IPEndPoint(IPAddress.Any, tcpEndpoint.Port));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}