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
    public struct HierarchyEntity : IEquatable<HierarchyEntity>
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

        public override bool Equals(object? obj)
        {
            return obj is HierarchyEntity entity && Equals(entity);
        }

        public bool Equals(HierarchyEntity other)
        {
            return EqualityComparer<ECSPointer>.Default.Equals(parent, other.parent) &&
                   EqualityComparer<ECSPointer>.Default.Equals(previous_sibling, other.previous_sibling) &&
                   EqualityComparer<ECSPointer>.Default.Equals(next_sibling, other.next_sibling) &&
                   EqualityComparer<ECSPointer>.Default.Equals(first_child, other.first_child) &&
                   EqualityComparer<ECSPointer>.Default.Equals(last_child, other.last_child);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(parent, previous_sibling, next_sibling, first_child, last_child);
        }

        public static bool operator ==(HierarchyEntity left, HierarchyEntity right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HierarchyEntity left, HierarchyEntity right)
        {
            return !(left == right);
        }

    }


   

    /// <summary>
    /// 
    /// </summary>
    public struct HierarchyEntityChildEnumerator : IEnumerator<Entity>
    {
        public Entity Current => new Entity(world, currentPointer);
        object IEnumerator.Current => Current;
        public HierarchyEntity CurrentHierarchyEntity => current;

        private ECSPointer currentPointer;
        private HierarchyEntity current;

        private HierarchyEntity root;

        private World world;

        public HierarchyEntityChildEnumerator(Entity root)
        {
            this.root = current = root.Get<HierarchyEntity>();
            this.world = root.World;
        }

        public bool MoveNext()
        {
            // If this is the first iteration
            if(current == root)
            {
                // If the root has a first child
                if(root.first_child != default)
                {
                    currentPointer = root.first_child;
                    current = new Entity(world, root.first_child).Get<HierarchyEntity>();
                    return true;
                }
            }
            else
            {
                // If the child and a sibling
                if (current.next_sibling != default)
                {
                    currentPointer = current.next_sibling;
                    current = new Entity(world, current.next_sibling).Get<HierarchyEntity>();
                    return true;
                }
            }
            return false;
        }

        public void Reset()
        {
            current = root;
        }

        public void Dispose()
        {
            world = null;
            current = root = default;
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