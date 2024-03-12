using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.Map.MapSpaces;

public interface IMapSpace
{
    public string MapSpaceId { get; }
    public int MinX { get; }
    public int MaxX { get; }
    public int MinY { get; }
    public int MaxY { get; }
    public int MinZ { get; }
    public int MaxZ { get; }
    
    public bool InBounds(MapSpot spot);
    public bool InBounds(int x, int y, int z);
    public IEnumerable<MapSpot> EnumerateMapSpots();

    public IEnumerable<IInst> ListInstsAtSpot(MapSpot spot, string? layer = null);
    
    public Result TryAddInstToSpots(IInst inst, List<MapSpot> spot, string layer);

    public void RemoveInst(IInst inst);


    public DataDict BuildSaveData();
}