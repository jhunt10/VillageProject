using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapSpaces;

namespace VillageProject.Core.Sprites.Actors;

public abstract class BaseActorSpriteComp : BaseSpriteComp, IMapPlacementWatcherComp
{
    public BaseActorSpriteComp(ICompDef def, IInst inst) : base(def, inst)
    {
        
    }

    public void MapPositionSet(MapPositionData? mapPos)
    {
        DirtySprite();
    }
}