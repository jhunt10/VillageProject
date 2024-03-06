using System.Collections.Generic;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapStructures;
using VillageProject.Core.Map.MapStructures.Constructables;
using VillageProject.Core.Sprites;
using VillageProject.Godot.Sprites;

namespace VillageProject.Godot.DefDefs.DefPrefabs;

public class FurnitureDefs
{

    public static IDef Bed = new Def
    {
        DefName = "Defs.MapStructures.Furniture.Bed",
        Label = "Bed",
        CompDefs = new System.Collections.Generic.List<ICompDef>
        {
            new MapStructCompDef
            {
                MapLayer = MapStructureManager.DEFAULT_MAP_LAYER,
                OccupationData = new OccupationData(new Dictionary<MapSpot, List<OccupationFlags>>
                {
                    {
                        new MapSpot(0, 0, 0), new List<OccupationFlags>
                        {
                            OccupationFlags.Center | OccupationFlags.Bottom | OccupationFlags.Front |
                            OccupationFlags.BottomFront
                        }
                    },

                    {
                        new MapSpot(0, -1, 0), new List<OccupationFlags>
                        {
                            OccupationFlags.Center | OccupationFlags.Bottom | OccupationFlags.Back |
                            OccupationFlags.BottomBack
                        }
                    }
                })

            },
            new ConstructableCompDef()
            {
                IconSprite = "BedIcon.png"
            },
            new ConstructableSpriteProviderCompDef()
            {
                PreviewSprites = new Dictionary<RotationFlag, SpriteDataDef>
                {
                    { RotationFlag.North, new SpriteDataDef("BedNorth.png", 32, 64, 0, -(32+40))},
                    { RotationFlag.East, new SpriteDataDef("BedEast.png", 32, 64, 0, 32)},
                    { RotationFlag.South, new SpriteDataDef("BedSouth.png", 32, 64, 0, 32)},
                    { RotationFlag.West, new SpriteDataDef("BedWest.png", 32, 64, 0, 32)},
                }
            }
        }
    };

    public static IDef Wall = new Def
    {
        DefName = "Defs.MapStructures.Furniture.Wall",
        Label = "Wall",
        CompDefs = new System.Collections.Generic.List<ICompDef>
        {
            new MapStructCompDef
            {
                MapLayer = MapStructureManager.DEFAULT_MAP_LAYER,
                OccupationData = new OccupationData(new Dictionary<MapSpot, List<OccupationFlags>>
                {
                    {
                        new MapSpot(0, 0, 0), new List<OccupationFlags>
                        {
                            OccupationFlags.Outer | OccupationFlags.Back
                        }
                    },

                    {
                        new MapSpot(0, 1, 0), new List<OccupationFlags>
                        {
                            OccupationFlags.Outer | OccupationFlags.Front
                        }
                    }
                })

            },
            new ConstructableCompDef()
            {
                IconSprite = "Wall.png"
            }
        }
    };
    
    
    public static Dictionary<string, IDef> Defs = new Dictionary<string, IDef>
    {
        {
            "Bed", Bed
        }
    };
}

