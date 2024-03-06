namespace VillageProject.Core.Sprites;

public class SpriteDataDef
{
    public string SpriteName { get; set; }
    public int Width { get; set; }
    public int Hight { get; set; }
    public int XOffset { get; set; }
    public int YOffset { get; set; }
    
    public SpriteDataDef() {}

    public SpriteDataDef(string spriteName, int width, int hight, int xOffset, int yOffset)
    {
        SpriteName = spriteName;
        Width = width;
        Hight = hight;
        XOffset = xOffset;
        YOffset = yOffset;
    }
}