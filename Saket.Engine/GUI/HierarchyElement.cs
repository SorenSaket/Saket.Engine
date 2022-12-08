using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.UI
{
    internal struct HierarchyElement
    { 
        // Index into world entity
        public int parent;
        public int previous_sibling;
        public int next_sibling;
        public int first_child;
        public int last_child;
    }
}