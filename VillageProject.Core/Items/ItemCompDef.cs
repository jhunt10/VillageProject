using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;

namespace VillageProject.Core.Items;

public class ItemCompDef : GenericCompDef<ItemCompInst, ItemManager>
{
    public bool CanStack { get; set; }
    public decimal Mass { get; set; }
    public decimal Volume { get; set; }
    public decimal BaseValue { get; set; }
}