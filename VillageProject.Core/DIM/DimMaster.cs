using System.Text.Json;
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
    public static List<IDef> Defs;
    public static Dictionary<string, IManager> Managers;

    private static bool _startup_completed = false;

    public static void StartUp()
    {
        LoadManagers();
        LoadDefs(GetDefPath());
        InitiateManagers();
    }

    #region Defs

    public static void LoadDefs(string defPath)
    {
        if (Defs == null)
            Defs = new List<IDef>();

        var defFiles = Directory.EnumerateFiles(defPath, "*.json", SearchOption.AllDirectories).ToList();
        foreach (var defFile in defFiles)
        {
            try
            {
                var def = _loadDefFromFile(defFile);
                Defs.Add(def);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to load Def from file '{defFile}': {e.Message}");
            }
        }
    }

    public static IEnumerable<IDef> GetAllDefsWithCompDefType<TCompDef>()
        where TCompDef : class, ICompDef
    {
        foreach (var def in Defs)
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
        var def = JsonSerializer.Deserialize<Def>(text);
        if (def == null)
            throw new Exception($"Failed to deserialize def '{filePath}'.");
        def.LoadPath = Path.GetDirectoryName(filePath);
        foreach (var compDef in def.CompDefs)
        {
            ((RootCompDef)compDef).ParentDef = def;
        }
        return def;
    }

    public static IInst InstantiateDef(IDef def, Dictionary<string, object>? compArgs = null)
    {
        var inst = new Inst(def);
        foreach (var compDef in def.CompDefs)
        {
            var managerName = compDef.ManagerClassName;
            var manager = GetManagerByName(managerName);
            var args = default(object?);
            if (compArgs != null && compArgs.ContainsKey(compDef.CompKey))
                args = compArgs[compDef.CompKey];
            var compInst = manager.CreateCompInst(compDef, inst, args);
            if(!inst.Components.Contains(compInst))
                inst.AddComponent(compInst);
        }

        return inst;
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

    #endregion
    
    
    
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