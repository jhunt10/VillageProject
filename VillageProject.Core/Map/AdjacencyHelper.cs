using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.Map;

public static class AdjacencyHelper
{
    public delegate bool InstAdjacencyMatch(IInst inst, IInst otherInst);

    public static InstAdjacencyMatch MatchSameDefDelegate = new InstAdjacencyMatch(MatchSameDef);
    public static bool MatchSameDef(IInst inst, IInst otherInst)
    {
        return inst.Def.DefName == otherInst.Def.DefName;
    }
}