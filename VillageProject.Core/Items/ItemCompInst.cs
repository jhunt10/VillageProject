using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.Items;

public class ItemCompInst : BaseCompInst
{
    public ItemCompDef ItemCompDef => CompDef as ItemCompDef ?? throw new Exception(
        $"Failed to cast ItemCompDef {CompDef.ParentDef.DefName}:{CompDef.CompKey} to ItemCompDef.");

    private string _parentInventoryId;
    public InventoryCompInst? ParentInventory
    {
        get
        {
            if (!string.IsNullOrEmpty(_parentInventoryId))
            {
                var keys = _parentInventoryId.Split(":");
                var inst = DimMaster.GetInstById(keys[0]);
                if (inst == null)
                    throw new Exception($"Failed to find ParentInventory {_parentInventoryId} for Item {Instance._DebugId}.");
                var comp = inst.GetComponentWithKey<InventoryCompInst>(keys[1]);
                if (comp == null)
                    throw new Exception($"Inst {inst._DebugId} does not have CompInst with key {_parentInventoryId}.");
                return comp;
            }

            return null;
        }
    }

    public ItemCompInst(ICompDef def, IInst inst) : base(def, inst)
    {
        
    }

    private bool _settingInventory;
    public void SetInventory(InventoryCompInst? newInventory)
    {
        if(_settingInventory)
            return;
        
        try
        {
            _settingInventory = true;
            var curtInventory = ParentInventory;
            if(curtInventory != null)
                curtInventory.RemoveItem(this);
            
            if (newInventory != null)
            {
                _parentInventoryId = newInventory.InventoryId;
                var testInventory = ParentInventory;
                var res = testInventory.TryAddItems(this, false);
                if (!res.Success)
                    throw new Exception(
                        $"Failed to add {this.Instance._DebugId} to Inventory {newInventory.InventoryId}");
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
}