using System;
using System.Collections.Generic;
using System.Numerics;

namespace Saket.Engine.Typography.TrueType
{
    struct FUnit {
        int value;

        public static explicit operator int (FUnit v) => v.value;
        public static explicit operator FUnit (int v) => new FUnit { value = v };

        public static FUnit operator -(FUnit lhs, FUnit rhs) => (FUnit)(lhs.value - rhs.value);
        public static FUnit operator +(FUnit lhs, FUnit rhs) => (FUnit)(lhs.value + rhs.value);
        public static float operator *(FUnit lhs, float rhs) => lhs.value * rhs;

        public static FUnit Max (FUnit a, FUnit b) => (FUnit)System.Math.Max(a.value, b.value);
        public static FUnit Min (FUnit a, FUnit b) => (FUnit)System.Math.Min(a.value, b.value);
    }

    struct Point {
        public FUnit X;
        public FUnit Y;
        public PointType Type;

        public Point (FUnit x, FUnit y) {
            X = x;
            Y = y;
            Type = PointType.OnCurve;
        }

        public static PointF operator *(Point lhs, float rhs) => new PointF(new Vector2(lhs.X * rhs, lhs.Y * rhs), lhs.Type);

        public static explicit operator Vector2 (Point p) => new Vector2((int)p.X, (int)p.Y);
    }

    struct PointF {
        public Vector2 P;
        public PointType Type;

        public PointF (Vector2 position, PointType type) {
            P = position;
            Type = type;
        }

        public PointF Offset (Vector2 offset) => new PointF(P + offset, Type);

        public override string ToString () => $"{P} ({Type})";

        public static implicit operator Vector2 (PointF p) => p.P;
    }

    enum PointType {
        OnCurve,
        Quadratic,
        Cubic
    }

}