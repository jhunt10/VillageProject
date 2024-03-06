using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;

namespace VillageProject.Core.Map.MapStructures;

public class MapStructCompInst : BaseCompInst
{
    public MapSpace MapSpace { get; protected set; }
    public MapSpot? MapSpot { get; protected set; }
    public RotationFlag Rotation { get; protected set; }

    public string Layer => this.GetCompDefAs<MapStructCompDef>().MapLayer;
    
    public OccupationData OccupationData { get; private set; }
    
    protected MapStructCompDef MapStructDef => CompDef as MapStructCompDef;

    public MapStructCompInst(ICompDef def, IInst inst) : base(def, inst)
    {
        
    }

    public override DataDict? BuildSaveData()
    {
        var data = new DataDict(CompKey);
        data.AddData("MapSpot", MapSpot);
        data.AddData("Rotation", Rotation);
        return data;
    }

    public override void LoadSavedData(DataDict dataDict)
    {
        MapSpot = dataDict.GetValueAs<MapSpot>("MapSpot");
        Rotation = dataDict.GetValueAs<RotationFlag>("Rotation");
        OccupationData = MapStructDef.OccupationData.BuildNewOccupationData(MapSpot.Value, Rotation);
    }

    public void SetMapSpot(MapSpace mapSpace, MapSpot spot, RotationFlag rotation)
    {
        if(MapSpace == mapSpace && MapSpot == spot && Rotation == rotation)
            return;
        
        // Validate that the map knows we are here
        var newOcc = MapStructDef.OccupationData.BuildNewOccupationData(spot, rotation);
        foreach (var newSpot in newOcc.ListOccupiedSpots())
        {
            if (!mapSpace.ListInstsAtSpot(newSpot).Contains(this.Instance))
            {
                throw new Exception(
                    $"MapStruct {Instance.Def.DefName}:{Instance.Id} was not added to MapSpace correctly");
            }
        }

        MapSpace = mapSpace;
        MapSpot = spot;
        Rotation = rotation;
        OccupationData = newOcc;
    }
}