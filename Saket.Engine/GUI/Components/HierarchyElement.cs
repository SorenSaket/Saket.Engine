using Saket.ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Saket.Engine.GUI
{
    /// <summary>
    /// Contains indexes for children and parents.
    /// </summary>
    public struct HierarchyElement
    {
        public int parent;
        public int previous_sibling;
        public int next_sibling;
        public int first_child;
        public int last_child;
    }

    /// <summary>
    /// Contains indexes for children and parents.
    /// </summary>
    public struct HierarchyEntity
    {
        public ECSPointer parent;
        public ECSPointer previous_sibling;
        public ECSPointer next_sibling;
        public ECSPointer first_child;
        public ECSPointer last_child;

        public HierarchyEntity(ECSPointer parent)
        {
            this.parent = parent;
            this.previous_sibling = default;
            this.next_sibling = default;
            this.first_child = default;
            this.last_child = default;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public struct HierarchyEntityTraverseIterator : IEnumerator, IEnumerator<Entity>
    {
        object IEnumerator.Current => current;
        Entity IEnumerator<Entity>.Current => current;

        private Entity current;
        private HierarchyEntity currentHierarchy;
        
        private Entity root;
        
        private World world;

        private Stack<ECSPointer> prev;

        public HierarchyEntityTraverseIterator(World world, Entity root, Stack<ECSPointer> prev)
        {
            this.root = current = root;
            this.world = world;
            this.prev = prev;
            prev.Clear();
        }

        public bool MoveNext()
        {
            // Add all children to stack


            // pop from stack
            /*

            if(currentHierarchy.first_child)
            {

            }

            if(currentHierarchy.next_sibling == default)
            {
                return false;
            }

            current = current.next_sibling;*/
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}