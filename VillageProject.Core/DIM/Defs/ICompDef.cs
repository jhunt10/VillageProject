using System.Text.Json.Serialization;

namespace VillageProject.Core.DIM.Defs;

[JsonConverter(typeof(CompDefJsonConverter))]
public interface ICompDef
{
    string CompDefClassName { get; }
    
    string CompInstClassName { get; }
    
    string ManagerClassName { get; }
    
    bool RegisterOnLoad { get; }
}