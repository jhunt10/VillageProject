using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;

namespace VillageProject.Core.Map;

public class MapManager : BaseManager
{

    private MapSpace _mainMapSpace;

    public void SetMainMapSpace(MapSpace space)
    {
        _mainMapSpace = space;
    }
    public MapSpace GetMainMapSpace()
    {
        return _mainMapSpace;
    }

    public Result CanPlaceInstOnMapSpace(
        MapSpace mapSpace, 
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
        MapSpace mapSpace,
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

    public override DataDict BuildSaveData()
    {
        return _mainMapSpace.BuildSaveData();
    }

    public override void LoadSaveData(DataDict data)
    {
        var mapSpace = new MapSpace(data);
        SetMainMapSpace(mapSpace);
    }
}