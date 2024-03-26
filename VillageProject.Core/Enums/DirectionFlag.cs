namespace VillageProject.Core.Enums;

/// <summary>
/// Non-Aggregable bitwise flags for relative directions
/// The basic 6 direction values (Top, Bottom, Back, Front, Left, and Right) can be concatenated together to represent non-cardinal directions.
/// For example: Back | Top = 1 + 16 = 17 or TopBack which means the cell above and behind.
/// Combining opposite directions like Back | Front has no valid meaning.
/// </summary>
[Flags]
public enum DirectionFlag
{
    None = 0,
    Back = 1,
    Right = 2,
    Front = 4,
    Left = 8,
    Top = 16,
    Bottom = 32,
    
    BackRight = Back | Right,
    BackLeft = Back | Left,
    TopBack = Top | Back,
    BottomBack = Bottom | Back,
    TopBackRight = Top | Back | Right,
    TopBackLeft = Top | Back | Left,
    BottomBackRight = Bottom | Back | Right,
    BottomBackLeft = Bottom | Back | Left,

    FrontRight = Front | Right,
    FrontLeft = Front | Left,
    TopFront = Top | Front,
    BottomFront = Bottom | Front,
    TopFrontRight = Top | Front | Right,
    TopFrontLeft = Top | Front | Left,
    BottomFrontRight = Bottom | Front | Right,
    BottomFrontLeft = Bottom | Front | Left,

    TopRight = Top | Right,
    TopLeft = Top | Left,
    BottomRight = Bottom | Right,
    BottomLeft = Bottom | Left,
}

public static class DirectionFlagExtensions
{
    public static CellSideFlags ToCellSide(this DirectionFlag dir)
    {
        switch (dir)
        {
            case DirectionFlag.None:
                return CellSideFlags.None;
            case DirectionFlag.BackLeft:
                return CellSideFlags.BackLeft;
            case DirectionFlag.Back:
                return CellSideFlags.Back;
            case DirectionFlag.BackRight:
                return CellSideFlags.BackRight;
            case DirectionFlag.Left:
                return CellSideFlags.Left;
            case DirectionFlag.Right:
                return CellSideFlags.Right;
            case DirectionFlag.FrontLeft:
                return CellSideFlags.FrontLeft;
            case DirectionFlag.Front:
                return CellSideFlags.Front;
            case DirectionFlag.FrontRight:
                return CellSideFlags.FrontRight;
            case DirectionFlag.Top:
                return CellSideFlags.Top;
            case DirectionFlag.TopBackLeft:
                return CellSideFlags.TopBackLeft;
            case DirectionFlag.TopBack:
                return CellSideFlags.TopBack;
            case DirectionFlag.TopBackRight:
                return CellSideFlags.TopBackRight;
            case DirectionFlag.TopLeft:
                return CellSideFlags.TopLeft;
            case DirectionFlag.TopRight:
                return CellSideFlags.TopRight;
            case DirectionFlag.TopFrontLeft:
                return CellSideFlags.TopFrontLeft;
            case DirectionFlag.TopFront:
                return CellSideFlags.TopFront;
            case DirectionFlag.TopFrontRight:
                return CellSideFlags.TopFrontRight;
            case DirectionFlag.Bottom:
                return CellSideFlags.Bottom;
            case DirectionFlag.BottomBackLeft:
                return CellSideFlags.BottomBackLeft;
            case DirectionFlag.BottomBack:
                return CellSideFlags.BottomBack;
            case DirectionFlag.BottomBackRight:
                return CellSideFlags.BottomBackRight;
            case DirectionFlag.BottomLeft:
                return CellSideFlags.BottomLeft;
            case DirectionFlag.BottomRight:
                return CellSideFlags.BottomRight;
            case DirectionFlag.BottomFrontLeft:
                return CellSideFlags.BottomFrontLeft;
            case DirectionFlag.BottomFront:
                return CellSideFlags.BottomFront;
            case DirectionFlag.BottomFrontRight:
                return CellSideFlags.BottomFrontRight;

        }

        throw new Exception($"Unrecognized Direction Flag: {dir}");
    }

