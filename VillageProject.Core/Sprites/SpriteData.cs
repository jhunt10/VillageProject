namespace VillageProject.Core.Sprites;

public class SpriteData
{
    public object Sprite { get; }

    public SpriteData(object sprite)
    {
        Sprite = sprite ?? throw new ArgumentNullException(nameof(sprite));
    }
}