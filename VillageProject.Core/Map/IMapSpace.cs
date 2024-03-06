using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.Map;

public interface IMapSpace
{
    public bool InBounds(MapSpot spot);
    public bool InBounds(int x, int y, int z);
    public IEnumerable<MapSpot> EnumerateMapSpots();

    public IEnumerable<IInst>? ListInstsAtSpot(MapSpot spot, string? layer = null);
    
    public Result TryAddInstToSpots(IInst inst, List<MapSpot> spot, string layer);

    public void RemoveInst(IInst inst);


    public DataDict BuildSaveData();
}