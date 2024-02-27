using VillageProject.Core.Enums;

namespace VillageProject.Core.Sprites.Interfaces;

public interface IConstructableSpriteProvider : ISpriteComp
{
    public SpriteData GetConstructableIconSprite();
    public SpriteData GetConstructablePreviewSprite(RotationFlag rotationFlag);
}