using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;

namespace VillageProject.Core.Map.MapStructures;

public interface IMapStructureManager : IManager, IMapPlacementValidator
{
    // public void RemoveMapStructure(IInst inst);
    //
    // public bool CanPlaceMapStructure(IInst inst, MapSpot spot, RotationFlag rotation);
    // public bool TryPlaceMapStructure(IInst inst, MapSpot spot, RotationFlag rotation);
    // public IEnumerable<IInst> ListInstsAtSpot(MapSpot spot, string? layer = null);

    public IInst CreateMapStructureFromDef(IDef def, MapSpot spot, RotationFlag rotation,
        Dictionary<string, object>? otherArgs = null);

    // public AdjacencyFlags GetAdjacency(
    //     IInst inst,
    //     IMapSpace mapSpace,
    //     MapSpot spot,
    //     RotationFlag rotation = RotationFlag.North,
    //     AdjacencyHelper.InstAdjacencyMatch? matchDelegate = null);
}