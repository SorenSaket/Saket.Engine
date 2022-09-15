using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TechWars.Shared
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Player
    {
        public PlayerInput input;
    }
}
