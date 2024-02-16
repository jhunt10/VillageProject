using System.Text.Json.Serialization;

namespace VillageProject.Core.DIM.Defs;

/// <summary>
/// A root class for internal functionality of CompDefs without dealing with generics.
/// </summary>
public abstract class RootCompDef : ICompDef
{
    [JsonIgnore]
    public IDef ParentDef { get; internal set; }
    
    [JsonPropertyName("CompDefClassName"), JsonPropertyOrder(0)]
    public abstract string CompDefClassName { get; }
    
    [JsonPropertyName("CompInstClassName"), JsonPropertyOrder(1)]
    public abstract string CompInstClassName { get; }
    
    [JsonPropertyName("ManagerClassName"), JsonPropertyOrder(2)]
    public abstract string ManagerClassName { get; }
    
    [JsonPropertyName("CompKey"), JsonPropertyOrder(3)]
    public abstract string CompKey { get; set; }
}