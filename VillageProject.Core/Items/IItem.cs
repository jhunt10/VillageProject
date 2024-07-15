using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.Items;

/// <summary>
/// TODO: Delete me
/// This is just a scaffolding interface for sketching out items
/// </summary>
public interface IItem
{
    public IInst SplitStack(int count);
    public void MergeWithStack(ItemCompInst item);
}