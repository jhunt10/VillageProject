namespace VillageProject.Core.Enums;

/// <summary>
/// Bitwise enum flags for representing adjacency.
/// Values can be concatenated together produce one value representing an array of bits for if adjacent cells match.
/// For example: TopLeft | BackLeft =  4096 + 1 = 4097 which means a condition is true for the Top Left and Back Left cells.
/// Two flags can not be joined to create another flag. ie. Back | Left != BackLeft
/// </summary>
[Flags]
public enum AdjacencyFlags
{
    None = 0,
    BackLeft = 1,
    Back = 2,
    BackRight = 4,
    Left = 8,
    Right = 16,
    FrontLeft = 32,
    Front = 64,
    FrontRight = 128,
    Top = 256,
    TopBackLeft = 512,
    TopBack = 1024,
    TopBackRight = 2048,
    TopLeft = 4096,
    TopRight = 8192,
    TopFrontLeft = 16384,
    TopFront = 32768,
    TopFrontRight = 65536,
    Bottom = 131072,
    BottomBackLeft = 262144,
    BottomBack = 524288,
    BottomBackRight = 1048576,
    BottomLeft = 2097152,
    BottomRight = 4194304,
    BottomFrontLeft = 8388608,
    BottomFront = 16777216,
    BottomFrontRight = 33554432,
}

public static class AdjacencyFlagExtensions
{
    public static bool HasFlag(this AdjacencyFlags flag, AdjacencyFlags check)
    {
        return ((flag & check) == check);
    }
}