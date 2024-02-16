using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Insts;

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
    public Dictionary<string, IInst> _terrainInsts;
    
    public TerrainManager()
    {
        _terrainInsts = new Dictionary<string, IInst>();
    }

    public override void Init()
    {
        LoadDefsToInsts();
        base.Init();
    }
    
    private void LoadDefsToInsts()
    {
        var terrainDefs = DimMaster.GetAllDefsWithCompDefType<TerrainDef>();
        foreach (var def in terrainDefs)
        {
            var inst = DimMaster.InstantiateDef(def);
            _terrainInsts.Add(def.DefName, inst);
        }
    }

    public IInst? GetTerrainByName(string name)
    {
        if (_terrainInsts.ContainsKey(name))
            return _terrainInsts[name];
        return null;
    }

    // public IInst? GetTerrainAtSpot(IMapSpace mapSpace, MapSpot spot)
    // {
    //     if (!TerrainMap.ContainsKey(spot))
    //         return null;
    //     var terrainName = TerrainMap[spot];
    //     var inst = _terrainInsts[terrainName];
    //     return inst;
    // }

    public AdjacencyFlags GetHorizontalAdjacency(IMapSpace mapSpace, MapSpot spot)
    {
        var terrain = mapSpace.GetTerrainAtSpot(spot);
        var adjacency = AdjacencyFlags.None;
        if (terrain == null)
            return adjacency;
        
        foreach (var adjPair in spot.ListAdjacentSpots())
        {
            var direction = adjPair.Key;
            var adjSpot = adjPair.Value;
            if (((direction & DirectionFlags.Top) == DirectionFlags.Top)
                || ((direction & DirectionFlags.Bottom) == DirectionFlags.Bottom))
            {
                // Console.WriteLine($"Ignoring direction: {direction}");
                continue;   
            }
            // Console.WriteLine($"Checking direction: {direction}, {adjSpot}");
                
            var terrainAtSpot = mapSpace.GetTerrainAtSpot(adjSpot);
            if(terrainAtSpot == null)
                continue;
            if (terrainAtSpot.Id == terrain.Id)
            {
                var adjFlag = MapHelper.DirectionToAdjacency(direction);
                
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
    public AdjacencyFlags GetVerticalAdjacencyAsHorizontal(MapSpace mapSpace, MapSpot spot)
    {
        var terrain = mapSpace.GetTerrainAtSpot(spot);
        var adjacency = AdjacencyFlags.None;
        if (terrain == null)
            return adjacency;
        
        foreach (var adjPair in spot.ListAdjacentSpots())
        {
            var direction = adjPair.Key;
            var adjSpot = adjPair.Value;
            if (((direction & DirectionFlags.Front) == DirectionFlags.Front)
                || ((direction & DirectionFlags.Back) == DirectionFlags.Back))
            {
                // Console.WriteLine($"Ignoring direction: {direction}");
                continue;   
            }
            // Console.WriteLine($"Checking direction: {direction}, {adjSpot}");
                
            var terrainAtSpot = mapSpace.GetTerrainAtSpot(adjSpot);
            if(terrainAtSpot == null)
                continue;
            if (terrainAtSpot.Id == terrain.Id)
            {
                var adjFlag = MapHelper.DirectionToAdjacency(direction);
                switch (direction)
                {
                    case DirectionFlags.TopLeft:
                        adjFlag = AdjacencyFlags.BackLeft;
                        break;
                    case DirectionFlags.Top:
                        adjFlag = AdjacencyFlags.Back;
                        break;
                    case DirectionFlags.TopRight:
                        adjFlag = AdjacencyFlags.BackRight;
                        break;
                    case DirectionFlags.BottomLeft:
                        adjFlag = AdjacencyFlags.FrontLeft;
                        break;
                    case DirectionFlags.Bottom:
                        adjFlag = AdjacencyFlags.Front;
                        break;
                    case DirectionFlags.BottomRight:
                        adjFlag = AdjacencyFlags.FrontRight;
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