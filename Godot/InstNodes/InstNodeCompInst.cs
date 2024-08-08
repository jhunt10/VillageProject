using Godot;
using VillageProject.Core.Behavior;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapSpaces;
using VillageProject.Core.Map.MapStructures;
using VillageProject.Godot.Map;

namespace VillageProject.Godot.InstNodes;

public class InstNodeCompInst : BaseCompInst
{
    public InstNodeCompDef InstNodeCompDef { get; } 
    public IInstNode InstNode { get; private set; }
    public InstNodeCompInst(ICompDef def, IInst inst) : base(def, inst)
    {
        InstNodeCompDef = (InstNodeCompDef)def;
    }

    protected override void _Init()
    {
        this.Active = true;
        InstNode = GameMaster.CreateInstNodeForInst(this);
    }
    
    
    
    public LayerVisibility LayerVisibility { get; protected set; }
    public RotationFlag ViewRotation { get; protected set; }

    public void SetLayerVisibility(LayerVisibility visibility)
    {
        if (this.LayerVisibility != visibility)
        {
            this.LayerVisibility = visibility;
            Instance.FlagWatchedChange(MapStructChangeFlags.ViewRotationChanged);
        }
    }

    public void SetViewRotation(RotationFlag viewRotation)
    {
        if (this.ViewRotation != viewRotation)
        {
            this.ViewRotation = viewRotation;
            Instance.FlagWatchedChange(MapStructChangeFlags.ViewRotationChanged);
        }
    }
}