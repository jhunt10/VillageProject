using System.Text.Json;

namespace VillageProject.Core.DIM;

public class DataDict
{
    public string Id { get; set; }
    public Dictionary<string, object?> Data { get; set; }
        = new Dictionary<string, object?>();

    public DataDict()
    {
        Id = Guid.NewGuid().ToString();
    }
    public DataDict(string id)
    {
        Id = id;
    }

    public void AddData(string key, object? obj)
    {
        Data.Add(key, obj);
    }

    public bool HasKey(string key)
    {
        return Data.ContainsKey(key);
    }
    
    public TVal? GetValueAs<TVal>(string key, bool errorIfMissing = true)
    {
        if (Data.ContainsKey(key))
        {
            var val = Data[key];
            try
            {
                if (val is JsonElement)
                {
                    var jVal = (JsonElement)val;
                    return jVal.Deserialize<TVal>();
                }
                else
                {
                    return (TVal)val;
                }
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