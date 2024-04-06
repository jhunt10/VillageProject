using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.DIM;

public abstract class BaseManager : IManager
{
    public virtual void Init()
    {
        
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
            throw new Exception($"Failed to instantiate Manager of type '{type.FullName}'.");

        var iCompInst = compInst as ICompInst;
        if (iCompInst == null)
            throw new Exception($"Failed cast Manager of type '{type.FullName}' to IManager.");

        return iCompInst;
    }
    
    public virtual ICompInst LoadSavedCompInst(ICompDef compDef, IInst newInst, DataDict? dataDict)
    {
        var type = DimMaster.GetTypeByName(compDef.CompInstClassName);
        if(type == null)
            throw new Exception($"Failed to find CompInst type '{compDef.CompInstClassName}'.");
        
        var compInst = Activator.CreateInstance(type, new object[] {compDef, newInst});
        if (compInst == null)
            throw new Exception($"Failed to instantiate Manager of type '{type.FullName}'.");

        var iCompInst = compInst as ICompInst;
        if (iCompInst == null)
            throw new Exception($"Failed cast Manager of type '{type.FullName}' to IManager.");

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