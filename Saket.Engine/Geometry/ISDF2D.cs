using Saket.Engine.Types;
using System.Numerics;

namespace Saket.Engine.Geometry;

public interface ISDF2D
{
    public SignedDistance GetSignedDistance(Vector2 point);
}
