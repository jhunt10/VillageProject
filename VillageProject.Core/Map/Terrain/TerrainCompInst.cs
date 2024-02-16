using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.Map.Terrain;

public class TerrainCompInst : BaseCompInst, IAdjacentable
{
    public MapSpace MapSpace { get; }
    public AdjacencyType[] GetHorizontalAdjacency()
    {
        throw new NotImplementedException();
    }

    public AdjacencyType[] GetVerticalAdjacency()
    {
        throw new NotImplementedException();
    }

    public TerrainCompInst(ICompDef def, IInst inst) : base(def, inst)
    {
        
    }
}