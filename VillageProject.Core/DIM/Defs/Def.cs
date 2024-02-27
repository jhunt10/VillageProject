using System.Text.Json.Serialization;

namespace VillageProject.Core.DIM.Defs;

public class Def : IDef
{
    [JsonPropertyName("DefName"), JsonPropertyOrder(-10)]
    public string DefName { get; set; }
    
    [JsonPropertyName("Label"), JsonPropertyOrder(-9)]
    private string? _label { get; set; }
    public string Label
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_label))
                _label = DefName.Split(DimMaster.PATH_SEPERATOR).Last();
            return _label;
        }

        set
        {
            _label = value;
        }
    }
    
    [JsonIgnore]
    public string LoadPath { get; internal set; }
    
    [JsonPropertyName("CompDefs")]
    public List<ICompDef> CompDefs { get; set; }

    public TComp GetComponentWithKey<TComp>(string key)
    {
        foreach (var comp in CompDefs)
        {
            if (comp.CompKey == key)
            {
                if (comp is TComp)
                    return (TComp)comp;
                else
                {
                    throw new Exception($"Component Def '{key}' is not of type {typeof(TComp).FullName}.");
                }
            }
        }
        return default(TComp);
    }
    
    public IEnumerable<TComp> GetComponentsOfType<TComp>()
    {
        foreach (var comp in CompDefs)
        {
            if (comp is TComp)
                yield return (TComp)comp;
        }
    }
    
    public TComp? GetComponentOfType<TComp>(bool errorIfNull = false)
    {
        foreach (var comp in CompDefs)
        {
            if (comp is TComp)
                return (TComp)comp;
        }

        if (errorIfNull)
            throw new Exception(
                $"Failed to find CompDef of type '{typeof(TComp).FullName}' on Def '{DefName}'.");
        return default(TComp);
    }
}