using System.Text.Json;

namespace VillageProject.Core.DIM;

public class DataDict
{
    public string Id { get; set; }
    public Dictionary<string, object?> Data { get; set; }

    public DataDict(string id)
    {
        Id = id;
        Data = new Dictionary<string, object?>();
    }

    public void AddData(string key, object? obj)
    {
        Data.Add(key, obj);
    }

    public TVal? GetValueAs<TVal>(string key, bool errorIfMissing = true)
    {
        if (Data.ContainsKey(key))
        {
            var val = Data[key];
            try
            {
                var jVal = (JsonElement)val;
                return jVal.Deserialize<TVal>();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to cast value of '{key}' to type {typeof(TVal).FullName}.");
            }
        }
        if(errorIfMissing)
            throw new Exception($"No property found with key '{key}'.");
        
        return default(TVal?);
    }
}