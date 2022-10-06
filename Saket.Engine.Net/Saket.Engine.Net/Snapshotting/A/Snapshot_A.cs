using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Net.Snapshotting.A
{
    // Snapshot_A Networked Data Structure
    // u32 : ObjectCount
    // Objects [ObjectCount]
    // u16 : id_network
    // u16 : id_objectType
    //      Components [ComponentCount]
    //          byte*X : value

    // Component count is held within the client schema
    //      Component Types [ComponentCount]
    //          u16 : id_component



    public class Snapshot_A
    {
        public uint numberOfEntities;
        public Dictionary<UInt16, NetworkedObject> objects;

        public byte[] data_components;

        public Snapshot_A()
        {
            this.objects = new();
            this.data_components = new byte[128];
            numberOfEntities = 0;
        }

        public struct NetworkedObject
        {
            public UInt16 id_network;
            public UInt16 id_objectType;
            public int relativeDataPtr;
        }
    }
}