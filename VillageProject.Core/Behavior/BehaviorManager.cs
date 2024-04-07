using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.Behavior;

public class BehaviorManager : BaseManager
{
    private List<string> _actors = new List<string>();

    public BehaviorInst InstantiateBehavior(IDef def, IInst actorInstance)
    {
        var behaviorDef = def as IBehaviorDef;
        if (behaviorDef == null)
            throw new Exception($"Failed to cast def {def.DefName} on to type IBehaviorDef.");
        var actorComp = actorInstance.GetComponentOfType<ActorCompInst>();
        if (actorComp == null)
            throw new Exception($"Failed to find {typeof(ActorCompInst).Name} on inst {actorInstance._DebugId}.");
        var args = new DataDict();
        args.AddData("ActorId", actorInstance.Id);
        var inst = DimMaster.InstantiateDef(behaviorDef, args);
        var bInst = inst as BehaviorInst;
        if (bInst == null)
            throw new Exception($"Failed to cast Inst '{inst._DebugId}' to BehaviorInst.");
        return bInst;
    }

    public override IInst CreateInst(IDef def, DataDict args)
    {
        return base.CreateInst(def, args);
    }

    public override ICompInst CreateCompInst(ICompDef compDef, IInst newInst, object? args)
    {
        if (compDef is ActorCompDef)
        {
            _actors.Add(newInst.Id);
            return base.CreateCompInst(compDef, newInst, args);
        }
        else if(compDef is IBehaviorDef)
        {
            var behaviorArgs = args as BehaviorArgs;
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