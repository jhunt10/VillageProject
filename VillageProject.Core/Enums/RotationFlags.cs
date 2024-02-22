namespace VillageProject.Core.Enums;

/// <summary>
/// Bitwise flags for cardinal directions
/// </summary>
public enum RotationFlag
{
    North = 0,
    East = 1,
    South = 2,
    West = 3
}

public enum RotationDirection
{
    None = 0,
    Clockwise = 1,
    HalfTurn = 2,
    CounterClockwise = 3,
}