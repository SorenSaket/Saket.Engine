using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Numerics;

namespace Saket.Engine
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Transform
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;


       // private Matrix4x4 _internal;
    }
}
