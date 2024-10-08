﻿using Saket.Engine.Types;
using System.Numerics;

namespace Saket.Engine.GeometryD2;

public interface ISDF2D
{
    public SignedDistance GetSignedDistance(Vector2 point);
    public bool IsWithin(Vector2 point) => GetSignedDistance(point).Distance <= 0;
}
