using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Items.Interfaces;
using VillageProject.Core.Reservations;

namespace VillageProject.Core.Items;

public class ItemCompInst : BaseCompInst, IReservable
{
    public ItemCompDef ItemCompDef => CompDef as ItemCompDef ?? throw new Exception(
        $"Failed to cast ItemCompDef {CompDef.ParentDef.DefName}:{CompDef.CompKey} to ItemCompDef.");

    public int Count { get; private set; }
    
    private string _parentInventoryId;

    public ItemCompInst(ICompDef def, IInst inst) : base(def, inst)
    {
        
    }
    
    public InventoryCompInst? GetParentInventory()
    {
        if (string.IsNullOrEmpty(_parentInventoryId))
            return null;
        var keys = _parentInventoryId.Split(":");
        var inst = DimMaster.GetInstById(keys[0]);
        if (inst == null)
            throw new Exception($"Failed to find ParentInventory {_parentInventoryId} for Item {Instance._DebugId}.");
        var comp = inst.GetComponentWithKey<InventoryCompInst>(keys[1]);
        if (comp == null)
            throw new Exception($"Inst {inst._DebugId} does not have CompInst with key {_parentInventoryId}.");
        return comp;
    }
    
    private bool _settingInventory;
    public void SetInventory(InventoryCompInst? newInventory)
    {
        if(_settingInventory)
            return;
        
        try
        {
            _settingInventory = true;
            var curtInventory = GetParentInventory();
            if(curtInventory != null)
                curtInventory.RemoveItem(this);
            
            if (newInventory != null)
            {
                _parentInventoryId = newInventory.Id;
                var testInventory = GetParentInventory();
                if(testInventory == null)
                    throw new Exception($"Failed find inventory {newInventory.Id} after setting.");
                if (!testInventory.HasItem(this))
                {
                    var res = testInventory.TryAddItem(this);
                    if (!res.Success)
                        throw new Exception(
                            $"Failed to add {this.Instance._DebugId} to Inventory {newInventory.Id}");
                }
            }
            else
            {
                _parentInventoryId = "";
            }
        }
        finally
        {
            _settingInventory = false;
        }
    }

    public void SetStackCount(int count)
    {
        if(!ItemCompDef.CanStack)
            throw new Exception($"Item '{Id}' can not stack.");
        count = Count;
        if (count <= 0)
        {
            count = 0;
            this.Instance.Delete();
        }
    }
    
    public ItemCompInst SplitStack(int count)
    {
        if (!ItemCompDef.CanStack)
            throw new Exception($"Item '{Id}' can not stack.");
        if (count > Count || Count <= 0)
            throw new Exception(
                $"Invalid requested stack count. Item {Id} contains {Count} but {count} were requested.");
        // Requesting the whole stack is invalid. This Insts should have been taken instead.
        if (count == Count)
            throw new Exception("Invalid requested stack count. Whole stack was requested.");
        var newStack = DimMaster.InstantiateDef(Instance.Def);
        var newItemComp = newStack.GetComponentWithKey<ItemCompInst>(this.ItemCompDef.CompKey);
        if (newItemComp == null)
            throw new Exception("Failed to created new inst of own def.");
        this.SetStackCount(Count - count);
        newItemComp.SetStackCount(count);
        return newItemComp;
    }

    public bool CanMerge(ItemCompInst other)
    {
        var parentInventory = this.GetParentInventory();
        var otherInventory = other.GetParentInventory();
        // Must be in same inventory
        if (parentInventory == null || otherInventory == null 
            || parentInventory.Id != otherInventory.Id)
            return false;

        // Must be made from same def
        if (other.Instance.Def.DefName != this.Instance.Def.DefName)
            return false;

        // Check other components for approval
        foreach (var compareComp in this.Instance.ListComponentsOfType<IItemComparableComp>())
        {
            var res = compareComp.IsSameItem(other.Instance);
            if (!res.Success)
                return false;
        }

        return true;
    }
    
    public void MergeWithStack(ItemCompInst otherStack)
    {
        if (!ItemCompDef.CanStack)
            throw new Exception($"Item '{Id}' can not stack.");
        if (otherStack.Id == this.Id)
            throw new Exception("Can not merge with self.");
        if (!CanMerge(otherStack))
            throw new Exception("Can not merge with other stack.");
        
        this.SetStackCount(otherStack.Count);
        otherStack.SetStackCount(0);
    }

    public override void OnDeleteInst()
    {
        var parent = GetParentInventory();
        if (parent != null && parent.HasItem(this))
        {
            parent.RemoveItem(this);
        }
        base.OnDeleteInst();
    }

    // TODO: Real Reservations
    private string _reservationId;
    public Result TryReserve(Reservation reservation)
    {
        if (string.IsNullOrEmpty(_reservationId))
            return new Result(false, "Item is already reserved.");
        _reservationId = reservation.Id;
        return new Result(true);
    }

    public void EndReservation(Reservation reservation)
    {
        if (reservation.Id == _reservationId)
            _reservationId = "";
    }
}