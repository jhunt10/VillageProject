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

    public float DistanceToSpot(MapSpot otherSpot)
    {
        return Math.Abs(this.X - otherSpot.X) +
               Math.Abs(this.Y - otherSpot.Y) +
               Math.Abs(this.Z - otherSpot.Z);
    }
    
    // AdjacencyToSpot doesn't make sense because one AdjacencyFlags contains references to multiple spots
    // public MapSpot AdjacencyToSpot(AdjacencyFlags flags, RotationFlag rotationFlag = RotationFlag.North)

    public MapSpot DirectionToSpot(DirectionFlag direction, RotationFlag rotation = RotationFlag.North)
    {
        var x = 0;
        var y = 0;
        var z = 0;
        if   ( direction == DirectionFlag.Back || direction == DirectionFlag.BackLeft || direction == DirectionFlag.BackRight 
            || direction == DirectionFlag.TopBack || direction == DirectionFlag.TopBackLeft || direction == DirectionFlag.TopBackRight
            || direction == DirectionFlag.BottomBack || direction == DirectionFlag.BottomBackLeft || direction == DirectionFlag.BottomBackRight)
            y += 1;
        
        if  ( direction == DirectionFlag.Front || direction == DirectionFlag.FrontLeft || direction == DirectionFlag.FrontRight 
           || direction == DirectionFlag.TopFront || direction == DirectionFlag.TopFrontLeft || direction == DirectionFlag.TopFrontRight
           || direction == DirectionFlag.BottomFront || direction == DirectionFlag.BottomFrontLeft || direction == DirectionFlag.BottomFrontRight)
            y -= 1;
        
        
        if  ( direction == DirectionFlag.BackLeft || direction == DirectionFlag.Left || direction == DirectionFlag.FrontLeft 
           || direction == DirectionFlag.TopBackLeft || direction == DirectionFlag.TopLeft || direction == DirectionFlag.TopFrontLeft
           || direction == DirectionFlag.BottomBackLeft || direction == DirectionFlag.BottomLeft || direction == DirectionFlag.BottomFrontLeft)
            x -= 1;
        
        if ( direction == DirectionFlag.BackRight || direction == DirectionFlag.Right || direction == DirectionFlag.FrontRight 
          || direction == DirectionFlag.TopBackRight || direction == DirectionFlag.TopRight || direction == DirectionFlag.TopFrontRight
          || direction == DirectionFlag.BottomBackRight || direction == DirectionFlag.BottomRight || direction == DirectionFlag.BottomFrontRight)
            x += 1;
        
        if( direction == DirectionFlag.TopBackLeft || direction == DirectionFlag.TopBack || direction == DirectionFlag.TopBackRight 
        || direction == DirectionFlag.TopLeft || direction == DirectionFlag.Top || direction == DirectionFlag.TopRight
        || direction == DirectionFlag.TopFrontLeft || direction == DirectionFlag.TopFront || direction == DirectionFlag.TopFrontRight)
            z += 1;
        
        if( direction == DirectionFlag.BottomBackLeft || direction == DirectionFlag.BottomBack || direction == DirectionFlag.BottomBackRight 
        || direction == DirectionFlag.BottomLeft || direction == DirectionFlag.Bottom || direction == DirectionFlag.BottomRight
        || direction == DirectionFlag.BottomFrontLeft || direction == DirectionFlag.BottomFront || direction == DirectionFlag.BottomFrontRight)
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
    
    public IEnumerable<KeyValuePair<DirectionFlag, MapSpot>> ListAdjacentSpots(RotationFlag rot = RotationFlag.North, bool includeVertical = false, bool includeCenter = false)
    {
        foreach (var direction in (DirectionFlag[]) Enum.GetValues(typeof(DirectionFlag)))
        {
            if(direction == DirectionFlag.None && !includeCenter)
                continue;
            if(direction.HasFlag(DirectionFlag.Top) && !includeVertical)
                continue;
            if(direction.HasFlag(DirectionFlag.Bottom) && !includeVertical)
                continue;
            
            yield return new KeyValuePair<DirectionFlag, MapSpot>(direction, DirectionToSpot(direction, rot));
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