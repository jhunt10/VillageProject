using System.Text.Json.Serialization;

namespace VillageProject.Core.DIM.Defs;

[JsonConverter(typeof(CompDefJsonConverter))]
public interface ICompDef
{
    IDef ParentDef { get; }
    
    string CompDefClassName { get; }
    
    string CompInstClassName { get; }
    
    string ManagerClassName { get; }
    
    string CompKey { get; }
}