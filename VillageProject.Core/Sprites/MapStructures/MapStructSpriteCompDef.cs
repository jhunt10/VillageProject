using VillageProject.Core.Enums;

namespace VillageProject.Core.Sprites.MapStructures;

public class MapStructSpriteCompDef : BaseSpriteCompDef<MapStructSpriteCompInst>
{
    public SpriteDataDef DefaultSprite;
    public SpriteDataDef DefaultShortSprite;
    public SpriteDataDef DefaultShadowSprite;
    public Dictionary<RotationFlag, SpriteDataDef> RotationSprites { get; set; }
    public Dictionary<RotationFlag, SpriteDataDef> RotationShortSprites { get; set; }
    public Dictionary<RotationFlag, SpriteDataDef> RotationShadowSprites { get; set; }
    
}