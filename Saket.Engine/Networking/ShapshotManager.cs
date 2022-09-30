using Saket.ECS;
using Saket.Engine.Collections;
using Saket.Engine.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Networking
{
    // Snapshot creation 
    // Max Size.
    // can defer entity updates based on priority
    // can discard entity updates based on groups/rooms?
    //
    //
    //



    // Reference source for used mscorlib classes
    // BinaryWriter/BinaryReader
    // https://referencesource.microsoft.com/#mscorlib/system/io/binarywriter.cs
    // https://referencesource.microsoft.com/#mscorlib/system/io/binaryreader.cs.html

    // Stream
    // https://referencesource.microsoft.com/#mscorlib/system/io/stream.cs

    // MemoryStream
    // https://referencesource.microsoft.com/#mscorlib/system/io/memorystream.cs

    /// <summary>
    /// Snapshots are parts
    /// </summary>
    class Snapshot
    {
        // ---- Data ----
        
        //
        //
        // id_network   : 0000_0000_0000_0000
        // id_component : 0000_0000_0000_0000 
        // value        : X
        //
        //
        //


        public List<Update> Updates { get; set; }
        public ulong length;

        public void Reset()
        {
            length = 0;
        }

        public struct Update
        {
            UInt16 id_network;
            UInt16 id_component;
            byte[] data;
        }
    }



    internal class ShapshotManager
    {
        private static Query query = new Query().With<NetworkedEntity>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="world"></param>
        /// <param name="snapshot_delta">The snapshot to provide delta compression against</param>
        public static void CreateSnapshot(World world, Snapshot snapshot_delta)
        {
            byte[] data = new byte[100];

            // Base array size off old snapshot size if avaliable
            
            // Collect 
            var entities = world.Query(query);

            var writer = new SerializerWriter(data);

            // Iterate over all archetypes
            foreach (var archetype in world.archetypes)
            {
                // Only handle archetypes with NetworkedEntities
                if (!archetype.Has<NetworkedEntity>())
                    continue;

                // For each entity in archetype
                foreach (var item in archetype)
                {
                    bool dirty = false;

                    // For each component/storage on archetype
                    foreach (var component in archetype.storage)
                    {
                        // Only handle registered networked components
                        if (!networkedComponents.Contains(component.Key))
                            continue;

                        // Only add component if its dirty

                        // Only add compnent if its change threshhold has been met

                        // Only add component if its in 


                        //writer.Write(entity.Get<NetworkedEntity>().id_network);

                        unsafe
                        {
                            writer.Serialize((ushort)i);
                            writer.Write(component.Value.Get(item), component.Value.ItemSizeInBytes);
                        }
                    }
                }
            }
        }







        public static void ApplySnapShot(World world, Snapshot snapshot)
        {
            // For each id_network
            for (int i = 0; i < snapshot; i++)
            {

            }
        }


        private static List<Type> networkedComponents = new();

        public void RegisterNetworkedComponent<T>()
        {
#if DEBUG
            if (networkedComponents.Contains(typeof(T)))
                throw new ArgumentException($"Typeof {typeof(T).Name} is already registered");

            if(networkedComponents.Count == ushort.MaxValue)
                throw new ArgumentException($"Limit of Networked Components reached");
#endif
            networkedComponents.Add(typeof(T));
        }
    }
}
