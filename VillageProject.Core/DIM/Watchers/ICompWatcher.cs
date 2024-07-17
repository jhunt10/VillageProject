using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.DIM.Watchers;

public interface IInstChangeWatcher<TComp>
    where TComp : ICompInst
{
    public string Id { get; }
}