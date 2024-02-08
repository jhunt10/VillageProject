using VillageProject.Core.DIM.Defs;
namespace VillageProject.Core.Map.Terrain;

public class TerrainDef : BaseCompDef<TerrainCompInst, TerrainManager>
{
    public override bool RegisterOnLoad
    {
        get { return true; }
    }
}