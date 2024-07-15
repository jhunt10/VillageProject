using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Filters;

namespace VillageProject.Core.Items;

public class InventoryCompDef : GenericCompDef<InventoryCompInst, ItemManager>
{
    public DefFilterDef ItemFilter { get; set; }
    public float? MaxMass { get; set; }
    public float? MaxVolume { get; set; }
}