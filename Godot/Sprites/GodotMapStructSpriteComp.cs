using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Sprites;
using VillageProject.Core.Sprites.Interfaces;

namespace VillageProject.Godot.Sprites;

public class GodotMapStructSpriteComp : BaseSpriteComp, IMapStructureSpriteProvider, IConstructableSpriteProvider
{
    public GodotMapStructSpriteComp(ICompDef def, IInst inst) : base(def, inst)
    {
    }

    public override SpriteData GetSprite()
    {
        throw new System.NotImplementedException();
    }

    public SpriteData GetMapStructureSpriteForPosition(MapSpot spot, RotationFlag rotation)
    {
        throw new System.NotImplementedException();
    }

    public SpriteData GetConstructableIconSprite()
    {
        throw new System.NotImplementedException();
    }

    public SpriteData GetConstructablePreviewSprite(RotationFlag rotationFlag)
    {
        throw new System.NotImplementedException();
    }
}