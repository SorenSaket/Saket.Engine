using Saket.ECS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Net.Realtime
{
    public class RTInputSender<ClientInput>
    {
        /// <summary> The last recived tick of the client </summary>
        public ushort tick_remote;

        /// <summary> The last client tick the server simulated</summary>
        public ushort tick_lastSim;

        /// <summary>
        /// Input Buffer
        /// TODO convert to faster datastructure
        /// </summary>
        public Dictionary<ushort, ClientInput> inputs = new();

        /// <summary>
        /// Many many input packets lost over 100 packets
        /// </summary>
        public float packetloss_avg = 0;
    }

    public class RTInput_Server<ClientInput>
    {
        public Dictionary<IDNet, RTInputSender<ClientInput>> clients = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id_network"></param>
        /// <param name="tick_client"></param>
        /// <param name="inputs"></param>
        /// <exception cref="Exception"></exception>
        public void OnInputRecived(IDNet id_network, ushort tick_client, ClientInput[] inputs)
        {
#if DEBUG
            //
            if (!clients.ContainsKey(id_network))
            {
                throw new Exception($"Recived input for nonexsistent Client with invalid id {id_network}");
            }
#endif
            // throw old state out
            // This is possible since a newer packet would already contain inputs for this tick
            if (NetworkCommon.SeqDiff(tick_client, clients[id_network].tick_remote) < 0)
                return;

            // Todo implement abort
            // if client is too behind reset input buffer & last tick proccsed
            //
            //if(NetworkCommon.SeqDiff(player.tick_remote, player.tick_lastSim ) < 20)
            //{
            //    Debug.WriteLine($"[S] skipping");
            //   player.inputs.Clear();
            //
            //}

            // Update the remote tick
            clients[id_network].tick_remote = tick_client;
            /*
            if (player.firstInput)
            {
                player.tick_lastSim = NetworkCommon.TickAdvance(packet.tick_player, -(packet.inputs.Length -1));
                player.firstInput = false;
            }*/

            // Maintain buffer of inputs
            // 0 = oldest
            // (inputs.Length-1) = newest
            for (int i = 0; i < inputs.Length; i++)
            {
                // The client tick the input command was issued
                ushort t = NetworkCommon.TickAdvance(tick_client, -(inputs.Length - 1) + i);

                if (!clients[id_network].inputs.ContainsKey(t) // If we don't already have the input buffered
                    && NetworkCommon.SeqDiff(t, clients[id_network].tick_lastSim) > 0 // If we haven't simulated the tick yet
                    )
                    clients[id_network].inputs.Add(t, inputs[i]);
            }
        }



    }
}
