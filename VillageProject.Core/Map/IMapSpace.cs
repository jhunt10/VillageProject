using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.Map;

public interface IMapSpace
{
    public bool InBounds(MapSpot spot);
    public bool InBounds(int x, int y, int z);
    public IEnumerable<MapSpot> EnumerateMapSpots();

    public IInst? GetTerrainAtSpot(MapSpot spot);
    public void SetTerrainAtSpot(IInst terrain, MapSpot spot);
}