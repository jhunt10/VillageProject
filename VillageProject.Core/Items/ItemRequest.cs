using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Filters;

namespace VillageProject.Core.Items;

public class ItemRequest
{
    public DefFilter Filter { get; }
    public int Count { get; }
    public bool AllowPartialFill { get; }
    public bool ForceInventoryProvide { get; }
    public string Label
    {
        get
        {
            if (ExactItem != null)
                return ExactItem.Instance._DebugId;
            else if (Count == 1)
                return Filter.Label;
            else
                return $"{Filter.Label} ({Count})";
        }
    }

    private string _targetItemId;
    private string _sourceInventoryId;
    private string _destinationInventoryId;

    public ItemCompInst? ExactItem
    {
        get
        {
            if (string.IsNullOrEmpty(_targetItemId))
                return null;
            var inst = DimMaster.GetInstById(_targetItemId);
            if (inst == null)
                throw new Exception($"Failed to find Inst with id '{_targetItemId}'.");
            var itemComp = inst.GetComponentOfType<ItemCompInst>(activeOnly:false);
            if (itemComp == null)
                throw new Exception($"Inst '{inst._DebugId}' does not contain a {nameof(ItemCompInst)}.");
            return itemComp;
        }
    }

    
    
    
    /// <summary>
    /// Create a request for a specific ItemCompInst.
    /// </summary>
    /// <param name="itemComp">ItemCompInst of item to request</param>
    /// <param name="count">Number of the item to request</param>
    /// <param name="allowPartialFill">Allow the request to be fulfilled by fewer items than request</param>
    /// <param name="forceInventoryProvide">Take from inventories that wouldn't normally provide items</param>
    public ItemRequest(ItemCompInst itemComp, int count = 1,  bool allowPartialFill = false, bool forceInventoryProvide = false)
    {
        if (itemComp == null) throw new ArgumentNullException(nameof(itemComp));
        Filter = new DefFilter(new DefFilterDef()
        {
            AlowedPaths = new List<string> { itemComp.Instance.Def.DefName },
            RequiredCompTypes = new List<string>{typeof(ItemCompInst).FullName},
        });
        Count = count;
        AllowPartialFill = false;
        ForceInventoryProvide = false;
        _targetItemId = itemComp.Instance.Id;
    }
    
    /// <summary>
    /// Create a request for items of a given Def.
    /// </summary>
    /// <param name="itemDef">Def of item to request</param>
    /// <param name="count">Number of the item to request</param>
    /// <param name="allowPartialFill">Allow the request to be fulfilled by fewer items than request</param>
    /// <param name="forceInventoryProvide">Take from inventories that wouldn't normally provide items</param>
    public ItemRequest(IDef itemDef, int count = 1,  bool allowPartialFill = false, bool forceInventoryProvide = false)
    {
        if (itemDef == null) throw new ArgumentNullException(nameof(itemDef));
        Filter = new DefFilter(new DefFilterDef()
        {
            AlowedPaths = new List<string> { itemDef.DefName },
            RequiredCompTypes = new List<string>{typeof(ItemCompInst).FullName},
        });
        Count = count;
        AllowPartialFill = false;
        ForceInventoryProvide = false;
    }

    /// <summary>
    /// Create a request for items allowed by a given DefFilter.
    /// </summary>
    /// <param name="itemDef">Def of item to request</param>
    /// <param name="count">Number of the item to request</param>
    /// <param name="allowPartialFill">Allow the request to be fulfilled by fewer items than request</param>
    /// <param name="forceInventoryProvide">Take from inventories that wouldn't normally provide items</param>
    public ItemRequest(DefFilter itemFilter, int count = 1,  bool allowPartialFill = false, bool forceInventoryProvide = false)
    {
        Filter = itemFilter ?? throw new ArgumentNullException(nameof(itemFilter));
        Count = count;
        AllowPartialFill = allowPartialFill;
        ForceInventoryProvide = forceInventoryProvide;
    }
}