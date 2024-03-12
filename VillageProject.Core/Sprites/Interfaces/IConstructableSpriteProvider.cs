using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapSpaces;

namespace VillageProject.Core.Sprites.Interfaces;

public interface IConstructableSpriteProvider : ISpriteComp
{
    public SpriteData GetConstructableIconSprite();
    public SpriteData GetConstructablePreviewSprite(IMapSpace mapSpace, MapSpot spot, RotationFlag rotation);
}