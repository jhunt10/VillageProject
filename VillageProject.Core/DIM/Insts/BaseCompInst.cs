using VillageProject.Core.DIM.Defs;

namespace VillageProject.Core.DIM.Insts;

public class BaseCompInst : ICompInst
{
    public string CompKey => CompDef.CompKey;
    public ICompDef CompDef { get; }
    public IInst Instance { get; }

    public BaseCompInst(ICompDef def, IInst inst)
    {
        CompDef = def ?? throw new ArgumentNullException(nameof(def));
        Instance = inst ?? throw new ArgumentNullException(nameof(inst));
    }

    protected TCompDef GetCompDefAs<TCompDef>()
        where TCompDef : ICompDef
    {
        var def = (TCompDef)CompDef;
        if (def == null)
            throw new Exception(
                $"Failed to case CompDef type {CompDef.GetType().FullName} to {typeof(TCompDef).FullName}");
        return def;
    }

    public virtual DataDict? BuildSaveData()
    {
        return null;
    }

    public virtual void LoadSavedData(DataDict dataDict)
    {
        
    }

    public virtual void Update()
    {
        
    }
}