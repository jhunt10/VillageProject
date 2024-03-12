using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Enums;
using VillageProject.Core.Sprites;

namespace VillageProject.Godot.Sprites;

public class GodotMapStructSpriteCompDef : GenericCompDef<GodotMapStructSpriteComp, SpriteManager>
{
    public SpriteDataDef DefaultSprite;
    public Dictionary<RotationFlag, SpriteDataDef> RotationSprites { get; set; }
}