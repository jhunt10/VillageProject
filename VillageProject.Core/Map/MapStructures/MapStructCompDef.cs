using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Enums;

namespace VillageProject.Core.Map.MapStructures;

public class MapStructCompDef : GenericCompDef<MapStructCompInst, MapStructureManager>
{
    public string MapLayer { get; set; }
    public OccupationData OccupationData { get; set; }
}