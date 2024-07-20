using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Filters;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.Items;

public class InventoryCompInst : BaseCompInst
{
    public InventoryCompDef InventoryCompDef => CompDef as InventoryCompDef ?? throw new Exception(
        $"Failed to cast InventoryCompDef {CompDef.ParentDef.DefName}:{CompDef.CompKey} to InventoryCompDef.");

    private List<string> _holdingItems = new List<string>();
    private DefFilter _filter;

    public InventoryCompInst(ICompDef def, IInst inst) : base(def, inst)
    {
        var compDef = def as InventoryCompDef;
        if (compDef == null)
            throw new Exception(
                $"Failed to case CompDef '{def.ParentDef.DefName}:{def.CompKey}' to {nameof(InventoryCompDef)}.");
        if(!compDef.ItemFilter.RequiredCompTypes.Contains(typeof(ItemCompInst).FullName))
            compDef.ItemFilter.RequiredCompTypes.Add(typeof(ItemCompInst).FullName);
        _filter = new DefFilter(compDef.ItemFilter);
    }

    public IEnumerable<ItemCompInst> ListHeldItems()
    {
        foreach (var item_id in _holdingItems)
        {
            var item = DimMaster.GetCompAsTypeById<ItemCompInst>(item_id, errorIfNull:true);
            yield return item;
        }
    }

    public bool HasItem(ItemCompInst item)
    {
        return _holdingItems.Contains(item.Id);
    }

    public Result CanAddItem(ItemCompInst item, int? subCount = null)
    {
        // TODO: Item limit logic
        return new Result(true);
    }

    public Result TryAddItem(ItemCompInst item)
    {
        if (_holdingItems.Contains(item.Id))
            return new Result(true, "Already added");
        
        var res = CanAddItem(item, item.Count);
        if (!res.Success)
            return res;

        // Check if it can merge with any held item.
        var foundMerge = false;
        foreach (var itemComp in _enumerateHeldItemComp())
        {
            if (itemComp.CanMerge(item))
            {
                itemComp.MergeWithStack(item);
                foundMerge = true;
                break;
            }
        }
        if(!foundMerge)
        {
            // Couldn't merge with any held items.
            _holdingItems.Add(item.Id);
            item.SetInventory(this);
        }
        Instance.FlagWatchedChange(InventoryChangeFlags.HeldItemsChange);
        return new Result(true);
    }

    /// <summary>
    /// Remove the given item from this inventory. If no subCount is provided, the entire item is returned.
    /// If a subCount is provided, a stack is split off from the item and that stack is returned.
    /// </summary>
    /// <param name="item">IItem Inst to remove</param>
    /// <param name="subCount">Number of the item to remove. -1 = All</param>
    /// <returns>The item removed, or a new stack split from the requested item.</returns>
    public ItemCompInst RemoveItem(ItemCompInst item, int? subCount = null)
    {
        if (!_holdingItems.Contains(item.Instance.Id))
            throw new Exception($"Inventory '{this.Id}' does not contain '{item.Id}'.");
        
        if (subCount.HasValue && (subCount.Value > item.Count || subCount.Value <= 0))
                throw new Exception(
                    $"Invalid requested stack count. Item {Id} contains {item.Count} but {subCount} were requested.");

        // Removing whole stack
        if (!subCount.HasValue || subCount.Value == item.Count)
        {
            _holdingItems.Remove(item.Instance.Id);
            item.SetInventory(null);
            return item;
        }
        
        // Splitting Stack
        var newStack = item.SplitStack(subCount.Value);
        newStack.SetInventory(this);
        return newStack;
    }

    public override void OnDeleteInst()
    {
        foreach (var itemComp in _enumerateHeldItemComp())
        {
            // TODO: Drop on ground
            itemComp.SetInventory(null);
        }
        base.OnDeleteInst();
    }

    private IEnumerable<ItemCompInst> _enumerateHeldItemComp()
    {
        var holdingIds = _holdingItems.ToList();
        foreach (var holdingId in _holdingItems)
        {
            var holdingInst = DimMaster.GetInstById(holdingId);
            var itemComp = holdingInst?.GetComponentOfType<ItemCompInst>();
            if (itemComp == null)
            {
                var foundId = holdingInst?.Id ?? "No Inst";
                Console.WriteLine($"Failed to find itemComp from id '{holdingId}'. Found IInst '{foundId}'.");
                _holdingItems.Remove(holdingId);
                continue;
            }

            yield return itemComp;
        }
    }
}