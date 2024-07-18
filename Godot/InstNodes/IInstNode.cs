using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Godot.Map;

namespace VillageProject.Godot.InstNodes;

public interface IInstNode
{
    public MapNode MapNode { get; set; }
    public IInst Inst { get; }
    public void SetInst(IInst inst);
    public void Delete();
    
    public void SetLayerVisibility(LayerVisibility visibility);
    public void SetViewRotation(RotationFlag viewRotation);
}