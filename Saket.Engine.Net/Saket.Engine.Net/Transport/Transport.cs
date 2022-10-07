using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Net.Transport
{
    /// <summary>
    /// The generic transport class all Netcode for GameObjects network transport implementations derive from. Use this class to add a custom transport.
    /// </summary>
    public abstract class NetworkTransport
    {
        /// <summary>
        /// A constant `clientId` that represents the server
        /// When this value is found in methods such as `Send`, it should be treated as a placeholder that means "the server"
        /// </summary>
        public abstract IDNet ServerClientId { get; }

        /// <summary>
        /// Delegate for transport network events
        /// </summary>
        public delegate void TransportEventDelegate(Event_Transport transportEvent);

        /// <summary>
        /// Occurs when the transport has a new transport network event.
        /// Can be used to make an event based transport instead of a poll based.
        /// </summary>
        public event TransportEventDelegate OnTransportEvent;



        /// <summary>
        /// Invokes the <see cref="OnTransportEvent"/>. Invokation has to occur on the Unity thread in the Update loop.
        /// </summary>
        /// <param name="eventType">The event type</param>
        /// <param name="clientId">The clientId this event is for</param>
        /// <param name="payload">The incoming data payload</param>
        /// <param name="receiveTime">The time the event was received, as reported by Time.realtimeSinceStartup.</param>
        protected void InvokeOnTransportEvent(Event_Transport transportEvent)
        {
            OnTransportEvent?.Invoke(transportEvent);
        }

        /// <summary>
        /// Send a payload to the specified clientId, data and networkDelivery.
        /// </summary>
        /// <param name="clientId">The clientId to send to</param>
        /// <param name="payload">The data to send</param>
        /// <param name="networkDelivery">The delivery type (QoS) to send data with</param>
        public abstract void Send(IDNet clientId, ArraySegment<byte> payload, NetworkDelivery networkDelivery);

        /// <summary>
        /// Polls for incoming events, with an extra output parameter to report the precise time the event was received.
        /// </summary>
        /// <param name="clientId">The clientId this event is for</param>
        /// <param name="payload">The incoming data payload</param>
        /// <param name="receiveTime">The time the event was received, as reported by Time.realtimeSinceStartup.</param>
        /// <returns>Returns the event type</returns>
        public abstract Event_Transport PollEvent();

        /// <summary>
        /// Connects client to the server
        /// </summary>
        /// <returns>Returns success or failure</returns>
        public abstract bool StartClient();

        /// <summary>
        /// Starts to listening for incoming clients
        /// </summary>
        /// <returns>Returns success or failure</returns>
        public abstract bool StartServer();

        /// <summary>
        /// Disconnects a client from the server
        /// </summary>
        /// <param name="clientId">The clientId to disconnect</param>
        public abstract void DisconnectRemoteClient(IDNet clientId);

        /// <summary>
        /// Disconnects the local client from the server
        /// </summary>
        public abstract void DisconnectLocalClient();

        /// <summary>
        /// Gets the round trip time for a specific client. This method is optional
        /// </summary>
        /// <param name="clientId">The clientId to get the RTT from</param>
        /// <returns>Returns the round trip time in milliseconds </returns>
        public abstract ulong GetCurrentRTT(IDNet clientId);

        /// <summary>
        /// Shuts down the transport
        /// </summary>
        public abstract void Shutdown();

    }
}
