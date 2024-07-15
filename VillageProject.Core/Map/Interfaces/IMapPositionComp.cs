using VillageProject.Core.DIM;

namespace VillageProject.Core.Map;

public interface IMapPositionComp
{
    public Result TrySetMapPosition(MapPositionData mapPos);
}