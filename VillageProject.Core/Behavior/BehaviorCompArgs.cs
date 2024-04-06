namespace VillageProject.Core.Behavior;

public class BehaviorCompArgs
{
    public ActorCompInst Actor { get; }
    
    public BehaviorCompArgs(ActorCompInst actor)
    {
        Actor = actor ?? throw new ArgumentNullException(nameof(actor));
    }
}