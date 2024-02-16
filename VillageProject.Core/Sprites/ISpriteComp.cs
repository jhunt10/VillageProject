using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.Sprites;

public interface ISpriteComp : ICompInst
{
    SpriteData GetSprite();
}