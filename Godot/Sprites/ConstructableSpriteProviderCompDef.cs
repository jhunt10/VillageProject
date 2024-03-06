using System.Collections.Generic;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Enums;
using VillageProject.Core.Sprites;

namespace VillageProject.Godot.Sprites;

public class ConstructableSpriteProviderCompDef : GenericCompDef<ConstructableSpriteProviderComp, SpriteManager>
{
    public Dictionary<RotationFlag, SpriteDataDef> PreviewSprites { get; set; }
}