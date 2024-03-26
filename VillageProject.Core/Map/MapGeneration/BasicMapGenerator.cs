using VillageProject.Core.DIM;
using VillageProject.Core.Enums;
using VillageProject.Core.Map.MapSpaces;
using VillageProject.Core.Map.Terrain;

namespace VillageProject.Core.Map.MapGeneration;

public static class BasicMapGenerator
{
    public static IMapSpace GenerateTestMap()
    {
        var def = DimMaster.GetDefByName("Defs.MapSpaces.Testing.LargerTest");
        var mapInst = DimMaster.InstantiateDef(def);
        var mapSpace = mapInst.GetComponentOfType<MapSpaceCompInst>();
        var mapManager = DimMaster.GetManager<MapManager>();
        var terrainManager = DimMaster.GetManager<TerrainManager>();
        var terrains = terrainManager._terrainInsts.Values.Select(x => x.Def).ToList();
		
        for(int x = mapSpace.MinX; x <= mapSpace.MaxX; x++)
        for (int y = mapSpace.MinY; y <= mapSpace.MaxY; y++)
        {
            var index = 0;
            if (x + y > 0)
                index = 1;
            var terrainDef = terrains[index];
            var terrainInst = DimMaster.InstantiateDef(terrainDef);
            for (int z = mapSpace.MinZ; z <= 0; z++)
            {
                var res = mapManager.TryPlaceInstOnMapSpace(
                    mapSpace, terrainInst, new MapSpot(x, y, z),
                    RotationFlag.North);
                if (!res.Success)
                    throw new Exception($"Failed placing terrain in map generation: {res.Message}.");
            }
        }

        return mapSpace;
    }
}