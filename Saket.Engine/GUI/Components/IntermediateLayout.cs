using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.GUI.Components
{
    public struct IntermediateLayout
    {
        public float preferredWidth;
        public float preferredHeight;
        public Constraints constaints;
        public bool Dirty = true;

        public IntermediateLayout()
        {

        }
    }

    public struct Constraints
    {
        public float minWidth;
        public float maxWidth;
        public float minHeight;
        public float maxHeight;
    }
}