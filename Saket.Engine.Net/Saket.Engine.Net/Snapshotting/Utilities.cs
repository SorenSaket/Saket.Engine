using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Net.Snapshotting
{
    public static class Utilities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SnapshotContainsEntity(Snapshot_B snapshot, int id_NetworkEntity)
        {
            // If the delta doesn't contain item in update then the networkentity is new
            if (snapshot.Updates.Any(x => x.id_network == id_NetworkEntity))
            {
                return true;
            }
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool IsInGroup(int* groups, int target, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (groups[i] == target)
                    return true;
            }
            return false;
        }

    }
}
