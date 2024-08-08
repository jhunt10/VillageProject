using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map.MapSpaces;

namespace VillageProject.Core.Map.MapStructures;

public class MapStructCompInst : BaseCompInst, IMapPositionComp
{
    public string? MapSpaceId => MapPosition?.MapSpaceId;
    public MapSpot? MapSpot => MapPosition?.MapSpot;
    public RotationFlag Rotation => MapPosition?.Rotation ?? RotationFlag.North;
    
    public MapPositionData? MapPosition { get; protected set; }
    public MapViewData? MapViewData { get; protected set; }

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
        var mapSpot = dataDict.GetValueAs<MapSpot>("MapSpot");
        var rotation = dataDict.GetValueAs<RotationFlag>("Rotation");
        OccupationData = MapStructDef.OccupationData.BuildNewOccupationData(MapSpot.Value, Rotation);

        var mapSpaceId = dataDict.GetValueAs<string>("MapSpaceId");
        var mapManager = DimMaster.GetManager<MapManager>();
        var mapSpace = mapManager.GetMapSpaceById(MapSpaceId, errorIfNull: false);
        if(mapSpace == null)
            Console.WriteLine("Failed to find MapSpace");
        else
        {
            var res = mapSpace.TryAddInstToSpots(Instance, OccupationData.ListOccupiedSpots().ToList(), Layer);
            if (!res.Success)
                throw new Exception("Failed to place self in spot: " + res.Message);
            MapPosition = new MapPositionData(mapSpace, mapSpot, rotation);
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
        
        var mapManager = DimMaster.GetManager<MapManager>();
        // Check if map knows we're here
        if (!mapManager.IsInstInPlacementQue(Instance))
        {
            var res = mapManager.TryPlaceInstOnMapSpace(mapPos.MapSpace, this.Instance, mapPos.MapSpot, mapPos.Rotation);
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
        if(this.MapSpot.HasValue && this.MapSpot.Value != mapPos.MapSpot)
            Instance.FlagWatchedChange(MapStructChangeFlags.MapPositionChanged);
        if(this.Rotation != mapPos.Rotation)
            Instance.FlagWatchedChange(MapStructChangeFlags.MapRotationChanged);
        this.MapPosition = mapPos;
        OccupationData = newOcc;

        
        NotifyWatchers();
        return new Result(true);
    }
}