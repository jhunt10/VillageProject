using VillageProject.Core.DIM.Defs;

namespace VillageProject.Core.Map.MapSpaces;

public class MapSpaceCompDef : GenericCompDef<MapSpaceCompInst, MapManager>
{
    public int MinX { get; set; }
    public int MaxX { get; set; }
    public int MinY { get; set; }
    public int MaxY { get; set; }
    public int MinZ { get; set; }
    public int MaxZ { get; set; }
}