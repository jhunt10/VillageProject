using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapSpaces;

namespace VillageProject.Godot.Map;

public interface IMapObjectNode
{
    // /// <summary>
    // /// Called when first placed on map.
    // /// </summary>
    // public void SetSprite(MapSpace mapSpace, MapSpot spot, RotationFlag rotation);
    
    // /// <summary>
    // /// Called when other things on map might have changed 
    // /// </summary>
    // public void UpdateSprite(MapSpace mapSpace, MapSpot spot, RotationFlag rotation);

    // /// <summary>
    // /// Flags sprite to be updated on next Process
    // /// </summary>
    // public void DirtySprite();

    public MapNode MapNode { get; set; }
    public void ForceUpdateSprite();
    public string? MapSpaceId { get; }
    public MapSpot? MapSpot { get; }
    public RotationFlag RealRotation { get; }
    public RotationFlag ViewRotation { get; }

    public void SetViewRotation(RotationFlag viewRotation);
}