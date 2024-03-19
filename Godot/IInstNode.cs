using VillageProject.Core.DIM.Insts;

namespace VillageProject.Godot;

public interface IInstNode
{
    public IInst Inst { get; }
    public void Delete();
}