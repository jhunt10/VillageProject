using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Map.Terrain;

namespace VillageProject.Core.Map;

public class MapSpace : IMapSpace
{
    private class MapCell
    {
        public int? TerrainIndex { get; set; }
    }
    
    public int MinX { get; private set; }
    public int MaxX { get; private set; }
    public int MinY { get; private set; }
    public int MaxY { get; private set; }
    public int MinZ { get; private set; }
    public int MaxZ { get; private set; }

    /// <summary>
    /// Matrix if cells ordered in [z][x][y]
    /// </summary>
    private MapCell[][][] _cellMatrix;

    private Dictionary<int, string> _terrainLibrary = new Dictionary<int, string>();

    public void _buildCellMatrix(int maxX, int minX, int maxY, int minY, int maxZ, int minZ)
    {
        MaxX = maxX; MinX = minX;
        MaxY = maxY; MinY = minY;
        MaxZ = maxZ; MinZ = minZ;
        var width = maxX - minX + 1;
        var depth = maxY - minY + 1;
        var hight = maxZ - minZ + 1;
        _cellMatrix = new MapCell[hight][][];
        for (int z = 0; z < hight; z++)
        {
            _cellMatrix[z] = new MapCell[width][];
            for (int x = 0; x < width; x++)
            {
                _cellMatrix[z][x] = new MapCell[depth];
                for (int y = 0; y < depth; y++)
                    _cellMatrix[z][x][y] = new MapCell();
            }
        }
    }

    private MapCell? _getCellAtSpot(MapSpot spot)
    {
        if (!InBounds(spot))
            return null;
        return _cellMatrix[spot.Z - MinZ][spot.X - MinX][spot.Y - MinY];
    }
    
    public bool InBounds(MapSpot spot)
    {
        return InBounds(spot.X, spot.Y, spot.Z);
    }

    public bool InBounds(int x, int y, int z)
    {
        return (x >= MinX && y >= MinY && z >= MinZ &&
                x <= MaxX && y <= MaxY && z <= MaxZ);
    }

    public IEnumerable<MapSpot> EnumerateMapSpots()
    {
        for(int z = MinZ; z <= MaxZ; z++)
        for(int x = MinX; x <= MaxX; x++)
        for (int y = MinY; y <= MaxY; y++)
            yield return new MapSpot(x, y, z);
    }

    public IInst? GetTerrainAtSpot(MapSpot spot)
    {
        var cell = _getCellAtSpot(spot);
        if (cell == null)
            return null;
        var terrainIndex = cell.TerrainIndex;
        if (!terrainIndex.HasValue)
            return null;

        if (!_terrainLibrary.ContainsKey(terrainIndex.Value))
            throw new Exception($"Unknown terrain index {terrainIndex.Value} at spot {spot}.");

        var terrainName = _terrainLibrary[terrainIndex.Value];
        var terrainManager = DimMaster.GetManager<TerrainManager>();
        var terrainInst = terrainManager.GetTerrainByName(terrainName);
        if(terrainInst == null)
            throw new Exception($"No terrain found with name {terrainName}.");
        return terrainInst;
    }

    public void SetTerrainAtSpot(IInst terrain, MapSpot spot)
    {
        var terrainName = terrain.Def.DefName;
        var index = -1;
        var existing = _terrainLibrary.Where(x => x.Value == terrainName).ToList();
        if (existing.Count > 1)
            throw new Exception($"Multiple indexes found for terrain named '{terrainName}'.");
        if (existing.Count == 1)
            index = existing.First().Key;
        else
        {
            index = _terrainLibrary.Count;
            _terrainLibrary.Add(index, terrainName);
        }

        var cell = _getCellAtSpot(spot);
        if(cell == null)
            return;
        cell.TerrainIndex = index;
    }
}