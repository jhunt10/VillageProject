using System.Collections.Generic;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapStructures;
using VillageProject.Core.Map.MapStructures.Constructables;
using VillageProject.Core.Map.Pathing;
using VillageProject.Core.Map.Terrain;
using VillageProject.Godot.Sprites;

namespace VillageProject.Godot.DefDefs.DefPrefabs;

public static class TerrainDefs
{
    public static IDef Dirt = new ObjectDef
    {
        DefName = "Defs.MapStructures.Terrain.Dirt",
        Label = "Dirt",
        CompDefs = new List<ICompDef>
        {
            new TerrainCompDef
            {
                
            },
            new MapStructCompDef
            {
                MapLayer = MapStructureManager.DEFAULT_MAP_LAYER,
                OccupationData = new OccupationData(new Dictionary<MapSpot, List<OccupationFlags>>
                {
                    {
                        new MapSpot(0, 0, 0), new List<OccupationFlags>
                        {
                            OccupationFlags.Full
                        }
                    }
                })

            },
            new GodotPatchCellSpriteDef()
            {
                CompKey = "TopSprite",
                SpriteSheet = "dirt_tiles.png",
                SpriteWidth = 32,
                SpriteHight = 32
            },
            new GodotPatchCellSpriteDef()
            {
                CompKey = "FrontSprite",
                SpriteSheet = "dirt_tiles_front.png",
                SpriteWidth = 32,
                SpriteHight = 40
            },
            new PathingCompDef()
            {
                PathSpotDefs = new Dictionary<MapSpot, PathSpotDef>
                {
                    {new MapSpot(0,0,0), new PathSpotDef(){SupportsPathOver = 
                        CellSideFlags.BackLeft | CellSideFlags.Back | CellSideFlags.BackRight |
                        CellSideFlags.Left | CellSideFlags.Right |
                        CellSideFlags.FrontLeft | CellSideFlags.Front | CellSideFlags.FrontRight
                    }} 
                }
            }
        },
    };
    
    public static IDef Stone = new ObjectDef
    {
        DefName = "Defs.MapStructures.Terrain.Stone",
        Label = "Stone",
        CompDefs = new List<ICompDef>
        {
            new TerrainCompDef
            {
                
            },
            new MapStructCompDef
            {
                MapLayer = MapStructureManager.DEFAULT_MAP_LAYER,
                OccupationData = new OccupationData(new Dictionary<MapSpot, List<OccupationFlags>>
                {
                    {
                        new MapSpot(0, 0, 0), new List<OccupationFlags>
                        {
                            OccupationFlags.Full
                        }
                    }
                })

            },
            new GodotPatchCellSpriteDef()
            {
                CompKey = "TopSprite",
                SpriteSheet = "stone_tiles.png",
                SpriteWidth = 32,
                SpriteHight = 32
            },
            new GodotPatchCellSpriteDef()
            {
                CompKey = "FrontSprite",
                SpriteSheet = "stone_tiles_front.png",
                SpriteWidth = 32,
                SpriteHight = 40
            },
            new PathingCompDef()
            {
                PathSpotDefs = new Dictionary<MapSpot, PathSpotDef>
                {
                    {new MapSpot(0,0,0), new PathSpotDef(){SupportsPathOver = 
                        CellSideFlags.BackLeft | CellSideFlags.Back | CellSideFlags.BackRight |
                        CellSideFlags.Left | CellSideFlags.Right |
                        CellSideFlags.FrontLeft | CellSideFlags.Front | CellSideFlags.FrontRight
                    }} 
                }
            }
        },
    };
    
    public static Dictionary<string, IDef> Defs = new Dictionary<string, IDef>
    {
        {
            "Dirt", Dirt
        },
        {
            "Stone", Stone
        }
    };
}