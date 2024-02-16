using VillageProject.Core.DIM.Defs;

namespace VillageProject.Core.DIM.Insts;

public interface IInst
{
    string Id { get; }
    IDef Def { get; }
    List<ICompInst> Components { get; }
    void AddComponent(ICompInst comp);
    TComp? GetComponentWithKey<TComp>(string key);
    TComp? GetComponentOfType<TComp>();
    IEnumerable<TComp> GetComponentsOfType<TComp>();
}