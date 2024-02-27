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
    /// <summary>
    /// Does this set of flag contain the one provided
    /// </summary>
    /// <param name="flag">This AdjacencyFlags</param>
    /// <param name="check">AdjacencyFlags to check for</param>
    /// <returns>True if contains, otherwise False</returns>
    public static bool HasFlag(this AdjacencyFlags flag, AdjacencyFlags check)
    {
        return ((flag & check) == check);
    }
    
    /// <summary>
    /// Returns Atlas Coordinates of tile sprite based on adjacent tiles.
    /// Always matches on side. Only matches on corners if sides are present.
    /// i.e. if there is no Back or no Left, we don't care what BackLeft is
    /// See VillageProject.Core.Sprites.TileSpriteTemplate.png for example
    /// </summary>
    /// <param name="adj">This AdjacencyFlags</param>
    /// <returns>Atlas Coordinates as in[X,Y]</returns>
    public static int[] ToAtlasCo(this AdjacencyFlags adj)
    {
        if(((int)adj) < 0)
            return new int[] { 10, 1 };
        // Has Back
        if (adj.HasFlag(AdjacencyFlags.Back))
        {
            if (adj.HasFlag(AdjacencyFlags.Left))
            {
                if (adj.HasFlag(AdjacencyFlags.Right))
                {
                    if (adj.HasFlag(AdjacencyFlags.Front))
                    {
                        if (adj.HasFlag(AdjacencyFlags.BackLeft))
                        {
                            if (adj.HasFlag(AdjacencyFlags.BackRight))
                            {
                                if (adj.HasFlag(AdjacencyFlags.FrontLeft))
                                {
                                    if (adj.HasFlag(AdjacencyFlags.FrontRight))
                                    {
                                        // Has Back, Left, Right, Front, BackLeft, BackRight, FrontLeft, FrontRight
                                        // Has All
                                        return new int[] { 9, 2 };
                                    }
                                    // No Front Right
                                    else
                                    {
                                        // Has Back, Left, Right, Front, BackLeft, BackRight, FrontLeft
                                        return new int[] { 6, 2 };
                                    }
                                }
                                // No Front Left
                                else
                                {
                                    // Has Back, Left, Right, Front, BackLeft, BackRight, FrontRight
                                    if (adj.HasFlag(AdjacencyFlags.FrontRight))
                                    {
                                        return new int[] { 5, 2 };
                                    }
                                    // Has Back, Left, Right, Front, BackLeft, BackRight
                                    else
                                    {
                                        return new int[] { 10, 3 };
                                    }
                                }
                            }
                            // No BackRight
                            else
                            {
                                if (adj.HasFlag(AdjacencyFlags.FrontLeft))
                                {
                                    if (adj.HasFlag(AdjacencyFlags.FrontRight))
                                    {
                                        // Has Back, Left, Right, Front, BackLeft, FrontLeft, FrontRight
                                        return new int[] { 6, 1 };
                                    }
                                    // No FrontRight
                                    else
                                    {
                                        // Has Back, Left, Right, Front, BackLeft, FrontLeft
                                        return new int[] { 11, 1 };
                                    }
                                }
                                // No FrontLeft
                                else
                                {
                                    if (adj.HasFlag(AdjacencyFlags.FrontRight))
                                    {
                                        // Has Back, Left, Right, Front, BackLeft, FrontRight
                                        return new int[] { 10, 2 };
                                    }
                                    // No FrontRight
                                    else
                                    {
                                        // Has Back, Left, Right, Front, BackLeft
                                        return new int[] { 4, 0 };
                                    }
                                }
                            }
                        }
                        // No BackLeft
                        else
                        {
                            if (adj.HasFlag(AdjacencyFlags.BackRight))
                            {
                                if (adj.HasFlag(AdjacencyFlags.FrontLeft))
                                {
                                    if (adj.HasFlag(AdjacencyFlags.FrontRight))
                                    {
                                        // Has Back, Left, Right, Front, BackRight, FrontLeft, FrontRight
                                        // Has All
                                        return new int[] { 5, 1 };
                                    }
                                    // No Front Right
                                    else
                                    {
                                        // Has Back, Left, Right, Front, BackRight, FrontLeft
                                        return new int[] { 9, 1 };
                                    }
                                }
                                // No Front Left
                                else
                                {
                                    // Has Back, Left, Right, Front, BackRight, FrontRight
                                    if (adj.HasFlag(AdjacencyFlags.FrontRight))
                                    {
                                        return new int[] { 8, 2 };
                                    }
                                    // Has Back, Left, Right, Front, BackRight
                                    else
                                    {
                                        return new int[] { 7, 0 };
                                    }
                                }
                            }
                            // No BackRight
                            else
                            {
                                if (adj.HasFlag(AdjacencyFlags.FrontLeft))
                                {
                                    if (adj.HasFlag(AdjacencyFlags.FrontRight))
                                    {
                                        // Has Back, Left, Right, Front, FrontLeft, FrontRight
                                        return new int[] { 9, 0 };
                                    }
                                    // No FrontRight
                                    else
                                    {
                                        // Has Back, Left, Right, Front, FrontLeft
                                        return new int[] { 4, 3 };
                                    }
                                }
                                // No FrontLeft
                                else
                                {
                                    if (adj.HasFlag(AdjacencyFlags.FrontRight))
                                    {
                                        // Has Back, Left, Right, Front, FrontRight
                                        return new int[] { 7, 3 };
                                    }
                                    // No FrontRight
                                    else
                                    {
                                        // Has Back, Left, Right, Front
                                        return new int[] { 2, 1 };
                                    }
                                }
                            }
                        }
                    }
                    // No Front
                    else
                    {
                        if (adj.HasFlag(AdjacencyFlags.BackLeft))
                        {
                            if (adj.HasFlag(AdjacencyFlags.BackRight))
                            {
                                // Has Back, Left, Right, BackLeft, BackRight
                                return new int[] { 9, 3 };
                            }
                            // No BackRight
                            else
                            {
                                // Has Back, Left, Right, BackLeft
                                return new int[] { 6, 3 };
                            }
                        }
                        // No BackLeft
                        else
                        {
                            if (adj.HasFlag(AdjacencyFlags.BackRight))
                            {
                                // Has Back, Left, Right, BackRight
                                return new int[] { 5, 3 };
                            }
                            // No BackRight
                            else
                            {
                                // Has Back, Left, Right
                                return new int[] { 2, 2 };
                            }
                        }
                    }
                }
                // No Right
                else
                {
                    if (adj.HasFlag(AdjacencyFlags.Front))
                    {
                        if (adj.HasFlag(AdjacencyFlags.BackLeft))
                        {
                            if (adj.HasFlag(AdjacencyFlags.FrontLeft))
                            {
                                // Has Back, Left, Front, BackLeft, FrontLeft
                                return new int[] { 11, 2 };
                            }
                            // No Front Left
                            else
                            {
                                // Has Back, Left, Front, BackLeft
                                return new int[] { 7, 2 };
                            }
                        }
                        // No BackLeft
                        else
                        {
                            if (adj.HasFlag(AdjacencyFlags.FrontLeft))
                            {
                                // Has Back, Left, Front, FrontLeft
                                return new int[] { 11, 2 };
                            }
                            // No Front Left
                            else
                            {
                                // Has Back, Left, Front
                                return new int[] { 3, 1 };
                            }
                        }
                    }
                    // No Front
                    else
                    {
                        if (adj.HasFlag(AdjacencyFlags.BackLeft))
                        {
                            // Has Back, Left, BackLeft
                            return new int[] { 11, 3 };
                        }
                        // No BackLeft
                        else
                        {
                            // Has Back, Left,
                            return new int[] { 3, 2 };
                        }
                    }
                }
            }
            // No Left
            else
            {
                if (adj.HasFlag(AdjacencyFlags.Right))
                {
                    if (adj.HasFlag(AdjacencyFlags.Front))
                    {
                        if (adj.HasFlag(AdjacencyFlags.BackRight))
                        {
                            if (adj.HasFlag(AdjacencyFlags.FrontRight))
                            {
                                // Has Back, Right, Front, BackRight, FrontRight
                                return new int[] { 8, 1 };
                            }
                            // No FrontRight
                            else
                            {
                                // Has Back, Right, Front, BackRight
                                return new int[] { 4, 2 };
                            }
                        }
                        // No BackRight
                        else
                        {
                            if (adj.HasFlag(AdjacencyFlags.FrontRight))
                            {
                                // Has Back, Right, Front, FrontRight
                                return new int[] { 4, 1 };
                            }
                            // No FrontRight
                            else
                            {
                                // Has Back, Right, Front
                                return new int[] { 1, 1 };
                            }
                        }
                    }
                    // No Front
                    else
                    {
                        if (adj.HasFlag(AdjacencyFlags.BackRight))
                        {
                            // Has Back, Right, BackRight
                            return new int[] { 8, 3 };
                        }
                        // No BackRight
                        else
                        {
                            // Has Back, Right
                            return new int[] { 1, 2 };
                        }
                    }
                }
                // No Right
                else
                {
                    if (adj.HasFlag(AdjacencyFlags.Front))
                    {
                        // Has Back, Front
                        return new int[] { 0, 1 };

                    }
                    // No Front
                    else
                    {
                        // Has Back
                        return new int[] { 0, 2 };
                    }
                }
            }
        }
        // No Back
        else
        {
            if (adj.HasFlag(AdjacencyFlags.Left))
            {
                if (adj.HasFlag(AdjacencyFlags.Right))
                {
                    if (adj.HasFlag(AdjacencyFlags.Front))
                    {
                        if (adj.HasFlag(AdjacencyFlags.FrontLeft))
                        {
                            if (adj.HasFlag(AdjacencyFlags.FrontRight))
                            {
                                // Has Left, Right, Front, FrontLeft, FrontRight
                                return new int[] { 10, 0 };
                            }
                            // No FrontRight
                            else
                            {
                                // Has Left, Right, Front, FrontLeft
                                return new int[] { 6, 0 };
                            }
                        }
                        // No FrontLeft
                        else
                        {
                            if (adj.HasFlag(AdjacencyFlags.FrontRight))
                            {
                                // Has Left, Right, Front, FrontRight
                                return new int[] { 5, 0 };
                            }
                            // No FrontRight
                            else
                            {
                                // Has Left, Right, Front
                                return new int[] { 2, 0 };
                            }
                        }
                    }
                    // No Front
                    else
                    {
                        // Has Left Right
                        return new int[] { 2, 3 };
                    }
                }

                // No Right
                {
                    if (adj.HasFlag(AdjacencyFlags.Front))
                    {
                        if (adj.HasFlag(AdjacencyFlags.FrontLeft))
                        {
                            // Has Left, Front, FrontLeft
                            return new int[] { 11, 0 };
                        }
                        // No Front Left
                        else
                        {
                            // Has Left, Front
                            return new int[] { 3, 0 };
                        }

                    }
                    // No Front
                    else
                    {
                        // Has Left
                        return new int[] { 3, 3 };
                    }
                }
            }
            // No Left
            else
            {
                if (adj.HasFlag(AdjacencyFlags.Right))
                {
                    if (adj.HasFlag(AdjacencyFlags.Front))
                    {
                        if (adj.HasFlag(AdjacencyFlags.FrontRight))
                        {
                            // Has Right, Front, FrontRight
                            return new int[] { 8, 0 };
                        }
                        // No FrontRight
                        else
                        {
                            // Has Right Front
                            return new int[] { 1, 0 };
                        }
                    }
                    // No Front
                    else
                    {
                        // Has Right
                        return new int[] { 1, 3 };

                    }
                }
                // No Right
                else
                {
                    if (adj.HasFlag(AdjacencyFlags.Front))
                    {
                        // Has Front
                        return new int[] { 0, 0 };
                    }
                    // No FrontRight
                    else
                    {
                        // Has None
                        return new int[] { 0, 3 };
                    }
                }
            }
        }
    }
}