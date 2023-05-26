using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Math.Geometry.Shapes
{
    public struct Rectangle
    {/*
        float sdOrientedBox(in Vector2 p, in Vector2 a, in Vector2 b, float th)
        {
            float l = length(b - a);
            Vector2 d = (b - a) / l;
            Vector2 q = (p - (a + b) * 0.5);
            q = mat2(d.x, -d.y, d.y, d.x) * q;
            q = abs(q) - vec2(l, th) * 0.5;
            return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0);
        }*/

    }
}
