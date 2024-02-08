using System.Text.Json;

namespace VillageProject.Core.DIM.Defs;

public class DefMaster
{
    public static char PATH_SEPERATOR = '.';
    public static List<IDef> Defs;

    public static async Task LoadDefs(string defPath)
    {
        if (Defs == null)
            Defs = new List<IDef>();

        var defFiles = Directory.EnumerateFiles(defPath, "*.json", SearchOption.AllDirectories).ToList();
        foreach (var defFile in defFiles)
        {
            var def = await _loadDefFromFile(defFile);
            Defs.Add(def);
        }
    }

    private static async Task<IDef> _loadDefFromFile(string filePath)
    {
        var text = await File.ReadAllTextAsync(filePath);
        var def = JsonSerializer.Deserialize<Def>(text);
        return def;
    }
}