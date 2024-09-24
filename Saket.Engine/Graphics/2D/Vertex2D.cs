using System.Numerics;

namespace Saket.Engine.Graphics._2D;

public struct Vertex2D
{
    public Vector2 pos;
    public Vector2 uv;
    public uint col;

    public Vertex2D(Vector2 pos, Vector2 uv, uint col = uint.MaxValue)
    {
        this.pos = pos;
        this.uv = uv;
        this.col = col;
    }
}