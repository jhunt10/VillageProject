using VillageProject.Core.DIM.Defs;

namespace VillageProject.Core.Sprites.PatchSprites;

public abstract class GenericPatchCellSpriteDef<TComp> : GenericCompDef<TComp, SpriteManager>, IPatchSpriteCompDef
    where TComp : BasePatchCellSpriteComp
{
    public string SpriteSheet { get; set; }
    public int SpriteWidth { get; set; }
    public int SpriteHight { get; set; }
}