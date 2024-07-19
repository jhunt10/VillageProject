using Godot;
using VillageProject.Core.Behavior;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapSpaces;
using VillageProject.Core.Map.MapStructures;

namespace VillageProject.Godot.InstNodes;

public class InstNodeCompInst : BaseCompInst
{
    public static string PrefabScenePath = @"res://Scenes\Prefabs";
    public InstNodeCompDef InstNodeCompDef { get; } 
    public IInstNode InstNode { get; private set; }
    public InstNodeCompInst(ICompDef def, IInst inst) : base(def, inst)
    {
        InstNodeCompDef = (InstNodeCompDef)def;
    }

    public override void Update(float delta)
    {
        if(Instance.GetWatchedChange("GodotInstNode:MapStruct"))
            GameMaster.MapControllerNode.PlaceInstNodeOnMap(InstNode);
    }

    protected override void _Init()
    {
        this.Active = true;
        var scenePath = Path.Combine(PrefabScenePath, InstNodeCompDef.PrefabNodeName);
        var prefab = GD.Load<PackedScene>(scenePath);
        var newNode = prefab.Instantiate<Node2D>();
        InstNode = (IInstNode)newNode;
        InstNode.SetInst(this.Instance);
        GameMaster.Instance.CallDeferred("add_child", (Node2D)InstNode);

        var mapStructComp = Instance.GetComponentOfType<MapStructCompInst>(activeOnly: false);
        if(mapStructComp != null)
            Instance.AddComponentWatcher<MapStructCompInst>("GodotInstNode:MapStruct", mapStructComp.Active);
    }
}