using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.GUI
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Widget
    {
        public int id; // Index into string array
        public int classGroup;

        public Widget(int id = -1, int classes = -1)
        {
            this.id = id;
            this.classGroup = classes;
        }
    }
}
