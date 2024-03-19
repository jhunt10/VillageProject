using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.DIM;

/// <summary>
/// Interface for any class to be notified of the creation, loading, or destruction of Insts
/// Must be registerd with the DimMaster.AddInstWatcher()
/// </summary>
public interface IInstWatcher
{
    public void OnNewInstCreated(IInst inst);
    public void OnInstLoaded(IInst inst);
    public void OnInstDeleted(IInst inst);
}