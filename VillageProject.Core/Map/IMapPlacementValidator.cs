using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;

namespace VillageProject.Core.Map;

/// <summary>
/// Manager interface to validate if an inst can be placed at a certain MapSpot
/// </summary>
public interface IMapPlacementValidator : IManager
{
    // public List<MapSpot> GetSpotsForDef(MapSpace space, IDef def, MapSpot anchorSpot, RotationFlag rotation, object args);
    public Result CanPlaceDefOnMapSpace(MapSpace space, IDef def, MapSpot anchorSpot, RotationFlag rotation, object args);
}