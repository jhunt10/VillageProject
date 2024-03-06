namespace VillageProject.Core.Sprites;

public class SpriteData
{
    public object Sprite { get; }
    public int Width { get; }
    public int Hight { get; }
    public int XOffset { get; }
    public int YOffset { get; }

    public SpriteData(object sprite, SpriteDataDef def)
    {
        Sprite = sprite ?? throw new ArgumentNullException(nameof(sprite));
        Width = def.Width;
        Hight = def.Hight;
        XOffset = def.XOffset;
        YOffset = def.YOffset;
    }
}