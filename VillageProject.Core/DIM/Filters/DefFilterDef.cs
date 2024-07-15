namespace VillageProject.Core.DIM.Filters;

public class DefFilterDef
{
    public virtual string Label { get; set; }

    public List<string> AlowedPaths { get; set; } = new List<string>();
    public List<string> ExcludePaths { get; set; } = new List<string>();

    public List<string> AllowedTags { get; set; } = new List<string>();
    public List<string> ExcludedTags { get; set; } = new List<string>();

    public List<string> RequiredCompTypes { get; set; } = new List<string>();

    /// <summary>
    /// Go through the list of AllowedPaths and Exceptions and remove any redundent or invalid entries
    /// </summary>
    public void CleanUp()
    {
        // Clean up tags
        AllowedTags = AllowedTags.OrderBy(x => x).ToList();
        ExcludedTags = ExcludedTags.OrderBy(x => x).ToList();
        for (int n = ExcludedTags.Count - 1; n > 0; n--)
        {
            if (AllowedTags.Contains(ExcludedTags[n]))
                AllowedTags.RemoveAll(x => x == ExcludedTags[n]);
            if (ExcludedTags[n-1] == ExcludedTags[n])
                ExcludedTags.RemoveAt(n);
        }
        for (int n = AllowedTags.Count - 1; n > 0; n--)
            if (AllowedTags[n - 1] == AllowedTags[n])
                ExcludedTags.RemoveAt(n);

        // Merge and order all records
        var dic = new Dictionary<string, bool>();
        foreach (var path in ExcludePaths)
            if (!dic.ContainsKey(path))
                dic.Add(path, false);

        foreach (var path in AlowedPaths)
            if (!dic.ContainsKey(path))
                dic.Add(path, true);

        var orderedDic = dic.OrderBy(x => x.Key).Select(x => new { Path = x.Key, Allowed = x.Value }).ToList();

        // Work our way from the bottom up
        // Weither a state is valid or not soley depends on it's parent
        for (int n = orderedDic.Count - 1; n >= 0; n--)
        {
            var current = orderedDic[n];
            var closetParent = orderedDic.LastOrDefault(x =>
                orderedDic[n].Path.StartsWith(x.Path) && orderedDic[n].Path != x.Path);

            if (closetParent == null)
            {
                // We exclude by default, so an exclude should always have a parent
                if (!current.Allowed)
                    orderedDic.RemoveAt(n);
                continue;
            }

            // Any node who's state matches their parent's state is redundent
            if (closetParent.Allowed == current.Allowed)
            {
                orderedDic.RemoveAt(n);
                continue;
            }
        }

        AlowedPaths.Clear();
        ExcludePaths.Clear();
        foreach (var rec in orderedDic)
        {
            if (rec.Allowed)
                AlowedPaths.Add(rec.Path);
            else
                ExcludePaths.Add(rec.Path);
        }

    }
}