using VillageProject.Core.DIM.Defs;

namespace VillageProject.Core.DIM.Insts;

public interface ICompInst
{
    IInst Instance { get; } 
    ICompDef CompDef { get; }
}