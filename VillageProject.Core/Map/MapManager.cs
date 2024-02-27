using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;

namespace VillageProject.Core.Map;

public class MapManager : BaseManager
{

    private MapSpace _mainMapSpace;

    public MapSpace GetMainMapSpace()
    {
        return _mainMapSpace;
    }

    public Result CanPlaceDefOnMapSpace(MapSpace mapSpace, IDef def, MapSpot anchorSpot, RotationFlag rotation, Dictionary<string, object>? args = null)
    {
        // var compArgs = args;
        // if (compArgs == null)
        //     compArgs = new Dictionary<string, object>();
        //
        // var spots = new List<MapSpot>{anchorSpot};
        // // Let managers determine the full list of occupied spots
        // foreach (var manager in DimMaster.ListManagersOfType<IMapPlacementValidator>())
        // {
        //     var name = manager.GetType().FullName;
        //     var arg = compArgs.ContainsKey(name) ? compArgs[name] : null;
        //     var extraSpots = manager.GetSpotsForDef(mapSpace, def, anchorSpot, rotation, arg);
        //     foreach (var eSpot in extraSpots)
        //     {
        //         if(!spots.Contains(eSpot))
        //             spots.Add(eSpot);
        //     }
        // }
        //
        // // Check that all managers allow for all occupied spots spots
        // foreach (var manager in DimMaster.ListManagersOfType<IMapPlacementValidator>())
        // {
        //     var name = manager.GetType().FullName;
        //     var arg = compArgs.ContainsKey(name) ? compArgs[name] : null;
        //     var res = manager.CanPlaceDefOnMapSpace(mapSpace, def, anchorSpot, rotation, arg);
        //     if (!res.Success)
        //         return res;
        // }

        return new Result();
    }

}