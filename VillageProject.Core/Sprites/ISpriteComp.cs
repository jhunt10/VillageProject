using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Sprites.Interfaces;

namespace VillageProject.Core.Sprites;

public interface ISpriteComp : ICompInst
{
    /// <summary>
    /// Flag sprite to be updated on next process cycle
    /// </summary>
    public void DirtySprite();
    
    /// <summary>
    /// Return the current sprite
    /// </summary>
    public SpriteData GetSprite();
    
}