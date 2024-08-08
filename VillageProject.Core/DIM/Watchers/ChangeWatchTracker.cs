using System.Collections;
using System.Runtime.InteropServices.ComTypes;

namespace VillageProject.Core.DIM.Watchers;

public class ChangeWatchTracker
{
    
    private Dictionary<string, Dictionary<string, bool>> _changeDict =
        new Dictionary<string, Dictionary<string, bool>>();
    
    private Dictionary<string, List<string>> _flagsToWatchers =
        new Dictionary<string, List<string>>();

    private static string KEY_FLAG_DIV = "|@|";
    private Hashtable _changeTable = new Hashtable();
    // private class TableKey
    // {
    //     public string WatcherKey { get; }
    //     public string WatchFlag { get; }
    //
    //     public TableKey(string key, string flag)
    //     {
    //         WatcherKey = key;
    //         WatchFlag = flag;
    //     }
    // }

    public void AddWatcher(string key, IEnumerable<string> flags, bool startDirty)
    {
        if (!_changeDict.ContainsKey(key))
            _changeDict.Add(key, new Dictionary<string, bool>());
        foreach (var flag in flags)
        {
            if(!_changeDict[key].ContainsKey(flag))
                _changeDict[key].Add(flag, startDirty);
            
            if(!_flagsToWatchers.ContainsKey(flag))
                _flagsToWatchers.Add(flag, new List<string>());
            
            if(!_flagsToWatchers[flag].Contains(key))
                _flagsToWatchers[flag].Add(key);
        }
        // foreach (var flag in flags)
        // {
        //     var tableKey = key + KEY_FLAG_DIV + flag;
        //     if (!_changeTable.ContainsKey(tableKey))
        //         _changeTable.Add(tableKey, startDirty);
        // }
    }

    public void AddChange(string flag)
    {
        if(!_flagsToWatchers.ContainsKey(flag))
            return;
        foreach (var watchKey in _flagsToWatchers[flag])
        {
            _changeDict[watchKey][flag] = true;
        }
    }

    public List<string> ListChanges(string key, bool consumeChange)
    {
        var outList = new List<string>();
        if (!_changeDict.ContainsKey(key))
            return outList;
        foreach (var pair in _changeDict[key])
        {
            if (pair.Value)
            {
                outList.Add(pair.Key);
                if (consumeChange)
                    _changeDict[key][pair.Key] = false;
            }
        }

        return outList;
    }

    public bool GetChange(string key, string flag, bool consumeChange)
    {
        if (!_changeDict.ContainsKey(key))
            Console.Error.WriteLine($"Unregistered key {key} asking for change flag {flag}.");
        else if (!_changeDict[key].ContainsKey(flag))
            Console.Error.WriteLine($"Key {key} asking for unregistered flag {flag}.");
        else if (_changeDict[key][flag])
        {
            if (consumeChange)
                _changeDict[key][flag] = false;
            return true;
        }

        return false;
    }
}