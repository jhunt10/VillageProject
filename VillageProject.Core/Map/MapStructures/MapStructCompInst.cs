using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map.MapSpaces;

namespace VillageProject.Core.Map.MapStructures;

public class MapStructCompInst : BaseCompInst, IMapPositionComp
{
    public string? MapSpaceId { get; protected set; }
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
        data.AddData("MapSpaceId", MapSpaceId);
        return data;
    }

    public override void LoadSavedData(DataDict dataDict)
    {
        MapSpot = dataDict.GetValueAs<MapSpot>("MapSpot");
        Rotation = dataDict.GetValueAs<RotationFlag>("Rotation");
        OccupationData = MapStructDef.OccupationData.BuildNewOccupationData(MapSpot.Value, Rotation);

        MapSpaceId = dataDict.GetValueAs<string>("MapSpaceId");
        var mapManager = DimMaster.GetManager<MapManager>();
        var mapSpace = mapManager.GetMapSpaceById(MapSpaceId, errorIfNull: false);
        if(mapSpace == null)
            Console.WriteLine("Failed to find MapSpace");
        else
        {
            var res = mapSpace.TryAddInstToSpots(Instance, OccupationData.ListOccupiedSpots().ToList(), Layer);
            if (!res.Success)
                throw new Exception("Failed to place self in spot: " + res.Message);
            
        }
    }

    private void NotifyWatchers()
    {
        var watchers = Instance.ListComponentsOfType<IMapPlacementWatcherComp>(activeOnly:false);
        foreach (var watcherComp in watchers)
        {
            var mapSpace = DimMaster.GetManager<MapManager>().GetMapSpaceById(MapSpaceId);
            watcherComp.MapPositionSet(new MapPositionData(mapSpace, MapSpot.Value, Rotation));
        }
    }

    public Result TrySetMapPosition(MapPositionData mapPos)
    {
        if(mapPos.MapSpaceId == MapSpaceId && MapSpot == mapPos.MapSpot && Rotation == mapPos.Rotation)
            return new Result(true, "Already at position");
        
        // Check if map knows we're here
        var here = mapPos.MapSpace.ListInstsAtSpot(mapPos.MapSpot).Any(x => x.Id == this.Instance.Id);
        if (!here)
        {
            var mapManager = DimMaster.GetManager<MapManager>();
            var res = mapManager.TryPlaceInstOnMapSpace(mapPos.MapSpace, this.Instance, mapPos.MapSpot, mapPos.Rotation);
            if (!res.Success)
                return res;
        }
        
        // Validate that the map knows we are here
        var newOcc = MapStructDef.OccupationData.BuildNewOccupationData(mapPos.MapSpot, mapPos.Rotation);
        foreach (var newSpot in newOcc.ListOccupiedSpots())
        {
            if (!mapPos.MapSpace.ListInstsAtSpot(newSpot).Contains(this.Instance))
            {
                throw new Exception(
                    $"MapStruct {Instance.Def.DefName}:{Instance.Id} was not added to MapSpace correctly");
            }
        }
        
        Active = true;
        MapSpaceId = mapPos.MapSpaceId;
        MapSpot = mapPos.MapSpot;
        Rotation = mapPos.Rotation;
        OccupationData = newOcc;

        NotifyWatchers();
        return new Result(true);
    }
}