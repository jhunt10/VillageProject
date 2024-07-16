using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Filters;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapSpaces;
using VillageProject.Core.Map.MapStructures;

namespace VillageProject.Core.Items;

public static class ItemHelper
{
    public static void DropItemOnMap(ItemCompInst itemComp, MapSpot spot, IMapSpace mapSpace)
    {
        var itemPileDef = DimMaster.GetDefByPartialName("ItemPile");
        var mapManager = DimMaster.GetManager<MapManager>();
        var exitingItemPile = mapSpace.ListInstsAtSpot(spot).FirstOrDefault(x => x.Def.DefName == itemPileDef.DefName);
        if (exitingItemPile != null)
        {
            var res = exitingItemPile.GetComponentOfType<InventoryCompInst>()?.TryAddItem(itemComp);
            if (res == null)
                Console.Error.Write("Failed to find InventoryCompInst on ItemPile");
            else if(!res.Success)
                Console.Error.WriteLine($"Failed to drop item in existing pile: {res.Message}.");
            return;
        }

        var newPile = DimMaster.InstantiateDef(itemPileDef);
        var mapStructComp = newPile.GetComponentOfType<MapStructCompInst>(activeOnly:false);

        if (mapStructComp == null)
        {
            Console.Error.Write("Failed to find MapStructCompInst on ItemPile");
            return;
        }

        var res2 = mapStructComp.TrySetMapPosition(new MapPositionData(mapSpace, spot, RotationFlag.North));
        if(!res2.Success)
            Console.Error.WriteLine($"Failed to drop item on map: {res2.Message}.");
        else
        {
            var res = newPile.GetComponentOfType<InventoryCompInst>()?.TryAddItem(itemComp);
            if (res == null)
                Console.Error.Write("Failed to find InventoryCompInst on ItemPile");
            else if(!res.Success)
                Console.Error.WriteLine($"Failed to drop item in new  pile: {res.Message}.");
        }
    }
    
    
}