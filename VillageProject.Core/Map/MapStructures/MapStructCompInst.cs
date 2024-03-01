using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;

namespace VillageProject.Core.Map.MapStructures;

public class MapStructCompInst : BaseCompInst
{
    public MapSpot? MapSpot { get; protected set; }
    public RotationFlag Rotation { get; protected set; }

    public string Layer => this.GetCompDefAs<MapStructCompDef>().MapLayer;
    
    public OccupationData OccupationData { get; private set; }

    public MapStructCompInst(ICompDef def, IInst inst) : base(def, inst)
    {
        
    }

    public void SetMapSpot(MapSpace mapSpace, MapSpot spot, RotationFlag rotaion)
    {
        if(MapSpot == spot && Rotation == rotaion)
            return;
        MapSpot = spot;
        Rotation = rotaion;
        
        // var manager = DimMaster.GetManager<MapStructureManager>();
        // if (manager.TryPlaceMapStructure(Instance, spot, rotaion))
        // {
        //     MapSpot = spot;
        //     Rotation = rotaion;
        // }
    }
}