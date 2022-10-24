using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Net.Snapshotting
{    
    // There is currently a lot of overhead in the snapshot data structure
    // A lot of bytes are used for headers that stay mostly the same between syncs
    // A solution is possible

    //

    // Snapshot Data structure
    // i16 : SpawnCount
    // i16 : DestroyCount 
    // i16 : UpdateCount
    // Spawns [UpdateCount]
    // i16 : id_network
    // i16 : type_object
    // Updates [UpdateCount]
    // i16 : id_network
    // i8  : ComponentCount
    //      Components [ComponentCount]
    //      i16 : id_component
    //      byte*X : value
    // Destroyed Objects [DestroyCount]
    // i16 : id_network

    /// <summary>
    /// Snapshots are a subset of the networked game state sent out to clients.
    /// Snapshots are unique for each player, even for the same state tick.
    /// Snapshots are delta compressed against the last acknowledged tick from the client.
    /// 
    /// 
    /// </summary>
    public class Snapshot_B
    {
        public Snapshot_B()
        {
            Spawns = new();
            Updates = new();
            Destroyed = new();
        }

        public List<Spawn> Spawns { get; set; }
        public List<Update> Updates { get; set; }
        public List<Destroy> Destroyed { get; set; }

        public void Clear()
        {
            Spawns.Clear();
            Updates.Clear();
            Destroyed.Clear();
        }

        public struct Spawn
        {
            public UInt16 id_network;
            public UInt16 id_type;

            public Spawn(ushort id_network, ushort id_type)
            {
                this.id_network = id_network;
                this.id_type = id_type;
            }
        }
        public struct Update
        {
            public UInt16 id_network;
            public UInt16[] components;

            public Update(ushort id_network, UInt16[] components)
            {
                this.id_network = id_network;
                this.components = components;
            }
        }
        public struct Component
        {
            public UInt16 id_component;
            public byte[] data;
        }
        public struct Destroy
        {
            public UInt16 id_network;

            public Destroy(ushort id_network)
            {
                this.id_network = id_network;
            }
        }


        public void DeepCopyTo(Snapshot_B other)
        {
            other.Clear();

            //other.Spawns.Capacity
            for (int i = 0; i < Spawns.Count; i++)
            {
                other.Spawns.Add(Spawns[i]);
            }

            for (int i = 0; i < Updates.Count; i++)
            {
                other.Updates.Add(Updates[i]);
            }
        }
    }

}
