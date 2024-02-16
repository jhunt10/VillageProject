﻿using VillageProject.Core.DIM.Defs;

namespace VillageProject.Core.DIM.Insts;

public class Inst : IInst
{
    public string Id { get; }
    public IDef Def { get; }

    public List<ICompInst> Components { get; }
    
    public Inst(IDef def)
    {
        Id = Guid.NewGuid().ToString();
        Def = def;
        Components = new List<ICompInst>();
    }

    public void AddComponent(ICompInst comp)
    {
        var conflict = GetComponentWithKey<ICompInst>(comp.CompDef.CompKey);
        if (conflict != null)
            throw new Exception($"Duplicate CompKeys: {comp.CompDef.CompKey}");
        Components.Add(comp);
    }

    public TComp GetComponentWithKey<TComp>(string key)
    {
        foreach (var comp in Components)
        {
            if (comp.CompDef.CompKey == key)
            {
                if (comp is TComp)
                    return (TComp)comp;
                else
                {
                    throw new Exception($"Component '{key}' is not of type {typeof(TComp).FullName}.");
                }
            }
        }
        return default(TComp);
    }
    
    public IEnumerable<TComp> GetComponentsOfType<TComp>()
    {
        foreach (var comp in Components)
        {
            if (comp is TComp)
                yield return (TComp)comp;
        }
    }
    
    public TComp? GetComponentOfType<TComp>()
    {
        foreach (var comp in Components)
        {
            if (comp is TComp)
                return (TComp)comp;
        }
        return default(TComp);
    }
}