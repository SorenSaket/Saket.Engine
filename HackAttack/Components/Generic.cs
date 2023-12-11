using Saket.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HackAttack.Components
{
    public struct RotateTowardsTarget
    {
        public ECSPointer target;
        public float rotationSpeed;
    }

    public struct MoveTowardsTarget
    {
        public ECSPointer target;
        public float moveSpeed;
        public Vector3 offset;
    }



    [System.Runtime.CompilerServices.InlineArray(10)]
    public struct StatStore
    {
        private float _element0;
    }

    struct StatsBearer
    {
        StatStore store;
        
        void SAd()
        {
            store[2] = 0;
        }
    }
}