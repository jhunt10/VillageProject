// namespace VillageProject.Core.DIM.Filters;
//
// public class DefFilter
// {
//     public string FilterId { get; }
//     public string Label => FilterDef.Label;
//     public DefFilterDef FilterDef { get; private set; }
//
//     private List<DefPath> _allowedPaths;
//     private List<DefPath> _excludePaths;
//
//     public DefFilter(DefFilterDef def)
//     {
//         FilterId = Guid.NewGuid().ToString();
//         FilterDef = def ?? throw new ArgumentNullException(nameof(def));
//         FilterDef.CleanUp();
//
//         _allowedPaths = FilterDef.AlowedPaths.Select(x => new DefPath(x)).ToList();
//         _excludePaths = FilterDef.ExcludePaths.Select(x => new DefPath(x)).ToList();
//     }
//
//     public bool CanAcceptDefPath(string defPath)
//     {
//         // We don't Accept Partials. Those are excluded defs flagged mostly for ui stuff
//         return GetBranchState(defPath) == FilterBranchState.Allowed;
//     }
//
//     public bool CanAcceptDef(IDef def)
//     {
//         if (!CanAcceptDefPath(def.FullName))
//             return false;
//
//         if(FilterDef.RequiredComps?.Any() ?? false)
//         {
//             var objDef = def as IObjectDef;
//             if (objDef == null)
//                 return false;
//             foreach (var req in FilterDef.RequiredComps)
//                 if (!objDef.CompDefs.ContainsKey(req))
//                     return false;
//         }
//         return true;
//     }
//
//     private FilterBranchState GetBranchState(string defPath)
//     {
//         var closestAllowedParentOrMatch = _allowedPaths.Where(x => x.IsParentOf(defPath) || x.IsMatch(defPath)).ToList().OrderByDescending(x => x.Length).FirstOrDefault();
//         var closestExcludedParentOrMatch = _excludePaths.Where(x => x.IsParentOf(defPath) || x.IsMatch(defPath)).ToList().OrderByDescending(x => x.Length).FirstOrDefault();
//
//         // If a parent or this is allowed and (no branch excludes it or a parent OR the closest parent is allow): Allow
//         if ((closestAllowedParentOrMatch != null && closestExcludedParentOrMatch == null) ||
//             (closestAllowedParentOrMatch != null && closestAllowedParentOrMatch.Length > closestExcludedParentOrMatch.Length))
//         {
//             if (_excludePaths.Any(x => x.IsChildOf(defPath)))
//                 return FilterBranchState.Partial;
//             return FilterBranchState.Allowed;
//         }
//
//         // Otherwise it's excluded, but we need to check for children
//         if (_allowedPaths.Any(x => x.IsChildOf(defPath)))
//             return FilterBranchState.Partial;
//         return FilterBranchState.Excluded;
//     }
//
//     public void AddAllowedBranch(string defPath)
//     {
//         var state = GetBranchState(defPath);
//         if (state == FilterBranchState.Allowed)
//             return;
//
//         _pruneWholeBranch(defPath);
//         FilterDef.AlowedPaths.Add(defPath);
//         FilterDef.CleanUp();
//
//         _allowedPaths = FilterDef.AlowedPaths.Select(x => new DefPath(x)).ToList();
//         _excludePaths = FilterDef.ExcludePaths.Select(x => new DefPath(x)).ToList();
//     }
//
//     public void AddExcludedBranch(string defPath)
//     {
//         var state = GetBranchState(defPath);
//         if (state == FilterBranchState.Excluded)
//             return;
//
//         _pruneWholeBranch(defPath);
//         FilterDef.ExcludePaths.Add(defPath);
//         FilterDef.CleanUp();
//
//         _allowedPaths = FilterDef.AlowedPaths.Select(x => new DefPath(x)).ToList();
//         _excludePaths = FilterDef.ExcludePaths.Select(x => new DefPath(x)).ToList();
//     }
//
//     private void _pruneWholeBranch(string defPath)
//     {
//         for (int n = FilterDef.AlowedPaths.Count - 1; n >= 0; n--)
//         {
//             if (FilterDef.AlowedPaths[n].StartsWith(defPath))
//                 FilterDef.AlowedPaths.RemoveAt(n);
//         }
//         for (int n = FilterDef.ExcludePaths.Count - 1; n >= 0; n--)
//         {
//             if (FilterDef.ExcludePaths[n].StartsWith(defPath))
//                 FilterDef.ExcludePaths.RemoveAt(n);
//         }
//     }
//
//     private enum FilterBranchState
//     {
//         /// <summary>
//         /// An item is allowed if: 
//         ///     1) It or a parent are explicatly allowed
//         ///     2) A parent is Excluded, but it or a closer parent are explicatly allowed
//         /// </summary>
//         Allowed = 0,
//         /// <summary>
//         /// An item is excluded if:
//         ///     1) Neither it or any parent is explicatly allowed
//         ///     2) A parent is explicatly allowed but it or a closer parent are listed as exceptions
//         /// </summary>
//         Excluded = 1,
//
//         /// <summary>
//         ///  An item is partial if:
//         ///     1) It is not allowed, but any child is specifically included
//         ///  * Mostly for Ui stuff *
//         /// </summary>
//         Partial = 2
//     }
//
//     private class DefPath
//     {
//         public string FullPath { get; private set; }
//         public string EndNode { get; private set; }
//         public int Length { get; private set; }
//
//         public DefPath(string path)
//         {
//             FullPath = path;
//             var tokens = FullPath.Split('.');
//             EndNode = tokens.Last();
//             Length = tokens.Length;
//         }
//
//         public bool IsRelated(string defPath) { return defPath.Contains(FullPath) || FullPath.Contains(defPath); }
//         public bool IsMatch(string defPath) { return FullPath == defPath; }
//         public bool IsParentOf(string defPath) { return defPath != FullPath && defPath.Contains(FullPath); }
//         public bool IsChildOf(string defPath) { return defPath != FullPath && FullPath.Contains(defPath); }
//     }
// }