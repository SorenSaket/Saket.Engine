using Saket.Engine;
using System.Numerics;

namespace HackAttack.Components;

public struct Collider2DBox
{
    public Vector2 Size;
    public byte flags;

    public bool IsTrigger => flags == 1;

    public Collider2DBox(Vector2 size, bool isTrigger = false)
    {
        Size = size;
        if (isTrigger)
            flags = 1;
        else
            flags = 0;
    }

    public static bool IntersectsWith(Collider2DBox selfBox, Transform2D selfTransform,
        Collider2DBox otherBox, Transform2D otherTransform)
    {
        return (MathF.Abs(selfTransform.Position.X - otherTransform.Position.X) < (selfBox.Size.X / 2f + otherBox.Size.X / 2f)) &&
                (MathF.Abs(selfTransform.Position.Y - otherTransform.Position.Y) < (selfBox.Size.Y / 2f + otherBox.Size.Y / 2f));
    }
}
