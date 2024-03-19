﻿using System.Collections.Generic;
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
                            OccupationFlags.Inner | OccupationFlags.Center | OccupationFlags.Bottom | OccupationFlags.Front |
                            OccupationFlags.BottomFront
                        }
                    },

                    {
                        new MapSpot(0, -1, 0), new List<OccupationFlags>
                        {
                            OccupationFlags.Inner | OccupationFlags.Center | OccupationFlags.Bottom | OccupationFlags.Back |
                            OccupationFlags.BottomBack
                        }
                    }
                })

            },
            new ConstructableCompDef()
            {
                IconSprite  = new SpriteDataDef("BedIcon.png", 64, 64, 0, 0),
                DefaultSprite = new SpriteDataDef("BedNorth.png", 32, 64, 0, -(32+40)),
                RotationSprites = new Dictionary<RotationFlag, SpriteDataDef>
                {
                    { RotationFlag.North, new SpriteDataDef("BedNorth.png", 32, 64, 0, -(32+40))},
                    { RotationFlag.East, new SpriteDataDef("BedEast.png", 32, 64, -32, -72)},
                    { RotationFlag.South, new SpriteDataDef("BedSouth.png", 32, 64, 0, -104)},
                    { RotationFlag.West, new SpriteDataDef("BedWest.png", 32, 64, 0, -72)},
                }
            },
            new GodotMapStructSpriteCompDef()
            {
                DefaultSprite = new SpriteDataDef("BedNorth.png", 32, 64, 0, -(32+40)),
                RotationSprites = new Dictionary<RotationFlag, SpriteDataDef>
                {
                    { RotationFlag.North, new SpriteDataDef("BedNorth.png", 32, 64, 0, -(32+40))},
                    { RotationFlag.West, new SpriteDataDef("BedEast.png", 32, 64, -32, -72)},
                    { RotationFlag.South, new SpriteDataDef("BedSouth.png", 32, 64, 0, -104)},
                    { RotationFlag.East, new SpriteDataDef("BedWest.png", 32, 64, 0, -72)},
                }
            }
        }
    };
    
    public static IDef FlowerBed = new Def
    {
        DefName = "Defs.MapStructures.Furniture.FlowerBed",
        Label = "FlowerBed",
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
                            OccupationFlags.Middle | OccupationFlags.Center
                        }
                    }
                })

            },
            new ConstructableCompDef()
            {
                IconSprite = new SpriteDataDef("FlowerBedIcon.png", 32, 32, 0, -32),
                DefaultSprite = new SpriteDataDef("FlowerBedIcon.png", 32, 32, 0, -32),
                RotationSprites = new Dictionary<RotationFlag, SpriteDataDef>
                {
                    { RotationFlag.North, new SpriteDataDef("FlowerBedIcon.png", 32, 32, 0, -32)},
                    { RotationFlag.East, new SpriteDataDef("FlowerBedIcon.png", 32, 32, 0, -32)},
                    { RotationFlag.South, new SpriteDataDef("FlowerBedIcon.png", 32, 32, 0, -32)},
                    { RotationFlag.West, new SpriteDataDef("FlowerBedIcon.png", 32, 32, 0, -32)},
                }
            },
            new GodotPatchCellSpriteDef()
            {
                SpriteHight = 32,
                SpriteWidth = 32,
                SpriteSheet = "planter_tiles.png"
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
        }
    };
    
    
    public static Dictionary<string, IDef> Defs = new Dictionary<string, IDef>
    {
        { "Bed", Bed },
        {"FlowerBed", FlowerBed }
    };
}
