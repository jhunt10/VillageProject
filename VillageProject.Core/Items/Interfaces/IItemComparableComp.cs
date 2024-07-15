using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.Items.Interfaces;

/// <summary>
/// Interface to allow other components to dictate if two Insts can be merged into one item stack.
/// </summary>
public interface IItemComparableComp : ICompInst
{
    /// <summary>
    /// Is this Item the "same" as the one provided, meaning the two Insts could be merged into one. 
    /// </summary>
    /// <param name="mergeIntoItem"></param>
    /// <returns></returns>
    public Result IsSameItem(IInst mergeIntoItem);
    
    
}