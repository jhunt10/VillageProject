using System.Text.Json.Serialization;
using VillageProject.Core.Serialization;

namespace VillageProject.Core.DIM.Defs;

[JsonConverter(typeof(CompDefJsonConverter))]
public interface ICompDef
{
    IManager GetManager();
    
    IDef ParentDef { get; }
    
    string CompDefClassName { get; }
    
    string CompInstClassName { get; }
    
    string ManagerClassName { get; }
    
    string CompKey { get; }
}