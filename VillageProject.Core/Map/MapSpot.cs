namespace VillageProject.Core.Map;

/// <summary>
/// Bitwise flags for cardinal directions
/// </summary>
public enum RotationFlag
{
    North = 1,
    East = 2,
    South = 4,
    West = 8
}

/// <summary>
/// Bitwise enum flags for representing adjacency.
/// Values can be concatenated together produce one value representing an array of bits for if adjacent cells match.
/// For example: TopLeft | BackLeft =  4096 + 1 = 4097 which means a condition is true for the Top Left and Back Left cells.
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

public struct MapSpot
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public MapSpot(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    
    public MapSpot(string s)
    {
        var tokens = s.Trim(new char[] { '(', ')' }).Split(',');

        X = Y = Z = int.MinValue;

        bool worked = true;
        if (tokens.Length != 3)
            return;
        else if (!int.TryParse(tokens[0], out int x))
            worked = false;
        else if (!int.TryParse(tokens[1], out int y))
            worked = false;
        else if (!int.TryParse(tokens[2], out int z))
            worked = false;
        else if(worked)
        {
            X = x; Y = y; Z = z;
        }
    }

    // AdjacencyToSpot doesn't make sense because one AdjacencyFlags contains references to multiple spots
    // public MapSpot AdjacencyToSpot(AdjacencyFlags flags, RotationFlag rotationFlag = RotationFlag.North)

    public MapSpot DirectionToSpot(DirectionFlags direction, RotationFlag rotation = RotationFlag.North)
    {
        var x = 0;
        var y = 0;
        var z = 0;
        if   ( direction == DirectionFlags.Back || direction == DirectionFlags.BackLeft || direction == DirectionFlags.BackRight 
            || direction == DirectionFlags.TopBack || direction == DirectionFlags.TopBackLeft || direction == DirectionFlags.TopBackRight
            || direction == DirectionFlags.BottomBack || direction == DirectionFlags.BottomBackLeft || direction == DirectionFlags.BottomBackRight)
            y -= 1;
        
        if  ( direction == DirectionFlags.Front || direction == DirectionFlags.FrontLeft || direction == DirectionFlags.FrontRight 
           || direction == DirectionFlags.TopFront || direction == DirectionFlags.TopFrontLeft || direction == DirectionFlags.TopFrontRight
           || direction == DirectionFlags.BottomFront || direction == DirectionFlags.BottomFrontLeft || direction == DirectionFlags.BottomFrontRight)
            y += 1;
        
        
        if  ( direction == DirectionFlags.BackLeft || direction == DirectionFlags.Left || direction == DirectionFlags.FrontLeft 
           || direction == DirectionFlags.TopBackLeft || direction == DirectionFlags.TopLeft || direction == DirectionFlags.TopFrontLeft
           || direction == DirectionFlags.BottomBackLeft || direction == DirectionFlags.BottomLeft || direction == DirectionFlags.BottomFrontLeft)
            x -= 1;
        
        if ( direction == DirectionFlags.BackRight || direction == DirectionFlags.Right || direction == DirectionFlags.FrontRight 
          || direction == DirectionFlags.TopBackRight || direction == DirectionFlags.TopRight || direction == DirectionFlags.TopFrontRight
          || direction == DirectionFlags.BottomBackRight || direction == DirectionFlags.BottomRight || direction == DirectionFlags.BottomFrontRight)
            x += 1;
        
        if( direction == DirectionFlags.TopBackLeft || direction == DirectionFlags.TopBack || direction == DirectionFlags.TopBackRight 
        || direction == DirectionFlags.TopLeft || direction == DirectionFlags.Top || direction == DirectionFlags.TopRight
        || direction == DirectionFlags.TopFrontLeft || direction == DirectionFlags.TopFront || direction == DirectionFlags.TopFrontRight)
            z += 1;
        
        if( direction == DirectionFlags.BottomBackLeft || direction == DirectionFlags.BottomBack || direction == DirectionFlags.BottomBackRight 
        || direction == DirectionFlags.BottomLeft || direction == DirectionFlags.Bottom || direction == DirectionFlags.BottomRight
        || direction == DirectionFlags.BottomFrontLeft || direction == DirectionFlags.BottomFront || direction == DirectionFlags.BottomFrontRight)
            z -= 1;
        
        //TODO: Rotation

        return new MapSpot(X + x, Y + y, Z + z);
    }
    
    public IEnumerable<KeyValuePair<DirectionFlags, MapSpot>> ListAdjacentSpots(RotationFlag rot = RotationFlag.North, bool includeCenter = false)
    {
        foreach (var direction in (DirectionFlags[]) Enum.GetValues(typeof(DirectionFlags)))
        {
            if(direction != DirectionFlags.None || includeCenter)
                yield return new KeyValuePair<DirectionFlags, MapSpot>(direction, DirectionToSpot(direction, rot));
        }
    }
    
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return $"({X},{Y},{Z})";
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        if (obj is MapSpot)
        {
            var spot = (MapSpot)obj;
            return this == spot;
        }
        return false;
    }

    public static bool operator ==(MapSpot m1, MapSpot m2)
    {
        /*
         * This operator is called if either m1 or m2 is a MapSpot meaning m1 or m2 can be 'null'
         * Calling 'MapSpot == null' anywhere in this method will cause a loop
         * Calling 'MapSpot?.X == null' is the same as 'int? == null'
         * 
         * Not needed if MapObject is a Struct
        */
        //if (m1?.X == null && m2?.X == null)
        //    return true;

        //if (m1?.X == null || m2?.X == null)
        //    return false;

        return m1.X == m2.X && m1.Y == m2.Y && m1.Z == m2.Z;
    }

    public static bool operator !=(MapSpot m1, MapSpot m2)
    {

        //if (m1?.X == null && m2?.X == null)
        //    return true;

        //if (m1?.X == null || m2?.X == null)
        //    return false;

        return m1.X != m2.X || m1.Y != m2.Y || m1.Z != m2.Z;
    }

    public static MapSpot operator +(MapSpot m1, MapSpot m2)
    {
        return new MapSpot(m1.X + m2.X, m1.Y + m2.Y, m1.Z + m2.Z);
    }

    public static MapSpot operator -(MapSpot m1, MapSpot m2)
    {
        return new MapSpot(m1.X - m2.X, m1.Y - m2.Y, m1.Z - m2.Z);
    }


    public static implicit operator string(MapSpot s) => s.ToString();
    public static explicit operator MapSpot(string s) => new MapSpot(s);
}