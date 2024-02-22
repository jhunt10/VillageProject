using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Enums;

namespace VillageProject.Core.Map.MapStructures;

public class MapStructCompDef : BaseCompDef<MapStructCompInst, MapStructureManager>
{
    public string MapLayer { get; set; }
    public Dictionary<MapSpot, OccupationFlags[]> FootPrint { get; set; }
}