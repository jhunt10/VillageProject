using System.Collections;

namespace VillageProject.Core.Enums;

/// <summary>
/// Aggregable bitwise enum flags for representing sides (or center) of a cell.
/// Values can be concatenated together produce one value representing an array of bits for if adjacent cells match.
/// For example: TopLeft | BackLeft =  4096 + 1 = 4097 which means a condition is true for the Top Left and Back Left sides of the cell.
/// Two flags can not be joined to create another flag. ie. Back | Left != BackLeft
/// </summary>
[Flags]
public enum CellSideFlags
{
    None = 0,
    Top = 1,
    Center = 2,
    Bottom = 4,
    TopBack = 8,
    TopRight = 16,
    TopFront = 32,
    TopLeft = 64,
    TopBackLeft = 128,
    TopBackRight = 256,
    TopFrontRight = 512,
    TopFrontLeft = 1024,
    Back = 2048,
    Right = 4096,
    Front = 8192,
    Left = 16384,
    BackLeft = 32768,
    BackRight = 65536,
    FrontRight = 131072,
    FrontLeft = 262144,
    BottomBack = 524288,
    BottomRight = 1048576,
    BottomFront = 2097152,
    BottomLeft = 4194304,
    BottomBackLeft = 8388608,
    BottomBackRight = 16777216,
    BottomFrontRight = 33554432,
    BottomFrontLeft = 67108864
}

public static class CellSideFlagExtensions
{
    public static IEnumerable<CellSideFlags> EnumerateCellSides(bool includeCenter = false)
    {
        foreach (var side in (CellSideFlags[]) Enum.GetValues(typeof(CellSideFlags)))
        {
            if(side == CellSideFlags.Center && !includeCenter)
                continue;
            yield return side;
        }
    }
    
    public static bool HasFlag(this CellSideFlags flags, CellSideFlags check)
    {
        return ((flags & check) == check);
    }
    
    /// <summary>
    /// Get the CellSideFlags that would result from turning this CellSideFlags in the provided RotationDirection.
    /// </summary>
    /// <param name="cellSide">Current CellSide flags</param>
    /// <param name="turn">Direction to turn</param>
    /// <returns>Rotated Occupation</returns>
    public static CellSideFlags Rotate(this CellSideFlags cellSide, RotationDirection turn)
    {
        if (turn == RotationDirection.None)
        {
            return cellSide;
        }
        
        var bitSets = new int[]
        {
            3, // Top Cardinal
            7, // Top Diagonal
            11, // Mid Cardinal
            15, // Mid Diagonal
            19, // Bottom Caridnal
            23 // Bottom Diaginal

        };
        
        var orgBitVector = new BitArray(new int[] { (int)cellSide});
        var newBitVector = new BitArray(32);

        // Top
        newBitVector.Set(0, orgBitVector.Get(0));
        // Center
        newBitVector.Set(1, orgBitVector.Get(1));
        // Bottom
        newBitVector.Set(2, orgBitVector.Get(2));
        
        foreach (var index in bitSets)
        {
            // [a,b,c,d]
            switch (turn)
            {
                case RotationDirection.None:
                    break;
                case RotationDirection.Clockwise:
                    // [d,a,b,c]
                    newBitVector.Set(index, orgBitVector.Get(index+3));
                    newBitVector.Set(index+1, orgBitVector.Get(index));
                    newBitVector.Set(index+2, orgBitVector.Get(index+1));
                    newBitVector.Set(index+3, orgBitVector.Get(index+2));
                    break;
                case RotationDirection.HalfTurn:
                    // [c,d,a,b]
                    newBitVector.Set(index, orgBitVector.Get(index+2));
                    newBitVector.Set(index+1, orgBitVector.Get(index+3));
                    newBitVector.Set(index+2, orgBitVector.Get(index));
                    newBitVector.Set(index+3, orgBitVector.Get(index+1));
                    break;
                case RotationDirection.CounterClockwise:
                    // [b,c,d,a]
                    newBitVector.Set(index,orgBitVector.Get(index+1));
                    newBitVector.Set(index+1, orgBitVector.Get(index+2));
                    newBitVector.Set(index+2, orgBitVector.Get(index+3));
                    newBitVector.Set(index+3, orgBitVector.Get(index));
                    break;
            }
        }
        
        int[] array = new int[1];
        newBitVector.CopyTo(array, 0);
        return (CellSideFlags)array[0];
    }
}