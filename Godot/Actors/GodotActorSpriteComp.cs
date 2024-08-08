using Godot;
using VillageProject.Core.Behavior;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map.MapStructures;
using VillageProject.Core.Sprites;
using VillageProject.Core.Sprites.Actors;
using VillageProject.Godot.Sprites;

namespace VillageProject.Godot.Actors;

public class GodotActorSpriteCompInst : BaseActorSpriteCompInst
{
    public GodotActorSpriteCompInst(ICompDef def, IInst inst) : base(def, inst)
    {
        Active = true;
    }

    protected override SpriteData _UpdateSprite()
    {
        var actorComp = Instance.GetComponentOfType<ActorCompInst>();
        if (actorComp == null)
            throw new Exception($"No ActorCompInst found on IInst '{Instance._DebugId}'.");
        var def = (GodotActorSpriteCompDef)this.CompDef;
        var spriteDef = def.DefaultSprite;

        var rotaion = actorComp.MapPosition.Value.Rotation.SubtractRotation(ViewRotation);
        if (def.RotationSprites.ContainsKey(rotaion))
            spriteDef = def.RotationSprites[rotaion];
        
        var spritePath = Path.Combine(def.ParentDef.LoadPath, spriteDef.SpriteName);

        var image = Image.LoadFromFile(spritePath);
        if (image == null)
            throw new Exception($"Failed to load image from '{spritePath}'.");
        
        return new SpriteData(ImageTexture.CreateFromImage(image), spriteDef);
        
    }
}