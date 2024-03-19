using Godot;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapStructures;
using VillageProject.Core.Sprites;
using VillageProject.Core.Sprites.Interfaces;
using VillageProject.Core.Sprites.MapStructures;

namespace VillageProject.Godot.Sprites;

public class GodotMapStructSpriteComp : BaseMapStructureSpriteComp
{
    public GodotMapStructSpriteComp(ICompDef def, IInst inst) : base(def, inst)
    {
    }

    protected override SpriteData _UpdateSprite()
    {
        var mapStructComp = Instance.GetComponentOfType<MapStructCompInst>();
        if (mapStructComp == null)
            throw new Exception($"No MapStructCompInst found on IInst '{Instance._DebugId}'.");
        var def = (GodotMapStructSpriteCompDef)this.CompDef;
        var spriteDef = def.DefaultSprite;

        var rotaion = mapStructComp.Rotation.SubtractRotation(ViewRotation);
        if (def.RotationSprites.ContainsKey(rotaion))
            spriteDef = def.RotationSprites[rotaion];
        
        var spritePath = Path.Combine(def.ParentDef.LoadPath, spriteDef.SpriteName);

        var image = Image.LoadFromFile(spritePath);
        if (image == null)
            throw new Exception($"Failed to load image from '{spritePath}'.");
        
        return new SpriteData(ImageTexture.CreateFromImage(image), spriteDef);
        
    }
}