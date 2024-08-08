using Godot;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Sprites;
using VillageProject.Core.Sprites.Interfaces;

namespace VillageProject.Godot.Sprites;

public class GodotSpriteHelper : ISpriteLoader
{
    public void SetSpriteFromDef(Sprite2D targetSprite, IDef sourceDef, SpriteDataDef spriteDef)
    {
        var sprite = LoadSprite(sourceDef, spriteDef);
        targetSprite.Texture = (ImageTexture)sprite.Sprite;
        targetSprite.Offset = new Vector2(sprite.XOffset, -sprite.Hight + sprite.YOffset);
    }
    
    public void SetSpriteFromData(Sprite2D targetSprite, SpriteData sprite)
    {
        targetSprite.Texture = (ImageTexture)sprite.Sprite;
        targetSprite.Offset = new Vector2(sprite.XOffset, -sprite.Hight + sprite.YOffset);
    }
    
    
    public SpriteData LoadSprite(IDef sourceDef, SpriteDataDef spriteDef)
    {
        var spritePath = Path.Combine(sourceDef.LoadPath, spriteDef.SpriteName);

        var image = Image.LoadFromFile(spritePath);
        if (image == null)
            throw new Exception($"Failed to load image from '{spritePath}'.");
        
        return new SpriteData(ImageTexture.CreateFromImage(image), spriteDef);
    }
}