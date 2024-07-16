using VillageProject.Core.DIM.Insts;

namespace VillageProject.Godot;

public interface Old_IInstNode
{
    public IInst Inst { get; }
    public void Delete();
}