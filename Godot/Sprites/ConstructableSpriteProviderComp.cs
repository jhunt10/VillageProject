using System;
using System.IO;
using Godot;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Sprites;
using VillageProject.Core.Sprites.Interfaces;

namespace VillageProject.Godot.Sprites;

public class ConstructableSpriteProviderComp : BaseSpriteComp, IConstructableSpriteProvider
{
    public ConstructableSpriteProviderComp(ICompDef def, IInst inst) : base(def, inst)
    {
    }

    public override SpriteData GetSprite()
    {
        throw new System.NotImplementedException();
    }

    public SpriteData GetConstructableIconSprite()
    {
        throw new System.NotImplementedException();
    }

    public SpriteData GetConstructablePreviewSprite(MapSpace mapSpace, MapSpot spot, RotationFlag rotation)
    {
        var def = (ConstructableSpriteProviderCompDef)CompDef;
        var spriteDef = def.PreviewSprites[rotation];
        var spritePath = Path.Combine(def.ParentDef.LoadPath, spriteDef.SpriteName);
        
        var image = Image.LoadFromFile(spritePath);
        if (image == null)
            throw new Exception($"Failed to load image from '{spritePath}'.");

        return new SpriteData(ImageTexture.CreateFromImage(image), spriteDef);
    }
}