// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapGeneration;
using VillageProject.Core.Map.MapStructures;
using VillageProject.Core.Map.MapStructures.Constructables;
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

// MakeAndSaveNewDef();


DimMaster.StartUp();

var fullOcc = OccupationFlags.Inner | OccupationFlags.Middle | OccupationFlags.Outer | OccupationFlags.Top | OccupationFlags.Center | OccupationFlags.Bottom 
              | OccupationFlags.TopBack | OccupationFlags.TopRight | OccupationFlags.TopFront | OccupationFlags.TopLeft | OccupationFlags.TopBackLeft 
              | OccupationFlags.TopBackRight | OccupationFlags.TopFrontRight | OccupationFlags.TopFrontLeft | OccupationFlags.Back | OccupationFlags.Right 
              | OccupationFlags.Front | OccupationFlags.Left | OccupationFlags.BackLeft | OccupationFlags.BackRight | OccupationFlags.FrontRight 
              | OccupationFlags.FrontLeft | OccupationFlags.BottomBack | OccupationFlags.BottomRight | OccupationFlags.BottomFront | OccupationFlags.BottomLeft 
              | OccupationFlags.BottomBackLeft | OccupationFlags.BottomBackRight | OccupationFlags.BottomFrontRight | OccupationFlags.BottomFrontLeft;

var mapSpace = BasicMapGenerator.GenerateTestMap();

var t = true;

// void MakeAndSaveNewDef()
// {
//     var defs = FurnitureDefs.Defs.Values;
//     foreach (var d in defs)
//     {
//         SaveDef(d);
//     }
//     
// }
//
//
// void SaveDef(IDef def)
// {
//     var savePath = BuildSavePath(def);
//     var serialized = JsonSerializer.Serialize(def);
//     Directory.CreateDirectory(Path.GetDirectoryName(savePath));
//     File.WriteAllText(savePath, serialized);
// }
//
// string BuildSavePath(IDef def)
// {
//     var tokens = def.DefName.Split(DimMaster.PATH_SEPERATOR);
//     var relativePath = Path.Combine(tokens) + ".json";
//     return Path.Combine(GetDefPath(), relativePath);
// }