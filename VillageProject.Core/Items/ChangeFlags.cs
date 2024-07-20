namespace VillageProject.Core.Items;

public static class ItemChangeFlags
{
    /// <summary>
    /// Change when this item is place in a new parent inventory
    /// </summary>
    public const string ParentInventoryChange = "ItemPar";
}

public static class InventoryChangeFlags
{

    /// <summary>
    /// Change when this inventory add or removes an item
    /// </summary>
    public const string HeldItemsChange = "InvHeld";
}