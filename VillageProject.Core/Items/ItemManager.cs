using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Filters;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.Items;

public class ItemManager : BaseManager
{
    private List<string> _itemInstIds = new List<string>();
    private List<string> _inventoryInstIds = new List<string>();

    public override ICompInst CreateCompInst(ICompDef compDef, IInst newInst, object? args)
    {
        var compInst = base.CreateCompInst(compDef, newInst, args);
        // New comp is an item
        if (compDef is ItemCompDef)
        {
            _itemInstIds.Add(newInst.Id);
        }
        // New comp is an inventory
        if (compDef is InventoryCompDef)
        {
            _inventoryInstIds.Add(newInst.Id);
        }

        return compInst;
    }

    public override void OnInstDelete(IInst inst)
    {
        if (_itemInstIds.Contains(inst.Id))
        {
            _itemInstIds.Remove(inst.Id);
        }
        if (_inventoryInstIds.Contains(inst.Id))
        {
            _inventoryInstIds.Remove(inst.Id);
        }
        base.OnInstDelete(inst);
    }

    public Result CanTransferItem(ItemCompInst itemInst, InventoryCompInst targetInventoryInst, int? subCount = null)
    {
        var sourceInventory = itemInst.GetParentInventory();
        if (sourceInventory != null)
        {
            if (sourceInventory.Id == targetInventoryInst.Id)
                return new Result(false, "Item already in target inventory.");
        }

        if (subCount.HasValue)
        {
            var subValue = subCount.Value;
            if(subValue < 0)
                return new Result(false, $"Can not transfer a negative substack of {subValue}.");
            if(subValue == 0)
                return new Result(false, "Can not transfer a substack of 0.");
            if(subValue > itemInst.Count)
                return new Result(false, $"SubCount {subValue} is greater than item count of {itemInst.Count}.");
        }

        return targetInventoryInst.CanAddItem(itemInst, subCount);
    }

    public Result TryTransferringItems(ItemCompInst itemInst, InventoryCompInst targetInventory, int? subCount = null)
    {
        var canRes = CanTransferItem(itemInst, targetInventory, subCount);
        if (!canRes.Success)
            return canRes;

        var transItem = itemInst;
        
        var sourceInventory = itemInst.GetParentInventory();
        if (sourceInventory != null)
        {
            transItem = sourceInventory.RemoveItem(itemInst, subCount);
        }

        var res = targetInventory.TryAddItem(transItem);
        if (!res.Success)
        {
            // TODO: Reverse transfer?
        }

        return res;
    }

    public IEnumerable<ItemCompInst> FindItemsForFilter(DefFilter filter, InventoryCompInst? inventory = null)
    {
        throw new NotImplementedException();
    }
}