using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Net.Realtime
{
    class RTInputClient<ClientInput>
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

    internal class Service_RTInput<ClientInput>
    {
        public Dictionary<int, RTInputClient<ClientInput>> clients = new();

        public void OnInputRecived(int id_network, ClientInput input, ushort tick_client)
        {
#if DEBUG
            //
            if (!clients.ContainsKey(id_network))
            {
                throw new Exception($"Recived input for nonexsistent Client with invalid id {id_network}");
            }
#endif
            // throw old state out
            if (NetworkCommon.SeqDiff(tick_client, clients[id_network].tick_remote) < 0)
                return;

            // Todo implement abort
            // if client is too behind reset input buffer & last ick proccsed
            //


            /*
            if(NetworkCommon.SeqDiff(player.tick_remote, player.tick_lastSim ) < 20)
            {
                Debug.WriteLine($"[S] skipping");
                player.inputs.Clear();

            }*/


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
            for (int i = 0; i < packet.inputs.Length; i++)
            {
                // The client tick the input command was issued
                ushort t = NetworkCommon.TickAdvance(packet.tick_player, -(packet.inputs.Length - 1) + i);

                if (!player.inputs.ContainsKey(t) && NetworkCommon.SeqDiff(t, player.tick_lastSim) > 0)
                    player.inputs.Add(t, packet.inputs[i]);
            }
        }
    }
}
