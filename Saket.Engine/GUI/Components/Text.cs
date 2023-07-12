using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.GUI
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Text
    {
        public int textIndex;

        public Text(int textIndex)
        {
            this.textIndex = textIndex;
        }
    }
}
