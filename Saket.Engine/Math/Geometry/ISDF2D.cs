using System.Numerics;

namespace Saket.Engine.Math.Geometry
{
    public interface ISDF2D
    {
        public SignedDistance GetSignedDistance(Vector2 point);
    }
}
