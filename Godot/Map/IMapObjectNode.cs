using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapSpaces;

namespace VillageProject.Godot.Map;

public interface IMapObjectNode : Old_IInstNode
{
    public MapNode MapNode { get; set; }
    public void ForceUpdateSprite();
    public string? MapSpaceId { get; }
    public MapSpot? MapSpot { get; }
    public RotationFlag RealRotation { get; }
    public RotationFlag ViewRotation { get; }
    public LayerVisibility LayerVisibility { get; }

    public void SetLayerVisibility(LayerVisibility visibility);
    public void SetViewRotation(RotationFlag viewRotation);
}