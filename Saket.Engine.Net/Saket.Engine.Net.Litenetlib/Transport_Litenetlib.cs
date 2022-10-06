using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

using LiteNetLib;
using LiteNetLib.Utils;

namespace Saket.Engine.Net.Transport.Litenetlib
{
    public class Transport_Litenetlib : NetworkTransport, INetEventListener
    {
        private Queue<Event_Transport> eventQueue = new();

        public NetManager netmanager;

        NetDataWriter writer = new();
        private NetPacketProcessor packetProcessor;

        public IPEndPoint IP = new(IPAddress.Loopback, 6969);

        private bool isClient;
        
        public Transport_Litenetlib()
        {
            netmanager = new NetManager(this);
        }

        public override uint ServerClientId => (uint)server.Id;
        protected NetPeer server;

        public override void DisconnectLocalClient()
        {
            throw new NotImplementedException();
        }

        public override void DisconnectRemoteClient(uint clientId)
        {
            throw new NotImplementedException();
        }

        public override ulong GetCurrentRTT(uint clientId)
        {
            throw new NotImplementedException();
        }

        public override Event_Transport PollEvent()
        {
            netmanager.PollEvents();
            if (eventQueue.TryDequeue(out var e))
            {
                return e;
            }
            return new Event_Transport(NetworkEvent.Nothing, 0, ArraySegment<byte>.Empty, 0);
        }

        public override void Send(uint clientId, ArraySegment<byte> payload, NetworkDelivery networkDelivery)
        {
            writer.Reset();
            writer.Put(payload.Array, payload.Offset, payload.Count);
            server.Send(writer, DeliveryMethodConversion(networkDelivery));
        }

        public override void Shutdown()
        {
            netmanager.Stop();
        }

        public override bool StartClient()
        {
            isClient = true;
            return netmanager.Start();
        }

        public override bool StartServer()
        {
            isClient = false;
            return netmanager.Start();
        }


        void INetEventListener.OnConnectionRequest(ConnectionRequest request)
        {
            if (isClient)
                request.Reject();
            else
                request.Accept();
        }
        void INetEventListener.OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            
        }
        void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }
        void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            var e = new Event_Transport(NetworkEvent.Data, (uint)peer.Id, reader.GetRemainingBytesSegment(), 0);
            eventQueue.Enqueue(e);
            InvokeOnTransportEvent(e);
        }
        void INetEventListener.OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            // do nothing

        }
        void INetEventListener.OnPeerConnected(NetPeer peer)
        {
            if (isClient)
            {
                server = peer;
            }
            var e = new Event_Transport(NetworkEvent.Connect, (uint)peer.Id, ArraySegment<byte>.Empty, 0);
            eventQueue.Enqueue(e);
            InvokeOnTransportEvent(e);
        }
        void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            var e = new Event_Transport(NetworkEvent.Disconnect, (uint)peer.Id, ArraySegment<byte>.Empty, 0);
            eventQueue.Enqueue(e);
            InvokeOnTransportEvent(e);
        }
    
    
    
        static DeliveryMethod DeliveryMethodConversion(Saket.Engine.Net.NetworkDelivery networkDelivery)
        {
            switch (networkDelivery)
            {
                case NetworkDelivery.Unreliable:
                    return DeliveryMethod.Unreliable;

                case NetworkDelivery.UnreliableSequenced:
                    return DeliveryMethod.Sequenced;

                case NetworkDelivery.Reliable:
                    return DeliveryMethod.ReliableUnordered;

                case NetworkDelivery.ReliableSequenced:
                    return DeliveryMethod.ReliableSequenced;


                default:
                    throw new NotSupportedException("");
            }
        }
    
    }
}
