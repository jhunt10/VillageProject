using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map.MapSpaces;

namespace VillageProject.Core.Map.Pathing;

public class PathingCompInst : BaseCompInst, ICompInst, IMapPlacementWatcherComp
{
    private Dictionary<MapSpot, PathSpotDef> _relativePathSpotDefs { get; set; }
        = new Dictionary<MapSpot, PathSpotDef>();

    private MapPositionData? _cachedPos;
    
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

    public void MapPositionSet(MapPositionData? mapPos)
    {
        var pathingCompDef = (PathingCompDef)CompDef;
        if(_cachedPos == mapPos)
            return;

        _relativePathSpotDefs.Clear();
        _cachedPos = mapPos;

        if (!_cachedPos.HasValue)
        {
            Active = false;
            return;
        }
        Active = true;
        var pos = _cachedPos.Value;
        foreach (var pathSpotPair in pathingCompDef.PathSpotDefs)
        {
            var relativeSpot = pathSpotPair.Key.RotateSpot(pos.Rotation) + pos.MapSpot;
            var relativePathSpot = new PathSpotDef
            {
                PathOverCost = pathSpotPair.Value.PathOverCost,
                BlocksPathThrough = pathSpotPair.Value.BlocksPathThrough
                    .Rotate(pos.Rotation.GetRotationDirection(RotationFlag.North)),
                SupportsPathOver = pathSpotPair.Value.SupportsPathOver
                    .Rotate(pos.Rotation.GetRotationDirection(RotationFlag.North))
            };
            _relativePathSpotDefs.Add(relativeSpot, relativePathSpot);
        }
    }
}