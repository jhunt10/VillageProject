using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.Behavior;

public class BehaviorManager : BaseManager
{
    private List<string> _actors = new List<string>();

    public IInst InstantiateBehavior(IDef behaviorDef, IInst actorInstance)
    {
        var compDef = behaviorDef.GetComponentDefOfType<IBehaviorCompDef>();
        if (compDef == null)
            throw new Exception($"Failed to find {typeof(IBehaviorCompDef).Name} on def {behaviorDef.DefName}.");
        var actorComp = actorInstance.GetComponentOfType<ActorCompInst>();
        if (actorComp == null)
            throw new Exception($"Failed to find {typeof(ActorCompInst).Name} on inst {actorInstance._DebugId}.");
        var args = new Dictionary<string, object>();
        args.Add(compDef.CompKey, new BehaviorCompArgs(actorComp));
        return DimMaster.InstantiateDef(behaviorDef, args);
    }
    
    public override ICompInst CreateCompInst(ICompDef compDef, IInst newInst, object? args)
    {
        if (compDef is ActorCompDef)
        {
            _actors.Add(newInst.Id);
            return base.CreateCompInst(compDef, newInst, args);
        }
        else if(compDef is IBehaviorCompDef)
        {
            var behaviorArgs = args as BehaviorCompArgs;
            if (behaviorArgs == null)
                throw new Exception($"Invalid arguments for Comp {compDef.CompKey}.");
            return base.CreateCompInst(compDef, newInst, behaviorArgs);
        }
        else
        {
            throw new Exception($"Unhandled Def Type {compDef.CompDefClassName}");
        }
    }
}