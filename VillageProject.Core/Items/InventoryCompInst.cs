using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.Items;

public class InventoryCompInst : BaseCompInst
{
    public InventoryCompDef InventoryCompDef => CompDef as InventoryCompDef ?? throw new Exception(
        $"Failed to cast InventoryCompDef {CompDef.ParentDef.DefName}:{CompDef.CompKey} to InventoryCompDef.");

    public string InventoryId { get; private set; }
    private List<string> _holdingItems = new List<string>();

    public InventoryCompInst(ICompDef def, IInst inst) : base(def, inst)
    {
        InventoryId = inst.Id + ":" + def.CompKey;
    }

    public Result TryAddItems(ItemCompInst item, bool allowPartial)
    {
        if (_holdingItems.Contains(item.Instance.Id))
            return new Result(true, "Already added");
        // TODO: Item limit logic
        _holdingItems.Add(item.Instance.Id);
        item.SetInventory(this);
        return new Result(true);
    }

    public void RemoveItem(ItemCompInst item)
    {
        if (!_holdingItems.Contains(item.Instance.Id))
            return;
        _holdingItems.Remove(item.Instance.Id);
        item.SetInventory(null);
    }
}