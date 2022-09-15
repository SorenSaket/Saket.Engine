using Saket.ECS.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Components
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Active
    {
        public bool active;
    }
}