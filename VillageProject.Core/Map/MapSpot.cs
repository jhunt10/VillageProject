using System.Text.Json.Serialization;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Enums;
using VillageProject.Core.Serialization;

namespace VillageProject.Core.Map;









[JsonConverter(typeof(MapSpotJsonConverter))]
public struct MapSpot
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public MapSpot()
    {
        X = 0;
        Y = 0;
        Z = 0;
    }
    
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
            y += 1;
        
        if  ( direction == DirectionFlags.Front || direction == DirectionFlags.FrontLeft || direction == DirectionFlags.FrontRight 
           || direction == DirectionFlags.TopFront || direction == DirectionFlags.TopFrontLeft || direction == DirectionFlags.TopFrontRight
           || direction == DirectionFlags.BottomFront || direction == DirectionFlags.BottomFrontLeft || direction == DirectionFlags.BottomFrontRight)
            y -= 1;
        
        
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
        
        switch (rotation)
        {
            case RotationFlag.North:
                return new MapSpot(X + x, Y + y, Z + z);
            case RotationFlag.East:
                return new MapSpot(X + y, Y - x, Z + z);
            case RotationFlag.South:
                return new MapSpot(X - x, Y - y, Z + z);
            case RotationFlag.West:
                return new MapSpot(X - y, Y + x, Z + z);
        }

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

    public MapSpot RotateSpot(RotationFlag rotation, RotationFlag previousRotation = RotationFlag.North)
    {
        var dir = previousRotation.GetRotationDirection(rotation);
        return RotateSpot(dir);
    }

    public MapSpot RotateSpot(RotationDirection rotDir)
    {
        switch (rotDir)
        {
            case RotationDirection.None:
                return new MapSpot(X, Y, Z);
            case RotationDirection.Clockwise:
                return new MapSpot(Y, -X, Z);
            case RotationDirection.HalfTurn:
                return new MapSpot(-X, -Y, Z);
            case RotationDirection.CounterClockwise:
                return new MapSpot(-Y, X, Z);
        }

        throw new Exception($"Unknown RotationDirection {rotDir}.");
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