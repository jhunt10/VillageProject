using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map.MapSpaces;

namespace VillageProject.Core.Map;

/// <summary>
/// Manager interface to validate if an inst can be placed at a certain MapSpot
/// </summary>
public interface IMapPlacementValidator : IManager
{
    public Result CouldPlaceDefOnMapSpace(IMapSpace mapSpace, IDef Def, MapSpot anchorSpot, RotationFlag rotation, object args);
    public Result CanPlaceInstOnMapSpace(IMapSpace mapSpace, IInst inst, MapSpot anchorSpot, RotationFlag rotation, object args);
    public Result TryPlaceInstOnMapSpace(IMapSpace mapSpace, IInst inst, MapSpot anchorSpot, RotationFlag rotation, object args);
}