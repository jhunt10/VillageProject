using VillageProject.Core.DIM.Defs;

namespace VillageProject.Core.DIM.Insts;

public abstract class BaseInst : IInst
{
    protected Dictionary<string, Dictionary<string, bool>> _watchedComps =
        new Dictionary<string, Dictionary<string, bool>>();

    protected Dictionary<string, ICompInst> _components = 
        new Dictionary<string, ICompInst>();

    private bool _beingDeleted;
    
    public string _DebugId => (Def?.Label ?? "NoDef") + ":" + Id;
    public string Id { get; }
    public IDef Def { get; }
    
    public BaseInst(IDef def)
    {
        Id = Guid.NewGuid().ToString();
        Def = def;
    }
    
    public BaseInst(IDef def, string id)
    {
        Id = id;
        Def = def;
    }

    #region ------Component Access------
    public virtual TComp? GetComponentWithKey<TComp>(string key, bool errorIfNull = false)
    {
        if (_components.ContainsKey(key))
        {
            var comp = _components[key];
            if (comp is TComp)
                return (TComp)comp;
            else if (errorIfNull)
                throw new Exception($"Component '{key}' is not of type {typeof(TComp).FullName}.");
        }
        if(errorIfNull)
            throw new Exception($"No Component found with key '{key}'.");
            
        return default(TComp);
    }
    
    public virtual IEnumerable<TComp> ListComponentsOfType<TComp>(bool activeOnly = true)
    {
        foreach (var comp in _components.Values)
        {
            if(activeOnly && !comp.Active)
                continue;
            
            if (comp is TComp)
                yield return (TComp)comp;
        }
    }
    
    public virtual TComp? GetComponentOfType<TComp>(bool activeOnly = true, bool errorIfNull = false)
    {
        foreach (var comp in _components.Values)
        {
            if(activeOnly && !comp.Active)
                continue;

            if (comp is TComp)
                return (TComp)comp;
        }

        if (errorIfNull)
            throw new Exception(
                $"Failed to find Component of type '{typeof(TComp).FullName}' on Inst '{Def.DefName}:{Id}'.");
        return default(TComp);
    }
    #endregion


    public abstract DataDict BuildSaveData();
    public abstract void LoadSavedData(DataDict dataDict);

    public abstract void Update(float delta);

    public void Delete()
    {
        // If we aren't already in the process of being deleted, 
        //      mark as deleted and pass over to DimMaster
        if (!_beingDeleted)
        {
            _beingDeleted = true;
            DimMaster.DeleteInst(this);
            return;
        }

        _Delete();
    }

    public abstract void _Delete();

    public void AddComponentWatcher<TComp>(string key, bool initiallyDirty = true)
        where TComp : ICompInst
    {
        foreach (var comp in ListComponentsOfType<TComp>(activeOnly:false))
        {
            if(!_watchedComps.ContainsKey(comp.CompKey))
                _watchedComps.Add(comp.CompKey, new Dictionary<string, bool>());
            if(!_watchedComps[comp.CompKey].ContainsKey(key))
                _watchedComps[comp.CompKey].Add(key, initiallyDirty);
        }
    }

    public void FlagCompChange(ICompInst comp)
    {
        if(_watchedComps.ContainsKey(comp.CompKey))
            foreach (var pair in _watchedComps[comp.CompKey])
            {
                _watchedComps[comp.CompKey][pair.Key] = true;
            }
    }

    public bool GetWatchedChange(string key, bool consumeChange = true)
    {
        var change = false;
        foreach (var watchedComp in _watchedComps.Values)
        {
            if(!watchedComp.ContainsKey(key))
                continue;
            if (watchedComp[key])
            {
                change = true;
                if (consumeChange)
                    watchedComp[key] = false;
            }
        }
        return change;
    }
}