namespace VillageProject.Core.Behavior;

public class BehaviorArgs
{
    public ActorCompInst Actor { get; }
    
    public BehaviorArgs(ActorCompInst actor)
    {
        Actor = actor ?? throw new ArgumentNullException(nameof(actor));
    }
}