using VillageProject.Core.DIM.Defs;

namespace VillageProject.Core.DIM.Insts;

public interface ICompInst
{
    public string CompKey { get; }
    IInst Instance { get; } 
    ICompDef CompDef { get; }

    DataDict? BuildSaveData();
    void LoadSavedData(DataDict dataDict);
    void Update();
}