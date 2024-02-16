using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.Sprites;

public abstract class BaseSpriteComp : BaseCompInst, ISpriteComp
{
    protected BaseSpriteComp(ICompDef def, IInst inst) : base(def, inst)
    {
        
    }
    public abstract SpriteData GetSprite();


}