using VillageProject.Core.DIM.Defs;

namespace VillageProject.Core.DIM.Insts;

public interface ICompInst
{
    public bool Active { get; }
    public string CompKey { get; }
    IInst Instance { get; } 
    ICompDef CompDef { get; }

    DataDict? BuildSaveData();
    void LoadSavedData(DataDict dataDict);
    
    /// <summary>
    /// Called when Instance is deleted
    /// </summary>
    void OnDeleteInst();
    void Update(float delta);

    void Deactivate();

    void Activate();
}