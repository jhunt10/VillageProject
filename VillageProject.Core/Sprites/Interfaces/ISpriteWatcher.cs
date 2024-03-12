namespace VillageProject.Core.Sprites.Interfaces;

/// <summary>
/// Interface for game engine classes to notified when their sprite changes
/// </summary>
public interface ISpriteWatcher
{
    public void OnSpriteUpdate();
}