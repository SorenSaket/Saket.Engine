using Saket.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saket.Engine.Serialization;
using System.Security.AccessControl;

namespace Saket.Engine.Net.Snapshotting.A
{
    public static class Client_Snapshot_A
    {
        public struct SpawnDesrtroyCallBack
        {
            public Action<Entity> OnSpawn;
            public Action<Entity> OnDestroy;
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

    }
}