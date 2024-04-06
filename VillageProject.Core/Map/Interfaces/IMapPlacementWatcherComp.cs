using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map.MapSpaces;

namespace VillageProject.Core.Map;

/// <summary>
/// Interface to be applied to components which need updating when map position changes
/// </summary>
public interface IMapPlacementWatcherComp : ICompInst
{
    public void MapPositionSet(MapPositionData? mapPos);
}