using VillageProject.Core.DIM.Defs;

namespace VillageProject.Godot.InstNodes;

public class InstNodeCompDef : GenericCompDef<InstNodeCompInst, InstNodeManager>
{
    public string PrefabNodeName { get; set; }
}