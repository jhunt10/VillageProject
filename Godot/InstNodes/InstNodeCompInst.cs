using Godot;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Map;

namespace VillageProject.Godot.InstNodes;

public class InstNodeCompInst : BaseCompInst, IMapPlacementWatcherComp
{
    public static string PrefabScenePath = @"res://Scenes\Prefabs";
    public InstNodeCompDef InstNodeCompDef { get; } 
    public IInstNode InstNode { get; private set; }
    public InstNodeCompInst(ICompDef def, IInst inst) : base(def, inst)
    {
        InstNodeCompDef = (InstNodeCompDef)def;
    }

    public void LoadNodePrefab()
    {
        var scenePath = Path.Combine(PrefabScenePath, InstNodeCompDef.PrefabNodeName);
        var prefab = GD.Load<PackedScene>(scenePath);
        var newNode = prefab.Instantiate<Node2D>();
        InstNode = (IInstNode)newNode;
        InstNode.SetInst(this.Instance);
    }

    public void MapPositionSet(MapPositionData? mapPos)
    {
        if(InstNode == null)
            LoadNodePrefab();
        GameMaster.MapControllerNode.PlaceInstNodeOnMap(InstNode);
    }
}