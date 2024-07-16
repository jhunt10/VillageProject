using VillageProject.Core.DIM.Insts;

namespace VillageProject.Godot.InstNodes;

public interface IInstNode
{
    public MapNode MapNode { get; set; }
    public IInst Inst { get; }
    public void SetInst(IInst inst);
    public void Delete();
}