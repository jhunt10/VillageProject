using VillageProject.Core.DIM.Defs;

namespace VillageProject.Core.DIM.Insts;

public interface IInst
{
    public string _DebugId { get; }
    string Id { get; }
    IDef Def { get; }
    TComp? GetComponentWithKey<TComp>(string key, bool errorIfNull = false);
    
    /// <summary>
    /// Return the first component found of the given type.
    /// </summary>
    /// <param name="activeOnly">Only return a component that is active</param>
    /// <param name="errorIfNull">Throw an exception if no valid components are found</param>
    /// <typeparam name="TComp">Type to cast components as</typeparam>
    /// <returns></returns>
    TComp? GetComponentOfType<TComp>(bool activeOnly = true, bool errorIfNull = false);
    
    /// <summary>
    /// List all components of the given type.
    /// </summary>
    /// <param name="activeOnly">Only return components that are active</param>
    /// <typeparam name="TComp">Type to cast components as</typeparam>
    IEnumerable<TComp> ListComponentsOfType<TComp>(bool activeOnly = true);
    
    public DataDict BuildSaveData();
    void LoadSavedData(DataDict dataDict);

    public void Update(float delta);

    public void Delete();

    /// <summary>
    /// Will add a Keyed Watcher to all components of the given type.
    /// The watcher can call GetWatchedChange(key) to check if changes have occured since it last checked. 
    /// </summary>
    /// <param name="key">Instance unique key to be used when checking</param>
    /// <param name="changeFlag">Flags of changes to watch for</param>
    /// <param name="initiallyDirty">Set this flag as dirty upon creating it</param>
    public void AddChangeWatcher(string key, IEnumerable<string> changeFlag, bool initiallyDirty = true);

    /// <summary>
    /// Flags a component's change for any watcher of that component.
    /// Should only be called by the component it's self.
    /// </summary>
    /// <param name="changeFlag">Flag of change that occured</param>
    public void FlagWatchedChange(string changeFlag);

    /// <summary>
    /// List all changes that have occured since the last time this key was checked.
    /// </summary>
    /// <param name="key">Instance unique key provided when watcher was added</param>
    /// <param name="consumeChanges">Mark all changes as false after checking</param>
    /// <returns>List of changes</returns>
    public List<string> ListWatchedChanges(string key, bool consumeChanges = true);
    
    /// <summary>
    /// Check if a change in components has occured since the last time this key was checked.
    /// </summary>
    /// <param name="key">Instance unique key provided when watcher was added</param>
    /// <param name="changeFlag">Flag of change to check for</param>
    /// <param name="consumeChanges">Mark changes as false after checking</param>
    /// <returns>True if any chane has occured</returns>
    public bool GetWatchedChange(string key, string changeFlag, bool consumeChanges = true);
}