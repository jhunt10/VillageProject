using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map.MapSpaces;

namespace VillageProject.Core.Map.Pathing;

public class PathingCompInst : BaseCompInst, ICompInst, IMapPlacementWatcherComp
{
    private Dictionary<MapSpot, PathSpotDef> _relativePathSpotDefs { get; set; }
        = new Dictionary<MapSpot, PathSpotDef>();
    
    private string _cachedMapSpaceId { get; set; }
    private MapSpot? _cachedMapSpot { get; set; }
    private RotationFlag _cachedRotation { get; set; }
    
    public PathingCompInst(ICompDef def, IInst inst) : base(def, inst)
    {
        
    }

    public float GetPathCost(MapSpot exitingSpot, MapSpot enteringSpot)
    {
        if (_relativePathSpotDefs.ContainsKey(enteringSpot))
            return (float)_relativePathSpotDefs[enteringSpot].PathOverCost;
        return 0;
    }

    public CellSideFlags GetSupportedPathOver(MapSpot mapSpot)
    {
        if (_relativePathSpotDefs.ContainsKey(mapSpot))
            return _relativePathSpotDefs[mapSpot].SupportsPathOver;
        return CellSideFlags.None;
    }
    
    public CellSideFlags GetSupportedPathThrough(MapSpot mapSpot)
    {
        if (_relativePathSpotDefs.ContainsKey(mapSpot))
            return _relativePathSpotDefs[mapSpot].SupportsPathThrough;
        return CellSideFlags.None;
    }

    public CellSideFlags GetBlockedPathThrough(MapSpot mapSpot)
    {
        if (_relativePathSpotDefs.ContainsKey(mapSpot))
            return _relativePathSpotDefs[mapSpot].BlocksPathThrough;
        return CellSideFlags.None;
    }

    public void MapPositionSet(IMapSpace mapSpaceCompInst, MapSpot mapSpot, RotationFlag rotation)
    {
        var pathingCompDef = (PathingCompDef)CompDef;
        if(mapSpaceCompInst.MapSpaceId == _cachedMapSpaceId 
           && mapSpot == _cachedMapSpot 
           && rotation == _cachedRotation)
            return;

        _cachedMapSpaceId = mapSpaceCompInst.MapSpaceId;
        _cachedMapSpot = mapSpot;
        _cachedRotation = rotation;
        
        _relativePathSpotDefs.Clear();
        foreach (var pathSpotPair in pathingCompDef.PathSpotDefs)
        {
            var relativeSpot = pathSpotPair.Key.RotateSpot(rotation) + mapSpot;
            var relativePathSpot = new PathSpotDef
            {
                PathOverCost = pathSpotPair.Value.PathOverCost,
                BlocksPathThrough = pathSpotPair.Value.BlocksPathThrough
                    .Rotate(rotation.GetRotationDirection(RotationFlag.North)),
                SupportsPathOver = pathSpotPair.Value.SupportsPathOver
                    .Rotate(rotation.GetRotationDirection(RotationFlag.North))
            };
            _relativePathSpotDefs.Add(relativeSpot, relativePathSpot);
        }
    }
}