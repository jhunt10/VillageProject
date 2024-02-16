using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.DIM;

public interface IManager
{
    public void Init();
    ICompInst CreateCompInst(ICompDef compDef, IInst newInst, object? args);
}