using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Filters;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapStructures;
using VillageProject.Core.Map.MapStructures.Constructables;
using VillageProject.Core.Sprites;

namespace VillageProject.Core.Items.ItemPile;

public static class ItemPileDefProvider
{
    public static ObjectDef ItemPileDef = new ObjectDef()
    {
        DefName = "Defs.Static.ItemPile",
        Label = "Item Pile",
        CompDefs = new System.Collections.Generic.List<ICompDef>
        {
            new MapStructCompDef
            {
                MapLayer = MapStructureManager.DEFAULT_MAP_LAYER,
                OccupationData = new OccupationData(new Dictionary<MapSpot, List<OccupationFlags>>())

            },
            new InventoryCompDef
            {
                MaxMass = null,
                MaxVolume = null,
                ItemFilter = new DefFilterDef
                {
                    AlowedPaths = new List<string>{"Defs.Items"}
                }
            }
        }
    };
}