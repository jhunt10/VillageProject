using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map.MapSpaces;
using VillageProject.Core.Map.MapStructures;

namespace VillageProject.Core.Map;

public class MapManager : BaseManager
{
    private Dictionary<string, MapSpaceCompInst> _mapSpaces = new Dictionary<string, MapSpaceCompInst>();
    
    // public void SetMainMapSpace(MapSpaceCompInst space)
    // {
    //     if(!_mapSpaces.ContainsKey(space.MapSpaceId))
    //         _mapSpaces.Add(space.MapSpaceId, space);
    //     _mainMapSpaceCompInst = spaceCompInst;
    // }
    // public MapSpaceCompInst GetMainMapSpace()
    // {
    //     return _mainMapSpaceCompInst;
    // }

    public IMapSpace? GetMapSpaceById(string id, bool errorIfNull = true)
    {
        if (_mapSpaces.ContainsKey(id))
            return _mapSpaces[id];
        if (errorIfNull)
            throw new Exception($"Failed to find MapSpace with id '{id}'.");
        return null;
    }

    public override ICompInst CreateCompInst(ICompDef compDef, IInst newInst, object? args)
    {
        var newComp = base.CreateCompInst(compDef, newInst, args);
        var newMapComp = newComp as MapSpaceCompInst;
        if (newMapComp == null)
            throw new Exception("Failed to cast new comp to type MapSpaceCompInst");
        if(!_mapSpaces.ContainsKey(newMapComp.MapSpaceId))
            _mapSpaces.Add(newMapComp.MapSpaceId, newMapComp);
        return newMapComp;
    }

    public override ICompInst LoadSavedCompInst(ICompDef compDef, IInst newInst, DataDict? dataDict)
    {
        var newComp = base.LoadSavedCompInst(compDef, newInst, dataDict);
        var newMapComp = newComp as MapSpaceCompInst;
        if (newMapComp == null)
            throw new Exception("Failed to cast new comp to type MapSpaceCompInst");
        if(!_mapSpaces.ContainsKey(newMapComp.MapSpaceId))
            _mapSpaces.Add(newMapComp.MapSpaceId, newMapComp);
        return newMapComp;
    }

    public override void OnInstDelete(IInst inst)
    {
        base.OnInstDelete(inst);
        foreach(var mapStructComp in inst.ListComponentsOfType<MapStructCompInst>())
        {
            if (!string.IsNullOrEmpty(mapStructComp.MapSpaceId) && _mapSpaces.ContainsKey(mapStructComp.MapSpaceId))
            {
                var mapSpace = _mapSpaces[mapStructComp.MapSpaceId];
                mapSpace.RemoveInst(inst);
            }
        }
        
    }

    public Result CouldPlaceDefOnMapSpace(
        IMapSpace mapSpace, 
        IDef def, 
        MapSpot anchorSpot, 
        RotationFlag rotation, 
        Dictionary<string, object>? args = null)
    {
        // Console.WriteLine($"CouldPlaceDefOnMapSpace: {anchorSpot} {rotation}");
        var compArgs = args;
        if (compArgs == null)
            compArgs = new Dictionary<string, object>();

        if (!mapSpace.InBounds(anchorSpot))
            return new Result(false, "Out of bounds");
        
        // Check that all managers allow for all occupied spots spots
        foreach (var manager in DimMaster.ListManagersOfType<IMapPlacementValidator>())
        {
            var name = manager.GetType().FullName;
            object? arg = (compArgs != null && compArgs.ContainsKey(name)) ? compArgs[name] : null;
            var res = manager.CouldPlaceDefOnMapSpace(mapSpace, def, anchorSpot, rotation, arg);
            if (!res.Success)
                return res;
        }

        return new Result();
    }

    public Result CanPlaceInstOnMapSpace(
        IMapSpace mapSpace, 
        IInst inst, 
        MapSpot anchorSpot, 
        RotationFlag rotation, 
        Dictionary<string, object>? args = null)
    {
        var compArgs = args;
        if (compArgs == null)
            compArgs = new Dictionary<string, object>();

        if (!mapSpace.InBounds(anchorSpot))
            return new Result(false, "Out of bounds");
        
        // Check that all managers allow for all occupied spots spots
        foreach (var manager in DimMaster.ListManagersOfType<IMapPlacementValidator>())
        {
            var name = manager.GetType().FullName;
            object? arg = (compArgs != null && compArgs.ContainsKey(name)) ? compArgs[name] : null;
            var res = manager.CanPlaceInstOnMapSpace(mapSpace, inst, anchorSpot, rotation, arg);
            if (!res.Success)
                return res;
        }

        return new Result();
    }

    public Result TryPlaceInstOnMapSpace(
        IMapSpace mapSpace,
        IInst inst,
        MapSpot anchorSpot,
        RotationFlag rotation,
        Dictionary<string, object>? args = null)
    {
        var compArgs = args;
        if (compArgs == null)
            compArgs = new Dictionary<string, object>();
        
        var canRes = CanPlaceInstOnMapSpace(mapSpace, inst, anchorSpot, rotation, compArgs);
        if (!canRes.Success)
            return canRes;

        Result? failedRes = null;
        
        foreach (var manager in DimMaster.ListManagersOfType<IMapPlacementValidator>())
        {
            var name = manager.GetType().FullName;
            object? arg = (compArgs != null && compArgs.ContainsKey(name)) ? compArgs[name] : null;
            var res = manager.TryPlaceInstOnMapSpace(mapSpace, inst, anchorSpot, rotation, arg);
            if (!res.Success)
            {
                failedRes = res;
            }
        }

        if (failedRes != null)
        {
            mapSpace.RemoveInst(inst);
            return failedRes;
        }

        return new Result(true);
    }
}