    /// <summary>
    /// Get the DirectionFlag that would result from turning this DirectionFlag in the provided RotationDirection.
    /// </summary>
    /// <param name="dir">Current DirectionFlag</param>
    /// <param name="turn">Direction to turn</param>
    /// <returns>Rotated Occupation</returns>
    public static DirectionFlag Rotate(this DirectionFlag dir, RotationDirection turn)
    {
        if (dir == DirectionFlag.None || dir == DirectionFlag.Bottom || dir == DirectionFlag.Top)
            return dir;
        
        switch (turn)
        {
            case RotationDirection.None:
                return dir;
            case RotationDirection.Clockwise:
                switch (dir)
                {
                    case DirectionFlag.Back:
                        return DirectionFlag.Right;
                    case DirectionFlag.Right:
                        return DirectionFlag.Front;
                    case DirectionFlag.Front:
                        return DirectionFlag.Left;
                    case DirectionFlag.Left:
                        return DirectionFlag.Back;

                    case DirectionFlag.BackRight:
                        return DirectionFlag.FrontRight;
                    case DirectionFlag.FrontRight:
                        return DirectionFlag.FrontLeft;
                    case DirectionFlag.FrontLeft:
                        return DirectionFlag.BackLeft;
                    case DirectionFlag.BackLeft:
                        return DirectionFlag.BackRight;

                    case DirectionFlag.TopBack:
                        return DirectionFlag.TopRight;
                    case DirectionFlag.TopRight:
                        return DirectionFlag.TopFront;
                    case DirectionFlag.TopFront:
                        return DirectionFlag.TopLeft;
                    case DirectionFlag.TopLeft:
                        return DirectionFlag.TopBack;

                    case DirectionFlag.TopBackRight:
                        return DirectionFlag.TopFrontRight;
                    case DirectionFlag.TopFrontRight:
                        return DirectionFlag.TopFrontLeft;
                    case DirectionFlag.TopFrontLeft:
                        return DirectionFlag.TopBackLeft;
                    case DirectionFlag.TopBackLeft:
                        return DirectionFlag.TopBackRight;

                    case DirectionFlag.BottomBack:
                        return DirectionFlag.BottomRight;
                    case DirectionFlag.BottomRight:
                        return DirectionFlag.BottomFront;
                    case DirectionFlag.BottomFront:
                        return DirectionFlag.BottomLeft;
                    case DirectionFlag.BottomLeft:
                        return DirectionFlag.BottomBack;

                    case DirectionFlag.BottomBackRight:
                        return DirectionFlag.BottomFrontRight;
                    case DirectionFlag.BottomFrontRight:
                        return DirectionFlag.BottomFrontLeft;
                    case DirectionFlag.BottomFrontLeft:
                        return DirectionFlag.BottomBackLeft;
                    case DirectionFlag.BottomBackLeft:
                        return DirectionFlag.BottomBackRight;
                }
                break;
            case RotationDirection.HalfTurn:
                switch (dir)
                {
                    case DirectionFlag.Back:
                        return DirectionFlag.Front;
                    case DirectionFlag.Right:
                        return DirectionFlag.Left;
                    case DirectionFlag.Front:
                        return DirectionFlag.Back;
                    case DirectionFlag.Left:
                        return DirectionFlag.Right;
    
                    case DirectionFlag.BackRight:
                        return DirectionFlag.FrontLeft;
                    case DirectionFlag.FrontRight:
                        return DirectionFlag.BackLeft;
                    case DirectionFlag.FrontLeft:
                        return DirectionFlag.BackRight;
                    case DirectionFlag.BackLeft:
                        return DirectionFlag.FrontRight;
    
                    case DirectionFlag.TopBack:
                        return DirectionFlag.TopFront;
                    case DirectionFlag.TopRight:
                        return DirectionFlag.TopLeft;
                    case DirectionFlag.TopFront:
                        return DirectionFlag.TopBack;
                    case DirectionFlag.TopLeft:
                        return DirectionFlag.TopRight;
    
                    case DirectionFlag.TopBackRight:
                        return DirectionFlag.TopFrontLeft;
                    case DirectionFlag.TopFrontRight:
                        return DirectionFlag.TopBackLeft;
                    case DirectionFlag.TopFrontLeft:
                        return DirectionFlag.TopBackRight;
                    case DirectionFlag.TopBackLeft:
                        return DirectionFlag.TopFrontRight;
    
                    case DirectionFlag.BottomBack:
                        return DirectionFlag.BottomFront;
                    case DirectionFlag.BottomRight:
                        return DirectionFlag.BottomLeft;
                    case DirectionFlag.BottomFront:
                        return DirectionFlag.BottomBack;
                    case DirectionFlag.BottomLeft:
                        return DirectionFlag.BottomRight;
    
                    case DirectionFlag.BottomBackRight:
                        return DirectionFlag.BottomFrontLeft;
                    case DirectionFlag.BottomFrontRight:
                        return DirectionFlag.BottomBackLeft;
                    case DirectionFlag.BottomFrontLeft:
                        return DirectionFlag.BottomBackRight;
                    case DirectionFlag.BottomBackLeft:
                        return DirectionFlag.BottomFrontRight;   
                }
                break;
            case RotationDirection.CounterClockwise:
                switch (dir)
                {
                    case DirectionFlag.Back:
                        return DirectionFlag.Left;
                    case DirectionFlag.Right:
                        return DirectionFlag.Back;
                    case DirectionFlag.Front:
                        return DirectionFlag.Right;
                    case DirectionFlag.Left:
                        return DirectionFlag.Front;
    
                    case DirectionFlag.BackRight:
                        return DirectionFlag.BackLeft;
                    case DirectionFlag.FrontRight:
                        return DirectionFlag.BackRight;
                    case DirectionFlag.FrontLeft:
                        return DirectionFlag.FrontRight;
                    case DirectionFlag.BackLeft:
                        return DirectionFlag.FrontLeft;
    
                    case DirectionFlag.TopBack:
                        return DirectionFlag.TopLeft;
                    case DirectionFlag.TopRight:
                        return DirectionFlag.TopBack;
                    case DirectionFlag.TopFront:
                        return DirectionFlag.TopRight;
                    case DirectionFlag.TopLeft:
                        return DirectionFlag.TopFront;
    
                    case DirectionFlag.TopBackRight:
                        return DirectionFlag.TopBackLeft;
                    case DirectionFlag.TopFrontRight:
                        return DirectionFlag.TopBackRight;
                    case DirectionFlag.TopFrontLeft:
                        return DirectionFlag.TopFrontRight;
                    case DirectionFlag.TopBackLeft:
                        return DirectionFlag.TopFrontLeft;
    
                    case DirectionFlag.BottomBack:
                        return DirectionFlag.BottomLeft;
                    case DirectionFlag.BottomRight:
                        return DirectionFlag.BottomBack;
                    case DirectionFlag.BottomFront:
                        return DirectionFlag.BottomRight;
                    case DirectionFlag.BottomLeft:
                        return DirectionFlag.BottomFront;
    
                    case DirectionFlag.BottomBackRight:
                        return DirectionFlag.BottomBackLeft;
                    case DirectionFlag.BottomFrontRight:
                        return DirectionFlag.BottomBackRight;
                    case DirectionFlag.BottomFrontLeft:
                        return DirectionFlag.BottomFrontRight;
                    case DirectionFlag.BottomBackLeft:
                        return DirectionFlag.BottomFrontLeft;
                }
                break;
        }

        return dir;
    }
}