namespace VillageProject.Core.Sprites;

public static class SpriteChangeFlags
{
    /// <summary>
    /// Change when this sprite needs to be updated
    /// </summary>
    public const string SpriteDirtied = "SpriteDirty";
    
    /// <summary>
    /// Change when this sprite is updated
    /// </summary>
    public const string SpriteRefreshed = "SpriteRefresh";
}