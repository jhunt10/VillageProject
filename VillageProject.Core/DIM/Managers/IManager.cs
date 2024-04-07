using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.DIM;

public interface IManager
{
    public void Init();
    IInst CreateInst(IDef compDef, DataDict args);
    IInst LoadSavedInst(IDef compDef, DataDict data);
    ICompInst CreateCompInst(ICompDef compDef, IInst newInst, object? args);
    ICompInst LoadSavedCompInst(ICompDef compDef, IInst newInst, DataDict? data);
    public DataDict BuildSaveData();
    public void LoadSaveData(DataDict data);
    public void OnInstDelete(IInst inst);
}