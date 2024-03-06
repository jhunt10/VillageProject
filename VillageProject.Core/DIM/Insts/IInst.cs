using VillageProject.Core.DIM.Defs;

namespace VillageProject.Core.DIM.Insts;

public interface IInst
{
    public string _DebugId { get; }
    string Id { get; }
    IDef Def { get; }
    List<ICompInst> Components { get; }
    void AddComponent(ICompInst comp);
    TComp? GetComponentWithKey<TComp>(string key);
    TComp? GetComponentOfType<TComp>(bool errorIfNull = false);
    IEnumerable<TComp> GetComponentsOfType<TComp>();
    
    public DataDict BuildSaveData();
}