using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Godot.DefDefs.DefPrefabs;

namespace VillageProject.Godot.DefDefs;

public static class DefWriter
{
    public static string GetDefPath()
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
    
    public static void SaveAllDefs()
    {
        var defs = MapDefs.Defs.Values.ToList();
        defs.AddRange(TerrainDefs.Defs.Values.ToList());
        defs.AddRange(MapStructureDefs.Defs.Values.ToList());
        defs.AddRange(BehaviorDefs.Defs.Values.ToList());
        foreach (var def in defs)
        {
            if (def == null)
                throw new Exception("What?");
            SaveDef(def);
        }
    }
    
    static void SaveDef(IDef def)
    {
        var savePath = BuildSavePath(def);
        var serialized = JsonSerializer.Serialize(def);
        Directory.CreateDirectory(Path.GetDirectoryName(savePath));
        File.WriteAllText(savePath, serialized);
    }

    static string BuildSavePath(IDef def)
    {
        var tokens = def.DefName.Split(DimMaster.PATH_SEPERATOR).ToList();
        tokens.Add(tokens.Last());
        var relativePath = Path.Combine(tokens.ToArray()) + ".json";
        return Path.Combine(GetDefPath(), relativePath);
    }
}