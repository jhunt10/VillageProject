using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Enums;
using VillageProject.Core.Sprites;

namespace VillageProject.Godot.Actors;

public class GodotActorSpriteCompDef : GenericCompDef<GodotActorSpriteCompInst, SpriteManager>
{
    public SpriteDataDef DefaultSprite;
    public Dictionary<RotationFlag, SpriteDataDef> RotationSprites { get; set; }
}