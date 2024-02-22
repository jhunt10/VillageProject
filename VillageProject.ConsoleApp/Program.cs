// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using System.Text.Json.Nodes;
using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapStructures;
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


// var mapSpot = new MapSpot(0, 0, 0);
// foreach (var adj in mapSpot.ListAdjacentSpots())
// {
//     Console.WriteLine($"{adj.Key}: {adj.Value}");
// }

var fakeGrassDef = new Def
{
    DefName = "Defs.MapStructures.Decorations.FakeGrass",
    Label = "Grass",
    CompDefs = new List<ICompDef>
    {
        new MapStructCompDef
        {
            MapLayer = "Default",
            FootPrint = new Dictionary<MapSpot, OccupationFlags[]>
            {
                { new MapSpot(0, 0, 0), new OccupationFlags[]{OccupationFlags.None }}
            }
        }
    }
};
// SaveDef(fakeGrassDef);

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
var ocupation = OccupationFlags.TopLeft | OccupationFlags.FrontLeft | OccupationFlags.BottomRight;
var occ2 = ocupation.Rotate(RotationDirection.Clockwise);
var occ3 = ocupation.Rotate(RotationDirection.HalfTurn);
var occ4 = ocupation.Rotate(RotationDirection.CounterClockwise);

DimMaster.StartUp();
// var mapStructManager = DimMaster.GetManager<MapStructureManager>();
// var def = DimMaster.GetAllDefsWithCompDefType<MapStructCompDef>().First();
// mapStructManager.CreateMapStructureFromDef(def, new MapSpot(0, 0, 0), RotationFlag.North);

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