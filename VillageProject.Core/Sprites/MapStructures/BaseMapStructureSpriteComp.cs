using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapSpaces;

namespace VillageProject.Core.Sprites.MapStructures;

public abstract class BaseMapStructureSpriteComp : BaseSpriteComp, IMapPlacementWatcherComp
{
    public BaseMapStructureSpriteComp(ICompDef def, IInst inst) : base(def, inst)
    {
        
    }

    public void MapPositionSet(IMapSpace mapSpaceCompInst, MapSpot mapSpot, RotationFlag rotation)
    {
        DirtySprite();
    }
}