using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;

namespace VillageProject.Core.Sprites.PatchSprites;

public abstract class BasePatchCellSpriteComp : BaseSpriteComp
{
    public const string SPRITE_KEY = "full_sprite";
    
    public BasePatchCellSpriteComp(ICompDef def, IInst inst) : base(def, inst)
    {
        
    }

    protected abstract SpriteData GetSubSprite(int x, int y);

    public SpriteData GetPatchSprite(Func<AdjacencyFlags> getAdj)
    {;
        var atlasCo = getAdj().ToAtlasCo();
        return GetSubSprite(atlasCo[0], atlasCo[1]);
    }
}