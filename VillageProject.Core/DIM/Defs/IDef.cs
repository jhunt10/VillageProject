using System.Text.Json.Serialization;

namespace VillageProject.Core.DIM.Defs;

public interface IDef
{
    string Name { get; }
    string Label { get; }
    
    [JsonIgnore]
    public IDef ParentDef { get; }
    public List<IDef> SubDefs { get; }
}