using Saket.ECS;
using Saket.Engine.Collections;
using Saket.Engine.Net.Rollback;
using Saket.Engine.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Net.Snapshotting
{
    // Reference source for used mscorlib classes
    // Would love to use built in classes in system however they are slow and bloated

    // BinaryWriter/BinaryReader
    // https://referencesource.microsoft.com/#mscorlib/system/io/binarywriter.cs
    // https://referencesource.microsoft.com/#mscorlib/system/io/binaryreader.cs.html
    // Stream
    // https://referencesource.microsoft.com/#mscorlib/system/io/stream.cs
    // MemoryStream
    // https://referencesource.microsoft.com/#mscorlib/system/io/memorystream.cs



    // ---- Snapshot creation ---
    //
    // Max Size. "real-world MTU is 1500 bytes."
    // can defer entity updates based on priority
    // can discard entity updates based on groups/rooms?
    // 
    // Make spawning implicit in updates?
    // 
    // Spawn/Destroy might notification may be sent multiple times between acks from client
    // Clients should ignore duplicates
    // Clients have registered routines for when Spawn/destroy is called


    // ---- Interest system ----
    // 
    // Networked Entity can at max be in 16 interestGroups at a time
    // Update/Spawn/Destroy is only sent to other in the same interestGroup
    // By default all entities are in interestGroup 0

    // As an example Spatial Hashing based interest systems are the most simple and perform well
    // The player is at most in 9 interest groups at a time. The tile and all surrounding

    // ---- Priority system ----
    //
    // Both NetworkedEntities and Networked Components have a priority that can change at runtime
    // A function that can calculate priority of a NetworkedEntity relative to client network_id
    //
    //
    // If Networked Component priority is set to 0 Updates will not get sent. However Spawn/Destroy will alway be sent
    // They contain a Priority Accumulator that increase with the ComponentPriority*EntityPriority.
    // Then snapshot size is limited a component is only sent if the priority has reached a certain threshhold.


    // ---- Automatic Snapshot Interpolation ----
    // Provides automatic linear clientside component interpolation based on two snapshots
    // All components that implement Saket.Engine.Networking.IInterpolatable will be interpolated
    // Interpolation speed is based on server sendrate so that t=1.0 when the next snapshot should arrive


    // ---- Extrapolation  ----
    // Maybe 

    // ---- Network_id recycling ----
    // WIP

    public class SnapshotReceiver
    {        
        /// <summary>  The associated entity with the player </summary>
        public int id_entity;

        /// <summary> Last Acknowledged Snapshot. Used to delta compress against</summary>
        public Snapshot snapshot_previous = new();

        /// <summary>  </summary>
        public Snapshot snapshot_next = new();

        /// <summary>  </summary>
        public PriorityAccumulator accumulator = new();
    }

    /// <summary>
    /// 
    /// </summary>
    public class Service_Snapshot
    {
        // Todo: Remove. This complicates the API unesserarily
        public List<Type> networkedComponents = new();

        /// <summary>
        /// <list type="table">
        /// <item>
        ///     <term>Key</term>
        ///     <description>Unique Client Identifier</description>
        /// </item>
        /// <item>
        ///     <term>Value</term>
        ///     <description>Snapshot specific client data</description>
        /// </item>
        /// </list>
        /// </summary>
        public Dictionary<int, SnapshotReceiver> clients = new();

        private readonly static Query query = new Query().With<NetworkedEntity>();

        // We need to keep track of spawned objects to compare to

        /// <summary>
        /// TODO a way to figure out when enities have been spawned on the world
        /// TODO add delta compression
        /// TODO speed up by like a lot
        /// </summary>

        /// <param name="snapshot_base"> The current snapshot. Snapshots are reused to prevent generating garbage.</param>
        /// <param name="snapshot_delta">The snapshot to provide delta compression against</param>

        /// <param name="state_base">Current World State</param>
        /// <param name="state_delta">World State when snapshot_delta was created </param>

        /// <param name="id_network_reciver">id_network of the reciver client. Used to calculate priorities</param>
        /// <param name="group"> The group to filer against </param>
        public void UpdateSnapshot(  
            Snapshot snapshot_base, 
            Snapshot snapshot_delta,
            GameState state_base, 
            GameState state_delta,
            int id_network_reciver, int group = 0)
        {
            // Clear the old snapshot data
            snapshot_base.Clear();

            // ---- Populate snapshot with new data ----
            
            // Iterate over all archetypes to find networked objects
            foreach (var archetype in state_base.archetypes)
            {
                // Only handle archetypes with NetworkedEntities
                if (!archetype.Has<NetworkedEntity>())
                    continue;

                // For each entity in archetype
                foreach (var row in archetype)
                {
                    // Get the entity
                    NetworkedEntity networkedEntity = archetype.Get<NetworkedEntity>(row);
                    ushort id_networkedEntity = networkedEntity.id_network;
                    ushort type_networkedEntity = networkedEntity.id_objectType;

                    List<UInt16> componentsToUpdate = new();

                    // Only add component if they're in the same interestGroup
                    // check if object left the group and call destroy for entity
                    unsafe
                    {
                        if (!Utilities.IsInGroup(networkedEntity.interestGroups, group, 16))
                            continue;
                    }

                    // Check if the object is new
                    
                    if (state_base.archetypes.Contains(archetype))
                    {
                        
                    }



                    // For each component/storage on archetype
                    foreach (var component in archetype.storage)
                    {
                        // Only handle registered networked components
                        int id_component = networkedComponents.IndexOf(component.Key);
                        if (id_component == -1)
                            continue;
                        
                        

                        // Only add if component changed since the last snapshot
                        // PROBLEM HERE Is that it will constantly switch between sending and not sending
                        // Only add component if its change threshhold has been met
                        /*
                        if(snapshot_delta != null)
                        {
                            if(snapshot_delta.Updates.FirstOrNull(x=>x.id_network == id_networkedEntity).Unwrap(out var update))
                            {
                                if(update.components.)
                            }
                        }*/
                        componentsToUpdate.Add((ushort)id_component);
                    }

                    snapshot_base.Updates.Add(new Snapshot.Update(id_networkedEntity, componentsToUpdate.ToArray()));
                }
            }

            // If the object is new add to added list
            // Check if state_base contains an entity that state_delta doesn't
            // Check that snapshot_delta doesn't contain a spawn of the entity
            // Add Spawn of entity to snapshot_base
            // Do the same for destroy
            // if (!SnapshotContainsEntity(snapshot_delta, id_networkedEntity))
            //{
            //    snapshot_base.Spawns.Add(new Snapshot.Spawn(id_networkedEntity, type_networkedEntity));
            // }


            // Truths:
            // All objects that exist in snapshot_base that doesn't exsist in snapshot_delta should be in new group
            // All objects that exist in snapshot_delta that doesn't exsist in snapshot_base should be in destroyed group
            // Objects that exist in state_base and not in state_delta should be in new group
            // Objects that exist in state_delta and not in state_base should be in destroy group
            // Only objects in in same interestGroup should be in update

            // Things to maybe consider:
            // 1. When new networked components are added or removed
            //
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id_network">The Network Identification of the Client</param>
        public void AcknowledgeRecivedSnapshot(int id_network)
        {
            // TODO DEEP COPY OF SNAPSHOT
            //clients[id_network].snapshot_previous = clients[id_network].snapshot_next;
        }


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