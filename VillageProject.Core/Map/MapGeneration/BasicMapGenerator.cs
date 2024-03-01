using VillageProject.Core.DIM;
using VillageProject.Core.Enums;
using VillageProject.Core.Map.Terrain;

namespace VillageProject.Core.Map.MapGeneration;

public static class BasicMapGenerator
{
    public static MapSpace GenerateTestMap()
    {
        var maxX = 3;
        var minX = -3;
        var maxY = 3;
        var minY = -3;
        var maxZ = 3;
        var minZ = -3;

        var mapSpace = new MapSpace(minX, maxX, minY, maxY, minZ, maxZ);
        var mapManager = DimMaster.GetManager<MapManager>();
        var terrainManager = DimMaster.GetManager<TerrainManager>();
        var terrains = terrainManager._terrainInsts.Values.ToList();
		
        for(int x = minX; x <= maxX; x++)
        for (int y = minY; y <= maxY; y++)
        {
            var index = 0;
            if (x + y > 0)
                index = 1;
            var terrainInst = terrains[index];
            var z = 0;
            var res = mapManager.TryPlaceInstOnMapSpace(mapSpace, terrainInst, new MapSpot(x, y, z), RotationFlag.North);
            if (!res.Success)
                throw new Exception($"Failed placing terrain in map generation: {res.Message}.");
        }

        return mapSpace;
    }
}