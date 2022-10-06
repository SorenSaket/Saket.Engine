using Saket.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saket.Engine.Net.Snapshotting;

namespace Saket.Engine.Net.Rollback
{
    // Rollback
    // 
    // 
    // There is no reason to do "full" rollback where the entire state of the world 
    //
    //    
    // 

    // -------- Full Rollback for --------
    // Full rollback allows to rollback entire ecs state. Ensures that all state is 
    // 
    //
    //


    // ---- Partial Rollback for lag compensation ----
    // Only relevant objects are rolled back on the server, and are not "rolled forward again". The server simply contiues where it left off.
    // This means it can only be used to perform checks on previous gamestate for hit detection etc. and the changes will not have cascading effects.
    // 
    // This ensures minimal cpu usage.
    // 

    // "It is only nessary to store state data for rollback for networked objects"
    // Is above true? think of edge cases 
    //

    // TODO: Delta compression to reduce memory consumption
    public struct GameState
    {
        public List<Archetype> archetypes;

        public GameState()
        {
            this.archetypes = new();
        }
    }

    public class Service_Rollback
    {



        public readonly int rollbackDistance;
        public GameState[] networkedGameStates;

        private int head;

        public Service_Rollback(int rollbackDistance)
        {
            this.rollbackDistance = rollbackDistance;
            networkedGameStates = new GameState[rollbackDistance];
            head = 0;
        }

        public void StoreNetworkedGameState(World world)
        {
            // Iterate over all archetypes
            foreach (var archetype in world.archetypes)
            {
                // Only handle archetypes with NetworkedEntities
                if (!archetype.Has<NetworkedEntity>())
                    continue;

                // -- Store networked game state --

                // If equvilent archetype does not exists in storage create one
                // FirstOrDefault compares hash of components
                if (!networkedGameStates[head].archetypes.FirstOrDefault(archetype).Unwrap(out var storedArchetype))
                {
                    storedArchetype = new Archetype(archetype.ComponentTypes);
                    networkedGameStates[head].archetypes.Add(storedArchetype);
                }

                // 
                archetype.Overwrite(storedArchetype);
            }
            // Advance head
            head = (head+1)%rollbackDistance;
        }
    }
}
