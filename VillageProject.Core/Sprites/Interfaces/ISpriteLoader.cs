using VillageProject.Core.DIM.Defs;

namespace VillageProject.Core.Sprites.Interfaces;

public interface ISpriteLoader
{
    SpriteData LoadSprite(IDef sourceDef, SpriteDataDef spriteDef);
}