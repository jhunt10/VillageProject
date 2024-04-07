using VillageProject.Core.DIM.Defs;

namespace VillageProject.Core.DIM.Insts;

public class ObjectInst : BaseInst
{
    public ObjectInst(IDef def) : base(def)
    {
        
    }
    public ObjectInst(IDef def, string id) : base(def, id)
    {
        
    }

    public void AddComponent(ICompInst comp)
    {
        if(_components.ContainsKey(comp.CompKey))
            throw new Exception($"Duplicate CompKeys: {comp.CompDef.CompKey}");
        _components.Add(comp.CompKey, comp);
    }

    public override DataDict BuildSaveData()
    {
        var data = new DataDict(Id);
        foreach (var compPair in _components)
        {
            var compData = compPair.Value.BuildSaveData();
            if(compData != null)
                data.AddData(compPair.Key, compData);
        }
        return data;
    }

    public override void LoadSavedData(DataDict dataDict)
    {
        // Components will be loaded as they are created by appropriate managers 
    }

    public override void Update(float delta)
    {
        foreach (var comp in _components.Values)
        {
            if(comp.Active)
                comp.Update(delta);
        }
    }
    
    public override void _Delete()
    {   
        foreach (var comp in _components.Values)
        {
            comp.OnDeleteInst();
        }
    }
}