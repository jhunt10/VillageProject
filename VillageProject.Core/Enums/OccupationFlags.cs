using System.Collections;
using System.Collections.Specialized;

namespace VillageProject.Core.Enums;


/// <summary>
/// Bitwise enum flags for representing occupation in cells.
/// Each value can only represent a single layer either Inner, Middle, or Outer.
/// Values of the same layer can be concatenated together produce one value representing an array of bits for a layer of occupation. 
/// For example: MidTopLeft | MidBackLeft = both the TopLeft and BackLeft are occupied.
/// Two flags can not be joined to create another flag. ie. InnerBack | InnerLeft != InnerBackLeft
/// Combing two values of different layers is invalid
/// </summary>
[Flags]
public enum OccupationFlags
{
    None = 0,
    Inner = 0,
    Middle = 1,
    Outer = 2,
    
    Top = 4,
    Center = 8,
    Bottom = 16,
    
    TopBack = 32,
    TopRight = 64,
    TopFront = 128,
    TopLeft = 256,
    TopBackLeft = 512,
    TopBackRight = 1024,
    TopFrontRight = 2048,
    TopFrontLeft = 4096,
    Back = 8192,
    Right = 16384,
    Front = 32768,
    Left = 65536,
    BackLeft = 131072,
    BackRight = 262144,
    FrontRight = 524288,
    FrontLeft = 1048576,
    BottomBack = 2097152,
    BottomRight = 4194304,
    BottomFront = 8388608,
    BottomLeft = 16777216,
    BottomBackLeft = 33554432,
    BottomBackRight = 67108864,
    BottomFrontRight = 134217728,
    BottomFrontLeft = 268435456,
}

public static class OccupationExtentions
{
    public static bool HasFlag(this OccupationFlags flag, OccupationFlags check)
    {
        return ((flag & check) == check);
    }
    
    public static OccupationFlags Rotate(this OccupationFlags occupation, RotationDirection turn)
    {
        var bitSets = new int[]
        {
            5, // Top Cardinal
            9, // Top Diagonal
            13, // Mid Cardinal
            17, // Mid Diagonal
            21, // Bottom Caridnal
            25 // Bottom Diaginal

        };
        
        var orgBitVector = new BitArray(new int[] { (int)occupation});
        var newBitVector = new BitArray(32);

        // Middle Flag
        newBitVector.Set(0, orgBitVector.Get(0));
        // Outer Flag
        newBitVector.Set(1, orgBitVector.Get(1));
        // Top
        newBitVector.Set(2, orgBitVector.Get(2));
        // Center
        newBitVector.Set(3, orgBitVector.Get(3));
        // Bottom
        newBitVector.Set(4, orgBitVector.Get(4));
        
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
        return (OccupationFlags)array[0];
    }
}