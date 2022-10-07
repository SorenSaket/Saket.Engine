using Saket.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saket.Engine.Serialization;
using System.Security.AccessControl;
using Saket.Engine.Net.Rollback;

namespace Saket.Engine.Net.Snapshotting.A
{
    public class Snapshot_A_Client
    {
        //
        private static readonly Query networkedEntities = new Query().With<NetworkedEntity>();

        private Snapshot_A snapshot_previous = new();
        private Snapshot_A snapshot_next = new();
        private Schema schema;
        private byte[] buffer = new byte[64];

        private float timer_lerp_state = 0;

        public Snapshot_A_Client(Schema schema)
        {
            this.schema = schema;
        }

        /// <summary>
        /// Applies snapshot to world
        /// </summary>
        /// <param name="world"></param>
        public void System_ApplySnapshot(World world)
        {
            timer_lerp_state += world.Delta;
            float t = timer_lerp_state / (1f/30f);
            // Go trough all the objects that aren't in snapshot next but in snapshot previous. Destroy all of them. invoke destroy callback for the destroyed object type

            // Go trough all objects that are in snapshot next but not in snapshot previous. Spawn them.invoke spawn  callback for the spawned object type


            // Get the current time between snapshot this and next
            // Interpolate between values that implements IInterpolatable
            // All other values are just set to snapshot next values
            // Apply to world

            var entities = world.Query(networkedEntities);

            // Comment( Soren ): This code is becoming obscure at best
            // Should uninterpolated values be based off previous or next snapshot?
            foreach (var entity in entities)
            {
                var net = entity.Get<NetworkedEntity>();
                var schema_object = schema.networkedObjects[net.id_objectType];

                if (snapshot_next.objects.ContainsKey(net.id_network))
                {
                    var obj_next = snapshot_next.objects[net.id_network];

                    for (int i = 0; i < schema_object.componentTypes.Length; i++)
                    {
                        var schema_component = schema.networkedComponents[schema_object.componentTypes[i]];
                        // If the previous snapshot also contains
                        if (snapshot_previous.objects.ContainsKey(net.id_network) && schema_component.interpolationFunction != null)
                        {
                            var obj_prev = snapshot_previous.objects[net.id_network];

                            unsafe
                            {
                                fixed (byte* ptr_prev = snapshot_previous.data_components)
                                {
                                    fixed (byte* ptr_next = snapshot_next.data_components)
                                    {
                                        schema_component.interpolationFunction.Invoke(buffer,
                                            new ArraySegment<byte>(snapshot_previous.data_components, obj_prev.relativeDataPtr + schema_object.componentOffsets[i], schema_component.sizeInBytes),
                                            new ArraySegment<byte>(snapshot_next.data_components, obj_next.relativeDataPtr + schema_object.componentOffsets[i], schema_component.sizeInBytes),
                                            t
                                            );
                                    }
                                }
                            }
                        }
                        else
                        {
                            unsafe
                            {
                                fixed (byte* ptr = snapshot_next.data_components)
                                {
                                    entity.Set(
                                        schema_component.type_component,
                                        entity.EntityPointer.index_row,
                                        (void*)ptr[obj_next.relativeDataPtr + schema_object.componentOffsets[i]]);
                                }
                            }
                        }
                    }
                }
            }
        }

        
        public void ReciveSnapshot(byte[] data)
        {
            // Reset lerping
            timer_lerp_state = 0;

            // Switch references
            Snapshot_A s = snapshot_previous;
            snapshot_previous = snapshot_next;
            snapshot_next = s;

            // 
            ReadSnapShotA(snapshot_next, data, schema);
        }
        
        
        /// <summary>
        /// TODO add destroy callback
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="world"></param>
        /// <param name="schema"></param>
        /// <param name="actions"></param>
        /// <param name="networkedEntities"></param>
        /// <exception cref="Exception"></exception>
        public static void ApplySnapshot(byte[] snapshot, World world, Schema schema, Dictionary<int, SpawnDesrtroyCallBack> actions, Dictionary<ushort, int> networkedEntities)
        {
            SerializerReader reader = new SerializerReader(snapshot);

            uint numberOfEntities = reader.Read<uint>();
            Span<int> netEntities = stackalloc int[(int)numberOfEntities];
            
            for (int i = 0; i < numberOfEntities; i++)
            {
                var id_network = reader.Read<ushort>();
                var id_objectType = reader.Read<ushort>();

                if(!schema.networkedObjects.FirstOrFalse(x=>x.id_object == id_objectType, out var schema_object))
                {
                    throw new Exception($"Schema doesn't cotain schema for object with id {id_objectType}");
                }

                Entity? entity = null;

                if (!networkedEntities.ContainsKey(id_network)) 
                {
                    entity = world.CreateEntity();
                    actions[id_objectType].OnSpawn?.Invoke(entity.Value);
                }
                else
                {
                    entity = world.GetEntity(networkedEntities[id_network]);
                }

                // Fill all values
                for (int y = 0; y < schema_object.componentTypes.Length; y++)
                {
                    unsafe
                    {
                        ArraySegment<byte> componentData = reader.Read(schema.networkedComponents[schema_object.componentTypes[y]].sizeInBytes);
                        fixed(byte* ptr = componentData.Array)
                        {
                            entity.Value.Set(
                             schema.networkedComponents[schema_object.componentTypes[y]].type_component,
                             entity.Value.EntityPointer.index_row,
                             ptr+componentData.Offset
                             );
                        }
                     
                    }
                }
            }

            // Remove missing entities
            foreach (var entity in networkedEntities)
            {
                if (!netEntities.Contains(entity.Key))
                {
                    //actions[entity.Value.].OnSpawn?.Invoke(entity.Value);
                    world.DestroyEntity(entity.Value);
                }
            }
        }
    
    
        public static void ReadSnapShotA(Snapshot_A snapshot, ArraySegment<byte> data, Schema schema)
        {
            SerializerReader reader = new SerializerReader(data.Array);
            snapshot.numberOfEntities = reader.Read<uint>();
            snapshot.objects.Clear();
            snapshot.objects.EnsureCapacity((int)snapshot.numberOfEntities);
            
            // Copy the data to the snapshot. Ensure that this is nessary.
            // Just referencing the incoming could lead to errors if the source array is reused
            Array.Resize(ref snapshot.data_components, data.Count);
            data.CopyTo(snapshot.data_components, 0);

            //
            for (int i = 0; i < snapshot.numberOfEntities; i++)
            {
                Snapshot_A.NetworkedObject obj = new Snapshot_A.NetworkedObject();
                obj.id_network = reader.Read<ushort>();
                obj.id_objectType = reader.Read<ushort>();
                obj.relativeDataPtr = reader.AbsolutePosition;
                
                if (!schema.networkedObjects.FirstOrFalse(x => x.id_object == obj.id_objectType, out var schema_object))
                {
                    throw new Exception($"Schema doesn't cotain schema for object with id {obj.id_objectType}");
                }

                reader.AbsolutePosition += schema_object.sizeInBytes;

                snapshot.objects.Add(obj.id_network, obj);
            }
        }

        public struct SpawnDesrtroyCallBack
        {
            public Action<Entity> OnSpawn;
            public Action<Entity> OnDestroy;
        }
    }
}