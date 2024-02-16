﻿// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using System.Text.Json.Nodes;
using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Map;
using VillageProject.Core.Map.Terrain;

Console.WriteLine("Hello, World!");

string GetDefPath()
{
    switch (Environment.UserName)
    {
        case "Jack":
            return @"C:\Users\Jack\Documents\Repos\VillageProject\Assets";
        case "johnnie.hunt":
            return @"C:\Users\johnnie.hunt\Documents\Personal\Repo\VillageProject\Assets";
    }

    throw new NotImplementedException($"No DefPath set for user: '{Environment.UserName}'");
}


var mapSpot = new MapSpot(0, 0, 0);
foreach (var adj in mapSpot.ListAdjacentSpots())
{
    Console.WriteLine($"{adj.Key}: {adj.Value}");
}

// var terrainDef = new Def
// {
//     DefName = "Defs.Terrain.Dirt",
//     Label = "Dirt",
//     CompDefs = new List<ICompDef>
//     {
//         new TerrainDef(),
//         
//     }
// };
// SaveDef(terrainDef);
//
// DimMaster.StartUp();
// var terrainManager = DimMaster.GetManager<TerrainManager>();
//
//
// var defSearch = DimMaster.GetAllDefsWithCompDefType<TerrainDef>();
//
// foreach (var inst in terrainManager._terrainInsts)
// {
//     var t = inst;
// }


void SaveDef(Def def)
{
    var savePath = BuildSavePath(def);
    var serialized = JsonSerializer.Serialize(def);
    Directory.CreateDirectory(Path.GetDirectoryName(savePath));
    File.WriteAllText(savePath, serialized);
}

string BuildSavePath(Def def)
{
    var tokens = def.DefName.Split(DimMaster.PATH_SEPERATOR);
    var relativePath = Path.Combine(tokens) + ".json";
    return Path.Combine(GetDefPath(), relativePath);
}