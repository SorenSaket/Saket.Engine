using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Net
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NetworkedEntity
    {
        public ushort id_network;
        public ushort id_objectType;
        public unsafe fixed int interestGroups[16];

        public NetworkedEntity(ushort id_network, ushort type_object) : this()
        {
            this.id_network = id_network;
            this.id_objectType = type_object;
        }
    }
}