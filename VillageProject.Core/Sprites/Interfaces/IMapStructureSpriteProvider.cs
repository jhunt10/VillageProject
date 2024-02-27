using VillageProject.Core.Enums;
using VillageProject.Core.Map;

namespace VillageProject.Core.Sprites;

public interface IMapStructureSpriteProvider
{
    public SpriteData GetMapStructureSpriteForPosition(MapSpot spot, RotationFlag rotation);
}