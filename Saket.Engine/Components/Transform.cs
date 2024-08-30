using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Numerics;

namespace Saket.Engine
{
    [StructLayout(LayoutKind.Explicit, Size = 128)]
    public struct Transform
    {
        [FieldOffset(0)]
        public Vector3 Position;
        [FieldOffset(48)]
        public Quaternion Rotation;
        [FieldOffset(96)]
        public Vector3 Scale;




        public Transform()
        {
            Position = new Vector3();
            Scale = Vector3.One;
            Rotation = Quaternion.Identity;
        }

        public Transform(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public readonly Matrix4x4 TRS()
        {
            return Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateTranslation(Position);
        }
    }
}
