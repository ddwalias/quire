using Quire.Mirror;

namespace Quire;

public static partial class DirectionOps
{
    [MirrorVerified("Release IL")]
    public static Direction Opposite(Direction dir)
    {
        switch (dir)
        {
            case Direction.Right:
                return Direction.Left;
            case Direction.Left:
                return Direction.Right;
            case Direction.Up:
                return Direction.Down;
            case Direction.Down:
                return Direction.Up;
            default:
                return Direction.Indeterminate;
        }
    }

    [MirrorVerified("Release IL")]
    public static bool IsHorizontal(Direction dir)
    {
        if (dir != Direction.Left)
        {
            return dir == Direction.Right;
        }
        return true;
    }

    [MirrorVerified("Release IL")]
    public static bool IsVertical(Direction dir)
    {
        if (dir != Direction.Up)
        {
            return dir == Direction.Down;
        }
        return true;
    }
}
