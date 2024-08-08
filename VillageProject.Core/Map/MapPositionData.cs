using System.Numerics;
using VillageProject.Core.DIM;
using VillageProject.Core.Enums;
using VillageProject.Core.Map.MapSpaces;

namespace VillageProject.Core.Map;

public struct MapPositionData
{
    public MapSpot MapSpot { get; }
    public RotationFlag Rotation { get; }
    public Vector3 Offset { get; }
    public string MapSpaceId { get; }
    public IMapSpace MapSpace
    {
        get
        {
            var mapManager = DimMaster.GetManager<MapManager>();
            return mapManager.GetMapSpaceById(MapSpaceId);
        }
    }

    public MapPositionData(IMapSpace mapSpace, MapSpot spot, RotationFlag rotation, Vector3? offet = null)
    {
        MapSpaceId = mapSpace.MapSpaceId;
        MapSpot = spot;
        Rotation = rotation;
        if (offet.HasValue)
            Offset = offet.Value;
        else
            Offset = new Vector3(0, 0, 0);
    }
    
    public static bool operator ==(MapPositionData m1, MapPositionData m2)
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

        return m1.MapSpaceId == m2.MapSpaceId 
               && m1.MapSpot == m2.MapSpot 
               && m1.Rotation == m2.Rotation
               && m1.Offset == m2.Offset;
    }
    
    public static bool operator !=(MapPositionData m1, MapPositionData m2)
    {
        
        return m1.MapSpaceId != m2.MapSpaceId 
               || m1.MapSpot != m2.MapSpot 
               || m1.Rotation != m2.Rotation
               || m1.Offset != m2.Offset;
    }
}