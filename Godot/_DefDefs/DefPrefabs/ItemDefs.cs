using VillageProject.Core.Behavior;
using VillageProject.Core.Behavior.CommonBehaviors;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Filters;
using VillageProject.Core.Enums;
using VillageProject.Core.Items;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapStructures;
using VillageProject.Core.Sprites;
using VillageProject.Godot.Actors;
using VillageProject.Godot.InstNodes;

namespace VillageProject.Godot.DefDefs.DefPrefabs;

public static class ItemDefs
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
            },
            new InstNodeCompDef()
            {
                PrefabNodeName = "item_pile_node.tscn"
            }
        }
    };
    
    public static IDef Apple = new ObjectDef()
    {
        DefName = "Defs.Items.Apple",
        Label = "Apple",
        CompDefs = new List<ICompDef>()
        {
            new ItemCompDef()
            {
                CanStack = true,
                Mass = 1,
                Volume = 1,
                BaseValue = 1,
                ItemSpriteDef = new SpriteDataDef("apple_test_sprite.png", 32, 32, 0, 0)
            }
        }
    };
    
    public static Dictionary<string, IDef> Defs = new Dictionary<string, IDef>
    {
        {"ItemPile", ItemPileDef},
        {
            "Apple", Apple
        },
    };
}