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

public static class RotationExtensions
{
    /// <summary>
    /// Turn this rotation in the provided direction.
    /// </summary>
    /// <param name="rotation">Current Rotation</param>
    /// <param name="direction">Direction to turn</param>
    /// <returns>Resulting Rotation</returns>
    public static RotationFlag AddRotation(this RotationFlag rotation, RotationFlag addRotation)
    {
        return (RotationFlag)(((int)rotation + (int)addRotation) % 4); 
    }
    public static RotationFlag SubtractRotation(this RotationFlag rotation, RotationFlag addRotation)
    {
        return (RotationFlag)(((int)rotation - (int)addRotation + 4) % 4); 
    }
    
    
    /// <summary>
    /// Turn this rotation in the provided direction.
    /// </summary>
    /// <param name="rotation">Current Rotation</param>
    /// <param name="direction">Direction to turn</param>
    /// <returns>Resulting Rotation</returns>
    public static RotationFlag ApplyRotationDirection(this RotationFlag rotation, RotationDirection direction)
    {
        return (RotationFlag)(((int)rotation + (int)direction) % 4); 
    }
    
    /// <summary>
    /// Get the direction this rotation would have to be turned to match the provided rotation.
    /// </summary>
    /// <param name="rotation">Current Rotation</param>
    /// <param name="targetRotation">Rotation to match</param>
    /// <returns>Direction to turn</returns>
    public static RotationDirection GetRotationDirection(this RotationFlag rotation, RotationFlag targetRotation)
    {
        return (RotationDirection)(((int)targetRotation - (int)rotation + 4) % 4);
    }
}