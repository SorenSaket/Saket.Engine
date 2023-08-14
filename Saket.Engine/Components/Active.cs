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
        public bool IsActive { get => active == 1; set { active = (byte) (value ? 0b_1 : 0b_0); } }

        private byte active;
    }
}