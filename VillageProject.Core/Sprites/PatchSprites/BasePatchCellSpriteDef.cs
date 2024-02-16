using VillageProject.Core.DIM.Defs;

namespace VillageProject.Core.Sprites.PatchSprites;

public abstract class BasePatchCellSpriteDef<TComp> : BaseCompDef<TComp, SpriteManager>, IPatchSpriteCompDef
    where TComp : BasePatchCellSpriteComp
{
    public string SpriteSheet { get; set; }
    public int SpriteWidth { get; set; }
    public int SpriteHight { get; set; }
}