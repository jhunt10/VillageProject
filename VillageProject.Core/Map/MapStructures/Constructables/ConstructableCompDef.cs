using VillageProject.Core.DIM.Defs;

namespace VillageProject.Core.Map.MapStructures.Constructables;

public class ConstructableCompDef : GenericCompDef<ConstructableCompInst, ConstructableManager>
{
    public string IconSprite { get; set; }
}