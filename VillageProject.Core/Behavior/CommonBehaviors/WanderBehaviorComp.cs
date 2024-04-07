using VillageProject.Core.Behavior.Tasks;
using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Map;
using VillageProject.Core.Map.Pathing;

namespace VillageProject.Core.Behavior.CommonBehaviors;

public class WanderBehaviorDef : BehaviorDef<WanderBehaviorInst>
{
        
}

public class WanderBehaviorInst : BehaviorInst
{
    private MoveAlongPathTask? _task;
    
    public WanderBehaviorInst(IDef def, string id, DataDict args) : base(def, id, args)
    {
    }

    protected override Result TryStart()
    {
        var actorComp = GetActorComp();
        if (!actorComp.MapPosition.HasValue)
            return new Result(false, "Actor is not on map");
        var mapSpace = actorComp.GetMapSpace();
        var curSpot = actorComp.MapPosition.Value.MapSpot;
        // Get a random spot
        var ran = new Random((int)DateTime.Now.Ticks);
        var x = curSpot.X + ran.Next(-5, 5);
        var y = curSpot.Y + ran.Next(-5, 5);
        var z = curSpot.Z;
        x = Math.Min(mapSpace.MaxX, Math.Max(mapSpace.MinX, x));
        y = Math.Min(mapSpace.MaxY, Math.Max(mapSpace.MinY, y));
        var path = PathFinder.FindPath(mapSpace, actorComp.Instance, curSpot, new MapSpot(x, y, z), cacheSearchedCells: true);
        if (!path.Successful)
        {
            var realSpot = PathFinder.CachedSearchedCells.MaxBy(pair => pair.Key.DistanceToSpot(curSpot)).Value;
            path = PathFinder.FindPath(mapSpace, actorComp.Instance, curSpot, realSpot.Spot);
        }

        _task = new MoveAlongPathTask(this, path);
        return new Result(true);
    }
    protected override BehaviorState OnUpdate(float ticks)
    {
        if (_task == null)
            throw new Exception("Task has not be set.");

        try
        {
            if (_task.Update(ticks))
                return BehaviorState.Finished;
            else
                return BehaviorState.Active;
        }
        catch (Exception e)
        {
            CurrentMessage = "Errored: " + e.Message;
            return BehaviorState.Finished;
        }
    }

    protected override void OnPause()
    {
        
    }

    protected override void OnFinish()
    {
    }

    public override DataDict BuildSaveData()
    {
        throw new NotImplementedException();
    }

    public override void LoadSavedData(DataDict dataDict)
    {
        throw new NotImplementedException();
    }

    public override void _Delete()
    {
        throw new NotImplementedException();
    }
}