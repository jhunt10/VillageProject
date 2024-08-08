using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Sprites.Interfaces;

namespace VillageProject.Core.Sprites;

public class SpriteManager : BaseManager
{
    private static ISpriteLoader _spriteLoader;

    public static void SetSpriteLoader(ISpriteLoader loader)
    {
        _spriteLoader = loader;
    }

    public SpriteData LoadSprite(IDef sourceDef, SpriteDataDef spriteDef)
    {
        if (_spriteLoader == null)
            throw new Exception("SpriteLoader not Loaded");
        return _spriteLoader.LoadSprite(sourceDef, spriteDef);
    }
}