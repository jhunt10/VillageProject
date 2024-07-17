using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Sprites.Items;

namespace VillageProject.Core.DIM.Insts;

public abstract class BaseCompInst : ICompInst
{
    public string Id => Instance.Id + ":" + CompDef.CompKey;
    public string CompKey => CompDef.CompKey;
    public ICompDef CompDef { get; }
    public IInst Instance { get; }
    
    public bool Active { get; protected set; }
    private bool _inited;

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

    public virtual void OnDeleteInst()
    {
        
    }

    public virtual void Update(float delta)
    {
        
    }
    
    protected virtual void _Init() {}
    public void Init()
    {
        if(_inited)
            return;
        _Init();
        _inited = true;
    }

    protected virtual void _OnDeactivate() {}
    public void Deactivate()
    {
        if(!this.Active)
            return;
        this.Active = false;
        _OnDeactivate();
    }

    protected virtual void _OnActivate() {}
    public void Activate()
    {
        if(this.Active)
            return;
        this.Active = true;
        _OnActivate();
    }

    
}