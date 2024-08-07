﻿using System.Collections;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.DIM;

public class DimMaster
{
    private static string GetDefPath()
    {
        switch (Environment.UserName)
        {
            case "Jack":
                return @"C:\Users\Jack\Documents\Repos\VillageProject\Assets";
            case "johnnie.hunt":
                return @"C:\Users\johnnie.hunt\Documents\Personal\Repo\VillageProject\Assets";
        }

        throw new NotImplementedException($"No DefPath set for user: '{Environment.UserName}'");
    }
    
    public static char PATH_SEPERATOR = '.';
    public static char INST_COMP_SEPERATOR = ':';
    public static Dictionary<string, IManager> Managers;

    private static SaveLoader _saveLoader;
    private static Dictionary<string, IInst> _insts;
    private static Dictionary<string, IDef> _defs;

    private static List<IInstWatcher> _instWatchers = new List<IInstWatcher>();
    private static List<string> _deletionQue = new List<string>();
    
    private static bool _startup_completed = false;

    public static void StartUp()
    {
        _saveLoader = new SaveLoader(Path.Combine(GetDefPath(), "Saves"));
        _insts = new Dictionary<string, IInst>();
        LoadManagers();
        LoadDefs(Path.Join(GetDefPath(), "Defs"));
        InitiateManagers();
        _startup_completed = true;
    }


    public static void SaveGameState(string saveName)
    {
        _saveLoader.SaveGameState(saveName);
    }

    public static void ClearGameState()
    {
        foreach (var inst in _insts.Values)
        {
            DeleteInst(inst);
        }
    }
    
    public static void LoadGameState(string saveName)
    {
        ClearGameState();
        _saveLoader.LoadGameState(saveName);
    }

    public static void UpdateGameState(float delta)
    {
        foreach (var inst in _insts.Values.ToList())
        {
            inst.Update(delta);
        }
    }
    
    #region Defs

    public static void LoadDefs(string defPath)
    {
        if (_defs == null)
            _defs = new Dictionary<string, IDef>();

        var defFiles = Directory.EnumerateFiles(defPath, "*.json", SearchOption.AllDirectories).ToList();
        foreach (var defFile in defFiles)
        {
            try
            {
                var def = _loadDefFromFile(defFile);
                _defs.Add(def.DefName, def);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to load Def from file '{defFile}': {e.Message} \n {e.StackTrace}");
            }
        }
    }

    public static IDef? GetDefByName(string defName, bool errorIfMissing = true)
    {
        if (_defs.ContainsKey(defName))
            return _defs[defName];
        if(errorIfMissing)
            throw new Exception($"Failed to find def by name '{defName}'.");
        return null;
    }
    
    public static IDef? GetDefByPartialName(string sufix, bool errorIfMissing = true)
    {
        foreach (var def in _defs)
        {
            if (def.Key.EndsWith(sufix))
                return def.Value;
        }
        if(errorIfMissing)
            throw new Exception($"Failed to find with suffix '{sufix}'.");
        return null;
    }

    public static IEnumerable<IDef> GetAllDefsWithCompDefType<TCompDef>()
        where TCompDef : class, ICompDef
    {
        foreach (var def in _defs.Values)
        foreach (var compDef in def.CompDefs)
            if (compDef is TCompDef)
            {
                yield return def;
                break;
            }
    }

    private static IDef _loadDefFromFile(string filePath)
    {
        var text = File.ReadAllText(filePath);
        
        
        var jsonObj = JsonNode.Parse(text);
        var className = jsonObj[nameof(IDef.DefClassName)].ToString();
        var type = GetTypeByName(className);
        
        var rawDef = JsonSerializer.Deserialize(text, type);
        if (rawDef == null)
            throw new Exception($"Failed to deserialize def '{filePath}'.");
        var def = rawDef as BaseDef;
        if (def == null)
            throw new Exception($"Failed to cast deserilaized type '{className}' to BaseDef");
        
        def.LoadPath = Path.GetDirectoryName(filePath);
        def.CompDefs = def.CompDefs.Where(x => x != null).ToList();
        foreach (var compDef in def.CompDefs)
        {
            if (compDef.CompKey.Contains(INST_COMP_SEPERATOR))
                throw new Exception(
                    $"Invalid CompKey {compDef.CompKey} on Def {def.DefName}. Can not contain \"{INST_COMP_SEPERATOR}\".");
            ((BaseCompDef)compDef).ParentDef = def;
        }
        return def;
    }

    #endregion
    

    #region Managers

