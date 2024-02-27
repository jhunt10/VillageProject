using System.Text.Json.Serialization;

namespace VillageProject.Core.DIM.Defs;

public interface IDef
{
    string DefName { get; }
    string Label { get; }
    string LoadPath { get; }
    
    public List<ICompDef> CompDefs { get; }
    
    TComp? GetComponentWithKey<TComp>(string key);
    
    TComp? GetComponentOfType<TComp>(bool errorIfNull = false);
    
    IEnumerable<TComp> GetComponentsOfType<TComp>();
}