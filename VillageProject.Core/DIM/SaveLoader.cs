using System.Text.Json;

namespace VillageProject.Core.DIM;

public class SaveLoader
{
    private string _saveDirPath;

    public SaveLoader(string saveDirectory)
    {
        _saveDirPath = saveDirectory;
    }
    
    public void SaveGameState(string saveName)
    {
        var savePath = Path.Join(_saveDirPath, "Saves");
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);
        
        var fullSavePath = Path.Join(savePath, saveName + ".json");
        if(File.Exists(fullSavePath))
            File.Delete(fullSavePath);

        var saveDict = new Dictionary<string, object>();
        foreach (var manager in DimMaster.Managers.Values)
        {
            var saveData = manager.BuildSaveData();
            if (saveData == null)
                continue;
            saveDict.Add("MAN:"+manager.GetType().FullName, saveData);
        }

        foreach (var inst in DimMaster.ListAllInsts())
        {
            var saveData = inst.BuildSaveData();
            var key = "INST:" + inst.Def.DefName + ":" + inst.Id;
            saveDict.Add(key, saveData);
        }
        File.WriteAllText(fullSavePath, JsonSerializer.Serialize(saveDict));
    }

    public void LoadGameState(string saveName)
    {
        var fullSavePath = Path.Join(_saveDirPath, "Saves", saveName + ".json");
        if (!File.Exists(fullSavePath))
            throw new Exception($"Failed to find save file at '{fullSavePath}'.");
        
        var text = File.ReadAllText(fullSavePath);
        var saveDict = JsonSerializer.Deserialize<Dictionary<string, DataDict>>(text);
        foreach (var pair in saveDict)
        {
            var key = pair.Key;
            var tokens = key.Split(':');
            var typeKey = tokens[0];
            var typeName = tokens[1];
            
            var saveData = pair.Value;

            if (typeKey == "MAN")
            {
                var manager = DimMaster.GetManagerByName(typeName);
                manager.LoadSaveData(saveData);
                
            }
            else if (typeKey == "INST")
            {
                var def = DimMaster.GetDefByName(typeName);
                var inst = DimMaster.LoadSavedInst(def, saveData);
            }
        }



    }
}