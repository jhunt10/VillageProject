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
    Inner = 1,
    Middle = 2,
    Outer = 4,
    
    Top = 8,
    Center = 16,
    Bottom = 32,
    
    TopBack = 64,
    TopRight = 128,
    TopFront = 256,
    TopLeft = 512,
    TopBackLeft = 1024,
    TopBackRight = 2048,
    TopFrontRight = 4096,
    TopFrontLeft = 8192,
    Back = 16384,
    Right = 32768,
    Front = 65536,
    Left = 131072,
    BackLeft = 262144,
    BackRight = 524288,
    FrontRight = 1048576,
    FrontLeft = 2097152,
    BottomBack = 4194304,
    BottomRight = 8388608,
    BottomFront = 16777216,
    BottomLeft = 33554432,
    BottomBackLeft = 67108864,
    BottomBackRight = 134217728,
    BottomFrontRight = 268435456,
    BottomFrontLeft = 536870912,
    
    Full = OccupationFlags.Inner | OccupationFlags.Middle | OccupationFlags.Outer | OccupationFlags.Top | OccupationFlags.Center | OccupationFlags.Bottom 
           | OccupationFlags.TopBack | OccupationFlags.TopRight | OccupationFlags.TopFront | OccupationFlags.TopLeft | OccupationFlags.TopBackLeft 
           | OccupationFlags.TopBackRight | OccupationFlags.TopFrontRight | OccupationFlags.TopFrontLeft | OccupationFlags.Back | OccupationFlags.Right 
           | OccupationFlags.Front | OccupationFlags.Left | OccupationFlags.BackLeft | OccupationFlags.BackRight | OccupationFlags.FrontRight 
           | OccupationFlags.FrontLeft | OccupationFlags.BottomBack | OccupationFlags.BottomRight | OccupationFlags.BottomFront | OccupationFlags.BottomLeft 
           | OccupationFlags.BottomBackLeft | OccupationFlags.BottomBackRight | OccupationFlags.BottomFrontRight | OccupationFlags.BottomFrontLeft,
}

public static class OccupationExtentions
{
    /// <summary>
    /// Does this set of flag contain the one provided
    /// </summary>
    /// <param name="flag">This OccupationFlags</param>
    /// <param name="check">OccupationFlag to check for</param>
    /// <returns>True if contains, otherwise False</returns>
    public static bool ContainsFlag(this OccupationFlags flag, OccupationFlags check)
    {
        return ((flag & check) == check);
    }

    /// <summary>
    /// Remove the provided flag if this flag contains it
    /// </summary>
    /// <param name="flag">This OccupationFlags</param>
    /// <param name="toRemove">OccupationFlag to be removed</param>
    /// <returns></returns>
    public static OccupationFlags RemoveFlag(this OccupationFlags flag, OccupationFlags toRemove)
    {
        if (flag.ContainsFlag(toRemove))
            return (OccupationFlags)((int)flag - (int)toRemove);
        return flag;
    }

    public static bool OverlapsFlags(this OccupationFlags flag, OccupationFlags check)
    {
        if ((flag.ContainsFlag(OccupationFlags.Inner) != check.ContainsFlag(OccupationFlags.Inner))
            || (flag.ContainsFlag(OccupationFlags.Middle) != check.ContainsFlag(OccupationFlags.Middle))
            || (flag.ContainsFlag(OccupationFlags.Outer) != check.ContainsFlag(OccupationFlags.Outer)))
            return false;
        
        var rawOccA = flag.RemoveFlag(OccupationFlags.Inner)
            .RemoveFlag(OccupationFlags.Middle)
            .RemoveFlag(OccupationFlags.Outer);
        var rawOccB = check.RemoveFlag(OccupationFlags.Inner)
            .RemoveFlag(OccupationFlags.Middle)
            .RemoveFlag(OccupationFlags.Outer);

        return (rawOccA & rawOccB) > 0;
    }
    
    /// <summary>
    /// Get the occupation that would result from turning this occupation in the provided rotation direction.
    /// </summary>
    /// <param name="occupation">Current Occupation</param>
    /// <param name="turn">Direction to turn</param>
    /// <returns>Rotated Occupation</returns>
    public static OccupationFlags Rotate(this OccupationFlags occupation, RotationDirection turn)
    {
        if (turn == RotationDirection.None)
        {
            return occupation;
        }
        
        var bitSets = new int[]
        {
            6, // Top Cardinal
            10, // Top Diagonal
            14, // Mid Cardinal
            18, // Mid Diagonal
            22, // Bottom Caridnal
            26 // Bottom Diaginal

        };
        
        var orgBitVector = new BitArray(new int[] { (int)occupation});
        var newBitVector = new BitArray(32);

        // Inner Flag
        newBitVector.Set(0, orgBitVector.Get(0));
        // Middle Flag
        newBitVector.Set(1, orgBitVector.Get(1));
        // Outer Flag
        newBitVector.Set(2, orgBitVector.Get(2));
        // Top
        newBitVector.Set(3, orgBitVector.Get(3));
        // Center
        newBitVector.Set(4, orgBitVector.Get(4));
        // Bottom
        newBitVector.Set(5, orgBitVector.Get(5));
        
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