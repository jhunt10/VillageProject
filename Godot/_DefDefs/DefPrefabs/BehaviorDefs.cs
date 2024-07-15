using VillageProject.Core.Behavior;
using VillageProject.Core.Behavior.CommonBehaviors;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Enums;
using VillageProject.Core.Map.MapSpaces;
using VillageProject.Core.Sprites;
using VillageProject.Godot.Actors;

namespace VillageProject.Godot.DefDefs.DefPrefabs;

public static class BehaviorDefs
{
    public static IDef TestActor = new ObjectDef()
    {
        DefName = "Defs.Testing.TestActor",
        Label = "TestActor",
        CompDefs = new List<ICompDef>()
        {
            new ActorCompDef()
            {

            },
            new GodotActorSpriteCompDef()
            {
                DefaultSprite = new SpriteDataDef("test_actor_south.png", 32, 40, 0, 0),
                RotationSprites = new Dictionary<RotationFlag, SpriteDataDef>
                {
                    { RotationFlag.North, new SpriteDataDef("test_actor_north.png", 32, 40, 0, 0)},
                    { RotationFlag.East, new SpriteDataDef("test_actor_east.png", 32, 40, 0, 0)},
                    { RotationFlag.South, new SpriteDataDef("test_actor_south.png", 32, 40, 0, 0)},
                    { RotationFlag.West, new SpriteDataDef("test_actor_west.png", 32, 40, 0, 0)},
                }
            }
        }
    };
    
    public static IDef WanderBehaviorDef = new WanderBehaviorDef()
    {
        DefName = "Defs.Behaviors.Common.Wander",
        Label = "Wander",
        CompDefs = new System.Collections.Generic.List<ICompDef>
        {
            
        }
    };
    
    public static Dictionary<string, IDef> Defs = new Dictionary<string, IDef>
    {
        {"TestActor", TestActor},
        { "Wander", WanderBehaviorDef },
    };
}