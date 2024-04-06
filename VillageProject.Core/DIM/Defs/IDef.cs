using System.Text.Json.Serialization;

namespace VillageProject.Core.DIM.Defs;

public interface IDef
{
    string DefName { get; }
    string Label { get; }
    string LoadPath { get; }
    
    public List<ICompDef> CompDefs { get; }
    
    TComp? GetComponentDefWithKey<TComp>(string key);
    
    TComp? GetComponentDefOfType<TComp>(bool errorIfNull = false);
    
    IEnumerable<TComp> ListComponentDefsOfType<TComp>();
}