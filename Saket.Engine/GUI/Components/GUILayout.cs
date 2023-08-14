using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.GUI
{
    /// <summary>
    /// This contains only data about the box and doesn't include outerspacing nor innerspacing
    /// </summary>
    public struct GUILayout
    {
        public float x;
        
        public float y;
        /// <summary>
        /// Depth factor
        /// </summary>
        public float z;

        public float w;
        public float h;
    }
}
