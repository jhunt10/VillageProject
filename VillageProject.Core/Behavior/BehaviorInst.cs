using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.Behavior;

public enum BehaviorState
{
    Queued = 0,
    Active = 1,
    Paused = 2,
    Finished = 3,
}

public abstract class BehaviorInst : BaseInst
{
    private ActorCompInst _actor;
    private BehaviorState _curtState;
    private BehaviorState _nextState;
    private bool _beenFinished;
    private bool _beenPaused;
    
    public string CurrentMessage { get; protected set; }
    
    
    public BehaviorInst(IDef def, string id, DataDict data) : base(def, id)
    {
        var actorId = data.GetValueAs<string>("ActorId");
        var actorInst = DimMaster.GetInstById(actorId);
        var actorComp = actorInst?.GetComponentOfType<ActorCompInst>(activeOnly: false);
        if (actorComp == null)
            throw new Exception("Failed to find actor.");
        _actor = actorComp;
    }

    public ActorCompInst GetActorComp()
    {
        if (_actor == null)
        {
            _nextState = BehaviorState.Finished;
            CurrentMessage = "Failed to find Actor";
            throw new Exception("BehaviorComp failed to find actor on instance.");
        }

        return _actor;
    }

    protected abstract BehaviorState OnUpdate(float ticks);
    public override void Update(float ticks)
    {
        if (_curtState == BehaviorState.Paused && _nextState != BehaviorState.Paused)
            _beenPaused = false;
        // Delay each change of state so we get an update after we finish
        _curtState = _nextState;
        switch (_curtState)
        {
            case BehaviorState.Queued:
                var res = TryStart();
                if(res.Success)
                    _nextState = BehaviorState.Active;
                CurrentMessage = res.Message;
                break;
            case BehaviorState.Active:
                _nextState = OnUpdate(ticks);
                break;
            case BehaviorState.Paused:
                if (!_beenPaused)
                {
                    OnPause();
                    _beenPaused = true;
                }
                break;
            case BehaviorState.Finished:
                if (_beenFinished)
                    throw new Exception("Behavior updated after finishing");
                OnFinish();
                _beenFinished = true;
                break;
        }
    }

    protected abstract Result TryStart();
    protected abstract void OnPause();
    protected abstract void OnFinish();

    public BehaviorState GetState()
    {
        return _curtState;
    }
}