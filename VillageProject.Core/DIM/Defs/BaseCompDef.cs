using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.DIM.Defs;

public abstract class BaseCompDef<TCompInst, TManager> : ICompDef 
    where TCompInst : class, ICompInst
    where TManager : class, IManager
{
    public virtual string CompDefClassName
    {
        get { return this.GetType().FullName; }
    }
    public virtual string CompInstClassName
    {
        get { return typeof(TCompInst).FullName; }
    }
    public virtual string ManagerClassName 
    {
        get { return typeof(TManager).FullName; }
    }
    public virtual bool RegisterOnLoad { get; }
}