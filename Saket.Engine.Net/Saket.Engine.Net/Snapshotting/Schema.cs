using Saket.ECS;
using Saket.Serialization;
using Saket.Serialization.Byte;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Net.Snapshotting
{
    public class Schema
    {
        public NetworkedComponent[] networkedComponents;
        public NetworkedObject[] networkedObjects;

        /// <summary>
        /// Converts Userspace networked object into NetworkedObject
        /// </summary>
        /// <param name="networkedObjects"></param>
        /// <param name="networkedComponents"></param>
        public Schema(UserNetworkedObject[] networkedObjects, NetworkedComponent[] networkedComponents = null)
        {
            List<NetworkedComponent> components = new();
            if (networkedComponents != null)
                components.AddRange(networkedComponents);

            List<NetworkedObject> objs = new();
            for (int id_object = 0; id_object < networkedObjects.Length; id_object++)
            {
                // 
                uint[] componentsInObject = new uint[networkedObjects[id_object].componentTypes.Length];
                int[] offsets = new int[networkedObjects[id_object].componentTypes.Length];
                int totalSize = 0; 

                for (int j = 0; j < networkedObjects[id_object].componentTypes.Length; j++)
                {
                    offsets[j] = totalSize;
                    int id_component = components.FindIndex(x => x.type_component == networkedObjects[id_object].componentTypes[j]);

                    if (id_component == -1)
                    {
                        componentsInObject[j] = (uint)components.Count;
                        int size = Marshal.SizeOf(networkedObjects[id_object].componentTypes[j]);
                        totalSize += size;
                        components.Add(new NetworkedComponent((uint)components.Count, networkedObjects[id_object].componentTypes[j], null));
                    }
                    else
                    {
                        componentsInObject[j] = (uint)id_component;
                        totalSize += components[id_component].sizeInBytes;
                    }
                }

                objs.Add(new NetworkedObject((uint)id_object, componentsInObject, offsets, totalSize, networkedObjects[id_object].spawnFunction, networkedObjects[id_object].destroyFunction));
            }
            this.networkedComponents = components.ToArray();
            this.networkedObjects = objs.ToArray();
        }


        public delegate void InterpolationFunction(ByteWriter dest, ByteReader from, ByteReader to, float t);

        public delegate void DestroyFunction(Entity entity);
        public delegate void SpawnFunction(Entity entity);

        public struct NetworkedComponent
        {
            /// <summary> identifier for this component </summary>
            public uint id_component;
            /// <summary> the type of the component </summary>
            public Type type_component;
            /// <summary> The total size in bytes </summary>
            public int sizeInBytes;
            /// <summary> Optional interpolation function. </summary>
            public InterpolationFunction? interpolationFunction;

            public NetworkedComponent(uint id_component, Type type_component, InterpolationFunction? interpolationFunction)
            {
                this.id_component = id_component;
                this.type_component = type_component;
                this.sizeInBytes = Marshal.SizeOf(type_component);
                this.interpolationFunction = interpolationFunction;
            }
        }
        public struct NetworkedObject
        {
            /// <summary> identifier for this object </summary>
            public uint id_object;
            /// <summary> identifiers for this components syncronized on this object</summary>
            public uint[] componentTypes;
            /// <summary> Byte offests of each compononents</summary>
            public int[] componentOffsets;
            /// <summary> Total byte size of all components combined in this object. </summary>
            public int sizeInBytes;
            /// <summary> Function to call when this object is spawned on the client.</summary>
            public SpawnFunction? spawnFunction;
            /// <summary> Function to call when this object is destroyed on the client.</summary>
            public DestroyFunction? destroyFunction;
            
            public NetworkedObject(uint id_object, uint[] componentTypes, int[] componentOffsets, int sizeInBytes, SpawnFunction? spawnFunction = null, DestroyFunction? destroyFunction = null)
            {
                this.id_object = id_object;
                this.componentTypes = componentTypes;
                this.componentOffsets = componentOffsets;
                this.sizeInBytes = sizeInBytes;
                this.spawnFunction = spawnFunction;
                this.destroyFunction = destroyFunction;
            }
        }


        public struct UserNetworkedObject
        {
            public Type[] componentTypes;
            
            public SpawnFunction? spawnFunction;
            
            public DestroyFunction? destroyFunction;

            public UserNetworkedObject(Type[] componentTypes, SpawnFunction? spawnFunction = null, DestroyFunction? destroyFunction = null)
            {
                this.componentTypes = componentTypes;
                this.spawnFunction = spawnFunction;
                this.destroyFunction = destroyFunction;
            }
        }

    }
}
