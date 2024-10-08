﻿using Saket.ECS;
using System;
using Saket.Serialization;
using Saket.Serialization.Byte;

namespace Saket.Engine.Net.Snapshotting.A
{
    public static class Snapshot_A_Server
    {
        public static void WriteSnapShot(
            IWriter writer,
            World state_base,
            Schema schema,
            out int sizeInBytes,
            int group = 0)
        {
            int startingOffset = writer.Position;

            uint numberOfEntities = 0;

            // Allocate space on the writer and correct later
            writer.Write(numberOfEntities);

            // ---- Populate snapshot with new data ----

            // Iterate over all archetypes to find networked objects
            foreach (var archetype in state_base.Archetypes)
            {
                // Only handle archetypes with NetworkedEntities
                if (!archetype.Has<NetworkedEntity>())
                    continue;

                // For each entity in archetype
                foreach (var row in archetype)
                {
                    // Get the entity
                    NetworkedEntity networkedEntity = archetype.Get<NetworkedEntity>(row);
                    IDNet id_networkedEntity = networkedEntity.id_network;
                    ushort id_objectType = networkedEntity.id_objectType;

                    if(!schema.networkedObjects.FirstOrFalse(x => x.id_object == id_objectType, out var schema_object))
                    {
                        throw new Exception($"Schema doesn't exist for object type with id_type: {id_objectType}");
                    }

                    // Only add component if they're in the same interestGroup
                    // check if object left the group and call destroy for entity
                   /* unsafe
                    {
                        if (!Utilities.IsInGroup(networkedEntity.interestGroups, group, 16))
                            continue;
                    }*/

                    numberOfEntities++;
                    writer.Write(id_networkedEntity);
                    writer.Write(id_objectType);

                    // For each component/storage on archetype
                    int basePosition = writer.Position;

                    // Iterate the storages non ordered
                    // 
                    foreach (var store in archetype.storage)
                    {
                        // Only handle registered networked components
                        if (!schema.networkedComponents.FirstOrFalse(x => x.type_component == store.Key, out var schema_component))
                        {
                            continue;
                        }

                        // Get the component index on the object
                        if (!schema_object.componentTypes.FirstOrFalse(x => x == schema_component.id_component, out var index_component))
                        {
                            throw new Exception($"Component of type {store.Key} doesn't exist for on object schema with id_type: {id_objectType}");
                        }

                        writer.Position = basePosition + schema_object.componentOffsets[index_component];

                        unsafe
                        {
                            writer.Write(store.Value.Get(row), store.Value.ItemSizeInBytes);
                        }
                    }
                    // Restore writer position for next object
                    writer.Position = basePosition + schema_object.sizeInBytes;
                }
            }

            var endPosition = writer.Position;

            // Overwrite the wrong numberOfEntities
            writer.Position = startingOffset;
            writer.Write(numberOfEntities);

            // Restore writer position to continue
            writer.Position = endPosition;
            sizeInBytes = endPosition - startingOffset;
        }

    }
}