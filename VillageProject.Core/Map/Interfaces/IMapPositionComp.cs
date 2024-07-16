using VillageProject.Core.DIM;

namespace VillageProject.Core.Map;

public interface IMapPositionComp
{
    public string? MapSpaceId { get; }
    public MapSpot? MapSpot { get; }
    public Result TrySetMapPosition(MapPositionData mapPos);
}