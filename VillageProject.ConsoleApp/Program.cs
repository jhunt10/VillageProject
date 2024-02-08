// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using System.Text.Json.Nodes;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Map.Terrain;

Console.WriteLine("Hello, World!");

string GetDefPath()
{
    switch (Environment.UserName)
    {
        case "Jack":
            return @"C:\Users\Jack\Documents\Repos\VillageProject\Assets";
        case "johnnie.hunt":
            return @"C:\Users\johnnie.hunt\Documents\Personal\Repo\VillageProject\Assets\Defs";
    }

    throw new NotImplementedException($"No DefPath set for user: '{Environment.UserName}'");
}


var terrainDef = new Def
{
    DefName = "Defs.Terrain.Dirt",
    Label = "Dirt",
    CompDefs = new List<ICompDef>
    {
        new TerrainDef()
    }
};
SaveDef(terrainDef);

await DefMaster.LoadDefs(GetDefPath());

foreach (var def in DefMaster.Defs)
{
    var t = def;
}


void SaveDef(Def def)
{
    var savePath = BuildSavePath(def);
    var serialized = JsonSerializer.Serialize(def);
    Directory.CreateDirectory(Path.GetDirectoryName(savePath));
    File.WriteAllText(savePath, serialized);
}

string BuildSavePath(Def def)
{
    var tokens = def.DefName.Split(DefMaster.PATH_SEPERATOR);
    var relativePath = Path.Combine(tokens) + ".json";
    return Path.Combine(GetDefPath(), relativePath);
}