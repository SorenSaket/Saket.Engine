using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Types;


[Flags]
public enum Direction : byte
{
    Undefined = 0,

    E = 1 << 0,
    SE = 1 << 1,
    S = 1 << 2,
    SW = 1 << 3,
    W = 1 << 4,
    NW = 1 << 5,
    N = 1 << 6,
    NE = 1 << 7,

    // Aliases
    Right = E,
    Down = S,
    Left = W,
    Up = N,
    Bottom = Down,
    Top = Up,
    Cardial = E | S | W | N,
    Intercardinal = SE | SW | NW | NE,
    All = Cardial | Intercardinal,
}


public static class Extenstions_Direction
{    /// <summary>
     /// Returns the next clockwise direction from the given direction.
     /// </summary>
    public static Direction GetNextClockwiseDirection(this Direction direction, int n = 1)
    {
        if (direction == Direction.Undefined)
            return Direction.Undefined;

        int dirValue = (int)direction;

        // Ensure the direction is a single value (not a combination)
        if (!IsPowerOfTwo(dirValue))
            throw new ArgumentException("Direction must be a single cardinal direction.");

        int index = (int)Math.Log(dirValue, 2);

        // Next index, modulo 8 to wrap around
        int nextIndex = Extensions_Math.Mod(index + n, 8);

        // Get the direction from the index
        Direction nextDirection = IndexToDirectionCW(nextIndex);

        return nextDirection;
    }
    /// <summary>
    /// Converts a Direction to radians.
    /// </summary>
    public static float DirectionToRadians(this Direction direction)
    {
        if (direction == Direction.Undefined)
            return 0f;

        int dirValue = (int)direction;

        if (!IsPowerOfTwo(dirValue))
            throw new ArgumentException("Direction must be a single cardinal direction.");

        int index = (int)Math.Log(dirValue, 2);

        // Angles increase clockwise from East (0 radians)
        float angle = index * (float)(Math.PI / 4);

        return angle;
    }

    /// <summary>
    /// Converts a Direction to degrees.
    /// </summary>
    public static float DirectionToDegrees(this Direction direction)
    {
        float radians = DirectionToRadians(direction);
        float degrees = radians * (180f / (float)Math.PI);
        return degrees;
    }

    /// <summary>
    /// Converts a Direction to turns (full rotations).
    /// </summary>
    public static float DirectionToTurns(this Direction direction)
    {
        float radians = DirectionToRadians(direction);
        float turns = radians / (2f * (float)Math.PI);
        return turns;
    }

    /// <summary>
    /// Converts a Direction to a unit Vector2.
    /// </summary>
    public static Vector2 DirectionToVector2(this Direction direction)
    {
        float radians = DirectionToRadians(direction);
        float x = (float)Math.Cos(radians);
        float y = (float)Math.Sin(radians);
        return new Vector2(x, y);
    }

    /// <summary>
    /// Converts degrees to the closest Direction.
    /// </summary>
    public static Direction DegreesToDirection(this float degrees)
    {
        degrees = NormalizeAngle(degrees);
        int index = (int)Math.Round(degrees / 45f) % 8;
        return IndexToDirectionCCW(index);
    }

    /// <summary>
    /// Converts turns to the closest Direction.
    /// </summary>
    public static Direction TurnsToDirection(this float turns)
    {
        float degrees = turns * 360f;
        return DegreesToDirection(degrees);
    }

    /// <summary>
    /// Converts a Vector2 to the closest Direction.
    /// </summary>
    public static Direction ToDirection(this Vector2 vector)
    {
        float radians = (float)Math.Atan2(vector.Y, vector.X);
        float degrees = radians * (180f / (float)Math.PI);
        degrees = NormalizeAngle(degrees);
        return DegreesToDirection(degrees);
    }




    private static Direction IndexToDirectionCCW(int index)
    {
        switch (index)
        {

            case 0: return Direction.E;
            case 1: return Direction.NE;
            case 2: return Direction.N;
            case 3: return Direction.NW;
            case 4: return Direction.W;
            case 5: return Direction.SW;
            case 6: return Direction.S;
            case 7: return Direction.SE;
            
            default: return Direction.Undefined;
        }
    }
    private static Direction IndexToDirectionCW(int index)
    {
        switch (index)
        {

            case 0: return Direction.E;
            case 1: return Direction.SE;
            case 2: return Direction.S;
            case 3: return Direction.SW;
            case 4: return Direction.W;
            case 5: return Direction.NW;
            case 6: return Direction.N;
            case 7: return Direction.NE;

            default: return Direction.Undefined;
        }
    }
    private static float NormalizeAngle(float degrees)
    {
        degrees = degrees % 360f;
        if (degrees < 0)
            degrees += 360f;
        return degrees;
    }

    private static bool IsPowerOfTwo(int x)
    {
        return (x != 0) && ((x & (x - 1)) == 0);
    }
}
