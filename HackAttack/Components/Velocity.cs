using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HackAttack.Components;

[StructLayout(LayoutKind.Sequential)]
public struct Velocity
{
    public Vector2 Value;
    public float X { get => Value.X; set { Value.X = value; } }
    public float Y { get => Value.Y; set { Value.Y = value; } }

    public Velocity(Vector2 v)
    {
        Value = v;
    }
    public Velocity(float x, float y)
    {
        this.X = x;
        this.Y = y;
    }

    public static implicit operator Velocity(Vector2 v) => new Velocity(v.X, v.Y);
}