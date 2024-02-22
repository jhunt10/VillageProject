namespace VillageProject.Core.Enums;

/// <summary>
/// Bitwise flags for relative directions
/// The basic 6 direction values (Top, Bottom, Back, Front, Left, and Right) can be concatenated together to represent non-cardinal directions.
/// For example: Back | Top = 1 + 16 = 17 or TopBack which means the cell above and behind.
/// Combining opposite directions like Back | Front has no valid meaning.
/// </summary>
[Flags]
public enum DirectionFlags
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