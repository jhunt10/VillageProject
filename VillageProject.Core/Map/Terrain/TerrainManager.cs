using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map.MapSpaces;
using VillageProject.Core.Map.MapStructures;

namespace VillageProject.Core.Map.Terrain;


public enum TerrainAdjacency
{
    None = 0,
    Match = 1,
    Different = 2
}
/// <summary>
/// Terrain is treated more like a Concept than a Thing since we don't create an instance for each individual block. 
/// </summary>
public class TerrainManager : BaseManager
{
    public const string TERRAIN_LAYER = MapStructureManager.DEFAULT_MAP_LAYER;
    // public Dictionary<string, IInst> _terrainInsts;
    
    public TerrainManager()
    {
        // _terrainInsts = new Dictionary<string, IInst>();
    }

    public override void Init()
    {
        // LoadDefsToInsts();
        base.Init();
    }
    
    // private void LoadDefsToInsts()
    // {
    //     var terrainDefs = DimMaster.GetAllDefsWithCompDefType<TerrainCompDef>();
    //     foreach (var def in terrainDefs)
    //     {
    //         var inst = DimMaster.InstantiateDef(def);
    //         _terrainInsts.Add(def.DefName, inst);
    //     }
    // }

    // public IInst? GetTerrainByName(string name)
    // {
    //     if (_terrainInsts.ContainsKey(name))
    //         return _terrainInsts[name];
    //     return null;
    // }

    public IInst? GetTerrainAtSpot(IMapSpace mapSpace, MapSpot spot)
    {
        var insts = mapSpace.ListInstsAtSpot(spot, TERRAIN_LAYER)?.FirstOrDefault();
        return insts;
    }

    public CellSideFlags GetHorizontalAdjacency(
        IMapSpace mapSpace, 
        MapSpot spot, 
        RotationFlag rotation = RotationFlag.North, 
        bool matchAny = false)
    {
        var terrain = GetTerrainAtSpot(mapSpace, spot);
        var adjacency = CellSideFlags.None;
        if (terrain == null)
            return (CellSideFlags) (0);

        // var topSpot = spot.DirectionToSpot(DirectionFlags.Top);
        // var topTerrain = mapSpace.GetTerrainAtSpot(topSpot);
        // if (topTerrain != null)
        //     return (AdjacencyFlags) (-1);
        
        foreach (var adjPair in spot.ListAdjacentSpots(rotation))
        {
            var direction = adjPair.Key;
            var adjSpot = adjPair.Value;
            if (((direction & DirectionFlag.Top) == DirectionFlag.Top)
                || ((direction & DirectionFlag.Bottom) == DirectionFlag.Bottom))
            {
                // Console.WriteLine($"Ignoring direction: {direction}");
                continue;   
            }
            // Console.WriteLine($"Checking direction: {direction}, {adjSpot}");
                
            var terrainAtSpot = GetTerrainAtSpot(mapSpace, adjSpot);
            if(terrainAtSpot == null)
                continue;
            if (terrainAtSpot.Id == terrain.Id || matchAny)
            {
                var adjFlag = direction.ToCellSide();
                
                // Console.WriteLine($"Before adding {adjFlag} to {adjacency}");
                adjacency = adjacency | adjFlag;
                // Console.WriteLine($"After adding {adjFlag} to {adjacency}");
            }
        }

        return adjacency;
    }
    
    /// <summary>
    /// Returns adjacency for front of block, but translated to horizontal adjacency for PatchSprites
    /// </summary>
    public CellSideFlags GetVerticalAdjacencyAsHorizontal(IMapSpace mapSpace, MapSpot spot, RotationFlag rotation = RotationFlag.North, bool matchAny = false)
    {
        var terrain = GetTerrainAtSpot(mapSpace, spot);
        var adjacency = CellSideFlags.None;
        if (terrain == null)
            return (CellSideFlags) (-1);

        var frontSpace = spot.DirectionToSpot(DirectionFlag.Front, rotation);
        var frontTerrain = GetTerrainAtSpot(mapSpace, frontSpace);
        if (frontTerrain != null)
            return (CellSideFlags) (-1);
        
        foreach (var adjPair in spot.ListAdjacentSpots(rotation))
        {
            var direction = adjPair.Key;
            var adjSpot = adjPair.Value;
            if (((direction & DirectionFlag.Front) == DirectionFlag.Front)
                || ((direction & DirectionFlag.Back) == DirectionFlag.Back))
            {
                // Console.WriteLine($"Ignoring direction: {direction}");
                continue;   
            }
            // Console.WriteLine($"Checking direction: {direction}, {adjSpot}");
                
            var terrainAtSpot = GetTerrainAtSpot(mapSpace, adjSpot);
            if(terrainAtSpot == null)
                continue;
            if (terrainAtSpot.Id == terrain.Id || matchAny)
            {
                var adjFlag = direction.ToCellSide();
                switch (direction)
                {
                    case DirectionFlag.TopLeft:
                        adjFlag = CellSideFlags.BackLeft;
                        break;
                    case DirectionFlag.Top:
                        adjFlag = CellSideFlags.Back;
                        break;
                    case DirectionFlag.TopRight:
                        adjFlag = CellSideFlags.BackRight;
                        break;
                    case DirectionFlag.BottomLeft:
                        adjFlag = CellSideFlags.FrontLeft;
                        break;
                    case DirectionFlag.Bottom:
                        // If it's got a block to the bottom front of it, show no adj bottom
                        var botFrontSpot = spot.DirectionToSpot(DirectionFlag.BottomFront);
                        var botFrontTerrain =  GetTerrainAtSpot(mapSpace, botFrontSpot);
                        if(botFrontTerrain != null && (botFrontTerrain.Def.DefName == terrain.Def.DefName || matchAny))
                            adjFlag = CellSideFlags.None;
                        else
                            adjFlag = CellSideFlags.Front;
                            
                        break;
                    case DirectionFlag.BottomRight:
                        adjFlag = CellSideFlags.FrontRight;
                        break;
                        
                }
                
                
                
                // Console.WriteLine($"Before adding {adjFlag} to {adjacency}");
                adjacency = adjacency | adjFlag;
                // Console.WriteLine($"After adding {adjFlag} to {adjacency}");
            }
        }

        return adjacency;
    }

}