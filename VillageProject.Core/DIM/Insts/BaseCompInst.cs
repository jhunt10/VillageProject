using VillageProject.Core.DIM.Defs;

namespace VillageProject.Core.DIM.Insts;

public class BaseCompInst : ICompInst
{
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
}