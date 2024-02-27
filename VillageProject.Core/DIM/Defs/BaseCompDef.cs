using System.Text.Json.Serialization;

namespace VillageProject.Core.DIM.Defs;

/// <summary>
/// A root class for internal functionality of CompDefs without dealing with generics.
/// </summary>
public abstract class BaseCompDef : ICompDef
{
    [JsonIgnore]
    public IDef ParentDef { get; internal set; }
    
    [JsonPropertyName("CompDefClassName"), JsonPropertyOrder(-10)]
    public abstract string CompDefClassName { get; }
    
    [JsonPropertyName("CompInstClassName"), JsonPropertyOrder(-9)]
    public abstract string CompInstClassName { get; }
    
    [JsonPropertyName("ManagerClassName"), JsonPropertyOrder(-8)]
    public abstract string ManagerClassName { get; }
    
    [JsonPropertyName("CompKey"), JsonPropertyOrder(-7)]
    public abstract string CompKey { get; set; }


    public IManager GetManager()
    {
        return DimMaster.GetManagerByName(ManagerClassName);
    }
}