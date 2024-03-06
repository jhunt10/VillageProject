using VillageProject.Core.Enums;
using VillageProject.Core.Map;

namespace VillageProject.Core.Sprites.Interfaces;

public interface IConstructableSpriteProvider : ISpriteComp
{
    public SpriteData GetConstructableIconSprite();
    public SpriteData GetConstructablePreviewSprite(MapSpace mapSpace, MapSpot spot, RotationFlag rotation);
}