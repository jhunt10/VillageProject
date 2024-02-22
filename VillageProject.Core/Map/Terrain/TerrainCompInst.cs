using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.Map.Terrain;

public class TerrainCompInst : BaseCompInst
{
    public MapSpace MapSpace { get; }
    

    public TerrainCompInst(ICompDef def, IInst inst) : base(def, inst)
    {
        
    }
}