using VillageProject.Core.DIM.Defs;

namespace VillageProject.Core.Sprites.PatchSprites;

public interface IPatchSpriteCompDef: ICompDef
{
    public string SpriteSheet { get; }
    public int SpriteWidth { get; }
    public int SpriteHight { get; }
}