using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.Map;

public interface IMapSpace
{
    public bool InBounds(MapSpot spot);
    public bool InBounds(int x, int y, int z);
    public IEnumerable<MapSpot> EnumerateMapSpots();

    public IEnumerable<IInst>? ListInstsAtSpot(MapSpot spot, string? layer = null);
    
    public Result TryAddInstToSpot(MapSpot spot, string layer, IInst inst);
    public Result TryAddInstsToSpot(List<MapSpot> spot, string layer, IInst inst);

    public void RemoveInstFromSpot(MapSpot spot, IInst inst);
    public void RemoveInstFromSpots(MapSpot spot, IInst inst);
}