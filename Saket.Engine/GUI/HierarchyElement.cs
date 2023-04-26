using Saket.ECS;

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
    }
}