    public static void LoadManagers()
    {
        var interfaceType = typeof(IManager);
        var managerTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => 
                !p.IsAbstract && !p.IsInterface
                && interfaceType.IsAssignableFrom(p));
        Managers = new Dictionary<string, IManager>();
        foreach (var type in managerTypes)
        {
            var manager = Activator.CreateInstance(type);
            if (manager == null)
                throw new Exception($"Failed to instantiate Manager of type '{type.FullName}'.");

            var iManager = manager as IManager;
            if (iManager == null)
                throw new Exception($"Failed cast Manager of type '{type.FullName}' to IManager.");

            Managers.Add(type.FullName, iManager);
        }
    }

    public static void InitiateManagers()
    {
        foreach (var manager in Managers.Values)
        {
            manager.Init();
        }
    }

    public static TManager GetManager<TManager>()
        where TManager : class, IManager
    {   
        var name = typeof(TManager).FullName;
        if (!Managers.ContainsKey(name))
            throw new Exception($"No Manager found with class name '{name}'.");
        var manager = Managers[name] as TManager;
        if(manager == null)
            throw new Exception($"Manager '{name}' is not of type '{name}'.");
        return manager;
    }
    
    public static IManager GetManagerByName(string name)
    {
        if (Managers.ContainsKey(name))
            return Managers[name];
        throw new Exception($"No Manager found with class name '{name}'.");
    }

    public static IEnumerable<TManager> ListManagersOfType<TManager>()
        where TManager : IManager
    {
        foreach (var manager in Managers.Values)
        {
            if (manager is TManager)
                yield return (TManager)manager;
        }
    }

    #endregion


    #region Insts

    private static List<string> _creatingInstsIds = new List<string>();
    
    /// <summary>
    /// Check if an Inst is being created by the DimMaster. All managers should check with the DimMaster
    /// to insure Insts aren't being created outside the DimMaster's control.
    /// </summary>
    public static bool CheckCreatingInst(string id)
    {
        return _creatingInstsIds.Contains(id);}
  
    public static IInst InstantiateDef(IDef def, DataDict? arguments = null)
    {
        DataDict args = arguments ?? new DataDict();
        _creatingInstsIds.Add(args.Id);
        
        IInst? inst = null;
        if (def.ManagerClassName == typeof(DimMaster).FullName)
        {
            inst = _createObjectInstFromDef(def, args);
        }
        else
        {
            var manager = GetManagerByName(def.ManagerClassName);
            inst = manager.CreateInst(def, args);
        }

        if (inst == null)
            throw new Exception($"Failed to create inst from def '{def.DefName}'.");
        foreach (var comp in inst.ListComponentsOfType<ICompInst>(activeOnly:false))
        {
            comp.Init();
        }
        
        Console.WriteLine($"Instantiated Def '{def.DefName}' to '{inst._DebugId}'.");
        _creatingInstsIds.Remove(inst.Id);
        return inst;
    }

    private static IInst _createObjectInstFromDef(IDef def, DataDict compArgs)
    {
        var inst = new ObjectInst(def, compArgs.Id);
        foreach (var compDef in def.CompDefs)
        {
            // Get manager responsible for creating each component
            var managerName = compDef.ManagerClassName;
            var manager = GetManagerByName(managerName);
            
            // Get args base off CompKey if args were provided
            var args = default(object?);
            if (compArgs != null && compArgs.HasKey(compDef.CompKey))
                args = compArgs.GetValueAs<object>(compDef.CompKey);
            
            // Manager creates new instance of the Component
            var compInst = manager.CreateCompInst(compDef, inst, args);
            inst.AddComponent(compInst);
        }
        _insts.Add(inst.Id, inst);
        
        // Notify watchers of new Instance created
        foreach (var watcher in _instWatchers)
        {
            watcher.OnNewInstCreated(inst);
        }
        return inst;
    }

    public static IInst LoadSavedInst(IDef def, DataDict saveData)
    {
        var inst = new ObjectInst(def, saveData.Id);
        _creatingInstsIds.Add(inst.Id);
        foreach (var compDef in def.CompDefs)
        {
            var managerName = compDef.ManagerClassName;
            var manager = GetManagerByName(managerName);
            var compSaveData = saveData.GetValueAs<DataDict>(compDef.CompKey, errorIfMissing: false);
            var compInst = manager.LoadSavedCompInst(compDef, inst, compSaveData);
            inst.AddComponent(compInst);
        }
        _insts.Add(inst.Id, inst);
        foreach (var comp in inst.ListComponentsOfType<ICompInst>(activeOnly:false))
        {
            comp.Init();
        }
        foreach (var watcher in _instWatchers)
        {
            watcher.OnInstLoaded(inst);
        }
        _creatingInstsIds.Remove(inst.Id);
        return inst;
    }

    public static IInst? GetInstById(string id, bool errorIfNull = false)
    {
        var instId = id;
        if (id.Contains(":"))
            instId = id.Split(':').First();
        if (_insts.ContainsKey(instId))
            return _insts[instId];
        if (errorIfNull)
            throw new Exception($"Failed to find inst with Id '{id}'.");
        return null;
    }
    
    public static TComp? GetCompAsTypeById<TComp>(string id, bool errorIfNull = false)
        where TComp : ICompInst
    {
        var tokens = id.Split(INST_COMP_SEPERATOR);
        if (tokens.Length != 2)
            throw new Exception($"Invalid CompId '{id}'. Expected format: INST_ID{INST_COMP_SEPERATOR}COMP_KEY ");
        if (_insts.ContainsKey(tokens[0]))
        {
            return _insts[tokens[0]].GetComponentWithKey<TComp>(tokens[1], errorIfNull);
        }
        if (errorIfNull)
            throw new Exception($"Failed to find inst with Id '{id}'.");
        return default(TComp);
    }

    public static IEnumerable<IInst> ListAllInsts()
    {
        foreach (var inst in _insts.Values)
        {
            yield return inst;
        }
    }

    public static void DeleteInst(IInst inst)
    {
        // If already being deleted, ignore
        if(_deletionQue.Contains(inst.Id))
            return;
        Console.WriteLine($"DimMaster Deleting Inst: {inst._DebugId}.");
        _deletionQue.Add(inst.Id);
        inst.Delete();
        foreach (var manager in Managers.Values)
        {
            manager.OnInstDelete(inst);
        }

        foreach (var watcher in _instWatchers)
        {
            watcher.OnInstDeleted(inst);
        }

        _insts.Remove(inst.Id);
    }
    
    #endregion

    public static void AddInstWatcher(IInstWatcher watcher)
    {
        if(_instWatchers.Contains(watcher))
            return;
        _instWatchers.Add(watcher);
    }
    
    public static Type? GetTypeByName(string name)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Reverse())
        {
            var tt = assembly.GetType(name);
            if (tt != null)
            {
                return tt;
            }
        }

        return null;
    }
}