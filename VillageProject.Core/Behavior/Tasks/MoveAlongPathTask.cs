using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.Pathing;

namespace VillageProject.Core.Behavior.Tasks;

public class MoveAlongPathTask : IBehaviorTask
{
    public BehaviorCompInst ParentBehavior { get; }
    private MapPath _path;
    private string _description;
    private float _subDistance;
    private MapSpot? _curtSpot;
    private MapSpot? _nextSpot;
    
    public MoveAlongPathTask(BehaviorCompInst parentBehavior, MapPath path, string targetName = null)
    {
        _subDistance = 0;
        ParentBehavior = parentBehavior;
        _path = path;
        // TODO: Language Translation
        if(!string.IsNullOrWhiteSpace(targetName))
            _description = $"Walking to {targetName}.";
        else
            _description = "Walking";
        _curtSpot = path.CurrentSpot();
        _nextSpot = path.NextSpot();
    }
    
    public bool Update(float ticks)
    {
        if (!_curtSpot.HasValue)
            return true;
        
        var actor = ParentBehavior.GetActorComp();
        _subDistance += ticks;
        if (_subDistance < 1)
        {
            var pos = PathHelper.GetPositionBetweenSpot(_curtSpot.Value, _nextSpot.Value, _subDistance);
            var rotation = RotationFlag.South;
            if(_curtSpot.HasValue )
                rotation = PathHelper.NextSpotToFacingRotation(_curtSpot.Value, _nextSpot.Value);
            actor.SetMapPosition(
                new MapPositionData(actor.GetMapSpace(), _curtSpot.Value, rotation, pos));

        }
        else
        {
            _subDistance = 0;
            
            var rotation = RotationFlag.South;
            if(_curtSpot.HasValue)
                rotation = PathHelper.NextSpotToFacingRotation(_curtSpot.Value, _nextSpot.Value);
            
            _curtSpot = _nextSpot;
            actor.SetMapPosition(
                new MapPositionData(actor.GetMapSpace(), _curtSpot.Value, rotation));
            
            _nextSpot = _path.NextSpot();
            
            if (_nextSpot == null)
                return true;

        }
        return false;
    }

    public string GetDescription()
    {
        return _description;
    }
}