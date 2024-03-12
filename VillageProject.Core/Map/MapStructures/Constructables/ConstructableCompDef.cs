using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Enums;
using VillageProject.Core.Sprites;

namespace VillageProject.Core.Map.MapStructures.Constructables;

public class ConstructableCompDef : GenericCompDef<ConstructableCompInst, ConstructableManager>
{
    public SpriteDataDef IconSprite { get; set; }
    public SpriteDataDef DefaultSprite { get; set; }
    public Dictionary<RotationFlag, SpriteDataDef> RotationSprites { get; set; }
}