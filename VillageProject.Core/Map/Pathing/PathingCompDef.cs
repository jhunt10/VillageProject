using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Enums;

namespace VillageProject.Core.Map.Pathing;

public class PathSpotDef
{
    public CellSideFlags BlocksPathThrough { get; set; }
    public CellSideFlags SupportsPathThrough { get; set; }
    public CellSideFlags SupportsPathOver { get; set; }
    
    public int PathThroughCost { get; set; }
    public int PathOverCost { get; set; }
}

public class PathBridgeDef
{
    public MapSpot SpotA { get; set; }
    public MapSpot SpotB { get; set; }
    public int PathCost { get; set; }
}

public class PathingCompDef : GenericCompDef<PathingCompInst, PathingManager>, ICompDef
{
    public Dictionary<MapSpot, PathSpotDef> PathSpotDefs { get; set; }
    public List<PathBridgeDef> PathBridgeDefs { get; set; }
}