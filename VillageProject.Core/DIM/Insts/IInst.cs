using VillageProject.Core.DIM.Defs;

namespace VillageProject.Core.DIM.Insts;

public interface IInst
{
    public string _DebugId { get; }
    string Id { get; }
    IDef Def { get; }
    List<ICompInst> Components { get; }
    void AddComponent(ICompInst comp);
    TComp? GetComponentWithKey<TComp>(string key);
    TComp? GetComponentOfType<TComp>(bool errorIfNull = false);
    IEnumerable<TComp> GetComponentsOfType<TComp>();
    
    public DataDict BuildSaveData();

    public void Delete();

    /// <summary>
    /// Will add a Keyed Watcher to all components of the given type.
    /// The watcher can call GetWatchedChange(key) to check if changes have occured since it last checked. 
    /// </summary>
    /// <param name="key">Instance unique key to be used when checking</param>
    /// <typeparam name="TComp">Type of component(s) to watch for change on</typeparam>
    public void AddComponentWatcher<TComp>(string key, bool initiallyDirty = true)
        where TComp : ICompInst;

    /// <summary>
    /// Flags a component's change for any watcher of that component.
    /// Should only be called by the component it's self.
    /// </summary>
    /// <param name="comp">Component who has changed</param>
    public void FlagWatchedChange(ICompInst comp);

    /// <summary>
    /// Check if a change in components has occured since the last time this key was checked.
    /// </summary>
    /// <param name="key">Instance unique key provided when watcher was added</param>
    /// <returns>True if any chane has occured</returns>
    public bool GetWatchedChange(string key, bool consumeChange = true);
}