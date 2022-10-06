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

        public Schema(Type[][] networkedObjects)
        {
            List<NetworkedComponent> components = new();
            List<NetworkedObject> objs = new();
            for (int id_object = 0; id_object < networkedObjects.Length; id_object++)
            {
                // 
                uint[] componentsInObject = new uint[networkedObjects[id_object].Length];
                int[] offsets = new int[networkedObjects[id_object].Length];
                int totalSize = 0; 

                for (int j = 0; j < networkedObjects[id_object].Length; j++)
                {
                    offsets[j] = totalSize;
                    int id_component = components.FindIndex(x => x.type_component == networkedObjects[id_object][j]);

                    if (id_component == -1)
                    {
                        componentsInObject[j] = (uint)components.Count;
                        int size = Marshal.SizeOf(networkedObjects[id_object][j]);
                        totalSize += size;
                        components.Add(new NetworkedComponent((uint)components.Count, networkedObjects[id_object][j], size));
                        
                    }
                    else
                    {
                        componentsInObject[j] = (uint)id_component;
                        totalSize += components[id_component].sizeInBytes;
                    }
                }

                objs.Add(new NetworkedObject((uint)id_object, componentsInObject, offsets, totalSize));
            }
            networkedComponents = components.ToArray();
            this.networkedObjects = objs.ToArray();
        }





        public struct NetworkedComponent
        {
            public uint id_component;
            public Type type_component;
            public int sizeInBytes;

            public NetworkedComponent(uint id_component, Type type_component, int sizeInBytes)
            {
                this.id_component = id_component;
                this.type_component = type_component;
                this.sizeInBytes = sizeInBytes;
            }
        }

        public struct NetworkedObject
        {
            public uint id_object;
            public uint[] componentTypes;
            public int[] componentOffsets;
            public int sizeInBytes;

            public NetworkedObject(uint id_object, uint[] componentTypes, int[] componentOffsets, int sizeInBytes)
            {
                this.id_object = id_object;
                this.componentTypes = componentTypes;
                this.componentOffsets = componentOffsets;
                this.sizeInBytes = sizeInBytes;
            }
        }
    }
}
