using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENet;
using Saket.Networking.Transport;

namespace Saket.Engine.Net.Transport.Enet
{
    public class Transport_Enet : NetworkTransport
    {
        public Transport_Enet()
        {
            ENet.Library.Initialize();
        }
        ~Transport_Enet()
        {
            ENet.Library.Deinitialize();
        }


        public override ulong ServerClientId => throw new NotImplementedException();

        public override void DisconnectLocalClient()
        {
            throw new NotImplementedException();
        }

        public override void DisconnectRemoteClient(ulong clientId)
        {
            throw new NotImplementedException();
        }

        public override ulong GetCurrentRTT(ulong clientId)
        {
            throw new NotImplementedException();
        }

        public override Event_Transport PollEvent()
        {
            throw new NotImplementedException();
        }

        public override void Send(ulong clientId, ArraySegment<byte> payload, NetworkDelivery networkDelivery)
        {
            throw new NotImplementedException();
        }

        public override void Shutdown()
        {
            throw new NotImplementedException();
        }

        public override bool StartClient()
        {
            throw new NotImplementedException();
        }

        public override bool StartServer()
        {
            throw new NotImplementedException();
            using (Host server = new Host())
            {
                Address address = new Address();

                address.Port = 10100;
                server.Create(address, 2000);

                Event netEvent;

                while (!Console.KeyAvailable)
                {
                    bool polled = false;

                    while (!polled)
                    {
                        if (server.CheckEvents(out netEvent) <= 0)
                        {
                            if (server.Service(15, out netEvent) <= 0)
                                break;

                            polled = true;
                        }

                        switch (netEvent.Type)
                        {
                            case EventType.None:
                                break;

                            case EventType.Connect:
                                Console.WriteLine("Client connected - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                                break;

                            case EventType.Disconnect:
                                Console.WriteLine("Client disconnected - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                                break;

                            case EventType.Timeout:
                                Console.WriteLine("Client timeout - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                                break;

                            case EventType.Receive:
                                Console.WriteLine("Packet received from - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP + ", Channel ID: " + netEvent.ChannelID + ", Data length: " + netEvent.Packet.Length);
                                netEvent.Packet.Dispose();
                                break;
                        }
                    }
                }

                server.Flush();
            }
        }
    }

}
