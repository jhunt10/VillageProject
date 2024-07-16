using Godot;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Sprites;

namespace VillageProject.Godot.Sprites;

public static class GodotSpriteHelper
{
    public static void SetSpriteFromDef(Sprite2D targetSprite, IDef sourceDef, SpriteDataDef spriteDef)
    {
        var spritePath = Path.Combine(sourceDef.LoadPath, spriteDef.SpriteName);

        var image = Image.LoadFromFile(spritePath);
        if (image == null)
            throw new Exception($"Failed to load image from '{spritePath}'.");

        targetSprite.Texture = ImageTexture.CreateFromImage(image);
        targetSprite.Offset = new Vector2(spriteDef.XOffset, -spriteDef.Hight + spriteDef.YOffset);
    }
}