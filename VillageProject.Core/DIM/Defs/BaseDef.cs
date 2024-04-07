using System.Text.Json.Serialization;

namespace VillageProject.Core.DIM.Defs;

public abstract class BaseDef : IDef
{
    [JsonPropertyName("DefName"), JsonPropertyOrder(-11)]
    public string DefName { get; set; }
    
    [JsonPropertyName("CompDefClassName"), JsonPropertyOrder(-10)]
    public abstract string DefClassName { get; }
    
    [JsonPropertyName("CompInstClassName"), JsonPropertyOrder(-9)]
    public abstract string InstClassName { get; }
    
    [JsonPropertyName("ManagerClassName"), JsonPropertyOrder(-8)]
    public abstract string ManagerClassName { get; }
    
    [JsonPropertyName("Label"), JsonPropertyOrder(-9)]
    private string? _label { get; set; }
    public virtual string Label
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
    public List<ICompDef> CompDefs { get; set; } = new List<ICompDef>();
    public TComp GetComponentDefWithKey<TComp>(string key)
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
    
    public IEnumerable<TComp> ListComponentDefsOfType<TComp>()
    {
        foreach (var comp in CompDefs)
        {
            if (comp is TComp)
                yield return (TComp)comp;
        }
    }
    
    public TComp? GetComponentDefOfType<TComp>(bool errorIfNull = false)
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