using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Map.MapSpaces;

namespace VillageProject.Core.Map.Terrain;

public class TerrainCompInst : BaseCompInst
{
    public MapSpaceCompInst MapSpaceCompInst { get; }
    

    public TerrainCompInst(ICompDef def, IInst inst) : base(def, inst)
    {
        
    }
}