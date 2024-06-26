﻿using System.Numerics;
using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapSpaces;

namespace VillageProject.Core.Behavior;

public class ActorCompInst : BaseCompInst
{
    public const string ACTOR_MAP_LAYER = "Actor";
    public string Layer => ACTOR_MAP_LAYER;

    public MapPositionData? MapPosition;

    private BehaviorInst? _currentBehavior = null;
    
    public ActorCompInst(ICompDef def, IInst inst) : base(def, inst)
    {
        Active = true;
    }
    
    private void NotifyWatchers()
    {
        var watchers = Instance.ListComponentsOfType<IMapPlacementWatcherComp>(activeOnly:false);
        foreach (var watcherComp in watchers)
        {
            watcherComp.MapPositionSet(MapPosition);
        }
    }

    public IMapSpace GetMapSpace()
    {
        return MapPosition?.MapSpace ?? null;
    }

    public void SetMapPosition(MapPositionData mapPos)
    {
        if(MapPosition.HasValue && MapPosition.Value == mapPos)
            return;
        
        Active = true;
        MapPosition = mapPos;
        NotifyWatchers();
    }

    public float rotateTimer = 0;

    public override void Update(float delta)
    {
        base.Update(delta);
        // rotateTimer += delta;
        // if (rotateTimer > 5)
        // {
        //     this.SetMapSpot(this.GetMapSpace(), this.MapSpot.Value, Rotation.ApplyRotationDirection(RotationDirection.Clockwise));
        //     rotateTimer = 0;
        // }
        //
        if (_currentBehavior == null)
        {
            _currentBehavior = GetNextBehavior();
        }
        
        if (_currentBehavior != null)
        {
            _currentBehavior.Update(delta);
            if (_currentBehavior.GetState() == BehaviorState.Finished)
            {
                DimMaster.DeleteInst(_currentBehavior);
                _currentBehavior = null;
            }
            
        }
    }

    private BehaviorInst GetNextBehavior()
    {
        var behaviorManager = DimMaster.GetManager<BehaviorManager>();
        var def = DimMaster.GetDefByName("Defs.Behaviors.Common.Wander");
        var newBehavior = behaviorManager.InstantiateBehavior(def, this.Instance);
        return newBehavior;
    }
}