using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.DIM;

public abstract class BaseManager : IManager
{
    public virtual void Init()
    {
        
    }
    public virtual IInst CreateInst(IDef def, DataDict args)
    {
        if (!DimMaster.CheckCreatingInst(args.Id))
        {
            throw new Exception(
                "Insts should not be created by directly calling a Manager. Use DimMaster.InstantiateDef instead.");
        }
        
        var type = DimMaster.GetTypeByName(def.InstClassName);
        if(type == null)
            throw new Exception($"Failed to find Inst type '{def.InstClassName}'.");

        var argsList = new List<object>();
        argsList.Add(def);
        argsList.Add(args.Id);
        if(args != null)
            argsList.Add(args);
        var inst = Activator.CreateInstance(type, argsList.ToArray());
        if (inst == null)
            throw new Exception($"Failed to instantiate Inst of type '{type.FullName}'.");

        var iInst = inst as IInst;
        if (iInst == null)
            throw new Exception($"Failed cast Inst of type '{type.FullName}' to IInst.");

        return iInst;
    }
    
    public virtual IInst LoadSavedInst(IDef def, DataDict dataDict)
    {
        if (!DimMaster.CheckCreatingInst(dataDict.Id))
        {
            throw new Exception(
                "Insts should not be load by directly calling a Manager. Use DimMaster.LoadSavedInst instead.");
        }
        
        var type = DimMaster.GetTypeByName(def.InstClassName);
        if(type == null)
            throw new Exception($"Failed to find Inst type '{def.InstClassName}'.");
        
        var inst = Activator.CreateInstance(type, new object[] {def, dataDict.Id});
        if (inst == null)
            throw new Exception($"Failed to instantiate Inst of type '{type.FullName}'.");

        var iInst = inst as IInst;
        if (iInst == null)
            throw new Exception($"Failed cast Inst of type '{type.FullName}' to IInst.");

        if(dataDict != null)
            iInst.LoadSavedData(dataDict);
        
        return iInst;
    }
    public virtual ICompInst CreateCompInst(ICompDef compDef, IInst newInst, object? args)
    {
        var type = DimMaster.GetTypeByName(compDef.CompInstClassName);
        if(type == null)
            throw new Exception($"Failed to find CompInst type '{compDef.CompInstClassName}'.");

        var argsList = new List<object>();
        argsList.Add(compDef);
        argsList.Add(newInst);
        if(args != null)
            argsList.Add(args);
        var compInst = Activator.CreateInstance(type, argsList.ToArray());
        if (compInst == null)
            throw new Exception($"Failed to instantiate CompInst of type '{type.FullName}'.");

        var iCompInst = compInst as ICompInst;
        if (iCompInst == null)
            throw new Exception($"Failed cast CompInst of type '{type.FullName}' to ICompInst.");

        return iCompInst;
    }
    
    public virtual ICompInst LoadSavedCompInst(ICompDef compDef, IInst newInst, DataDict? dataDict)
    {
        var type = DimMaster.GetTypeByName(compDef.CompInstClassName);
        if(type == null)
            throw new Exception($"Failed to find CompInst type '{compDef.CompInstClassName}'.");
        
        var compInst = Activator.CreateInstance(type, new object[] {compDef, newInst});
        if (compInst == null)
            throw new Exception($"Failed to instantiate CompInst of type '{type.FullName}'.");

        var iCompInst = compInst as ICompInst;
        if (iCompInst == null)
            throw new Exception($"Failed cast CompInst of type '{type.FullName}' to ICompInst.");

        if(dataDict != null)
            iCompInst.LoadSavedData(dataDict);
        
        return iCompInst;
    }

    public virtual DataDict BuildSaveData()
    {
        return null;
    }

    public virtual void LoadSaveData(DataDict data)
    {
        
    }

    public virtual void OnInstDelete(IInst inst)
    {
        
    }
}