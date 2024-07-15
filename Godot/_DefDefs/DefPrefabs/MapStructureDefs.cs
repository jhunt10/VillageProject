using System.Collections.Generic;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapStructures;
using VillageProject.Core.Map.MapStructures.Constructables;
using VillageProject.Core.Map.Pathing;
using VillageProject.Core.Sprites;
using VillageProject.Godot.Sprites;

namespace VillageProject.Godot.DefDefs.DefPrefabs;

public class MapStructureDefs
{

    public static IDef Bed = new ObjectDef
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
                            OccupationFlags.BottomFront,
                            OccupationFlags.Middle | OccupationFlags.Front,
                            OccupationFlags.Outer | OccupationFlags.Front,
                            
                        }
                    },

                    {
                        new MapSpot(0, -1, 0), new List<OccupationFlags>
                        {
                            OccupationFlags.Inner | OccupationFlags.Center | OccupationFlags.Bottom | OccupationFlags.Back |
                            OccupationFlags.BottomBack,
                            OccupationFlags.Middle | OccupationFlags.Back,
                            OccupationFlags.Outer | OccupationFlags.Back,
                        }
                    }
                })

            },
            new ConstructableCompDef()
            {
                IconSprite  = new SpriteDataDef("BedIcon.png", 64, 64, 0, 0),
                DefaultSprite = new SpriteDataDef("BedNorth.png", 32, 104, 0, 32),
                RotationSprites = new Dictionary<RotationFlag, SpriteDataDef>
                {
                    { RotationFlag.North, new SpriteDataDef("BedNorth.png", 32, 104, 0, 32)},
                    { RotationFlag.East, new SpriteDataDef("BedEast.png", 64, 72, -32, 0)},
                    { RotationFlag.South, new SpriteDataDef("BedSouth.png", 32, 104, 0, 0)},
                    { RotationFlag.West, new SpriteDataDef("BedWest.png", 64, 72, 0, 0)},
                }
            },
            new GodotMapStructSpriteCompDef()
            {
                DefaultSprite = new SpriteDataDef("BedNorth.png", 32, 104, 0, 32),
                RotationSprites = new Dictionary<RotationFlag, SpriteDataDef>
                {
                    { RotationFlag.North, new SpriteDataDef("BedNorth.png", 32, 104, 0, 32)},
                    { RotationFlag.East, new SpriteDataDef("BedEast.png", 64, 72, -32, 0)},
                    { RotationFlag.South, new SpriteDataDef("BedSouth.png", 32, 104, 0, 0)},
                    { RotationFlag.West, new SpriteDataDef("BedWest.png", 64, 72, 0, 0)},
                }
            }
        }
    };
    
    public static IDef FlowerBed = new ObjectDef
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

    public static IDef BrickWall = new ObjectDef
    {
        DefName = "Defs.MapStructures.Walls.BrickWall",
        Label = "BrickWall",
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
                    }
                })

            },
            new ConstructableCompDef()
            {
                IconSprite  = new SpriteDataDef("BrickWallIcon.png", 64, 64, 0, 0),
                DefaultSprite = new SpriteDataDef("BrickWallNorth.png", 32, 72, 0, 0),
                RotationSprites = new Dictionary<RotationFlag, SpriteDataDef>
                {
                    { RotationFlag.North, new SpriteDataDef("BrickWallNorth.png", 32, 72, 0, 0)},
                    { RotationFlag.East, new SpriteDataDef("BrickWallEast.png", 32, 72, 0, 0)},
                    { RotationFlag.South, new SpriteDataDef("BrickWallSouth.png", 32, 72, 0, 0)},
                    { RotationFlag.West, new SpriteDataDef("BrickWallWest.png", 32, 72, 0, 0)},
                }
            },
            new GodotMapStructSpriteCompDef()
            {
                DefaultSprite = new SpriteDataDef("BrickWallNorth.png", 32, 72, 0, 0),
                RotationSprites = new Dictionary<RotationFlag, SpriteDataDef>
                {
                    { RotationFlag.North, new SpriteDataDef("BrickWallNorth.png", 32, 72, 0, 0)},
                    { RotationFlag.East, new SpriteDataDef("BrickWallEast.png", 32, 72, 0, 0)},
                    { RotationFlag.South, new SpriteDataDef("BrickWallSouth.png", 32, 72, 0, 0)},
                    { RotationFlag.West, new SpriteDataDef("BrickWallWest.png", 32, 72, 0, 0)},
                }
            },
            new PathingCompDef()
            {
                PathSpotDefs = new Dictionary<MapSpot, PathSpotDef>
                {
                    {new MapSpot(0,0,0), new PathSpotDef(){
                        BlocksPathThrough = CellSideFlags.BackLeft | CellSideFlags.Back | CellSideFlags.BackRight
                    }} 
                }
            }
        }
    };
    
    
    public static Dictionary<string, IDef> Defs = new Dictionary<string, IDef>
    {
        { "Bed", Bed },
        {"FlowerBed", FlowerBed },
        {"BrickWall", BrickWall}
    };
}

