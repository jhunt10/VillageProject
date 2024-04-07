using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.Items;

public class ItemManager : BaseManager
{
    private List<string> _itemInstIds = new List<string>();
    private List<string> _inventoryInstIds = new List<string>();

    public override ICompInst CreateCompInst(ICompDef compDef, IInst newInst, object? args)
    {
        if (compDef.CompKey.Contains(':'))
            throw new Exception(
                $"Invalid CompKey {compDef.CompKey} on inst {newInst._DebugId}. Can not contain \":\".");
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
}