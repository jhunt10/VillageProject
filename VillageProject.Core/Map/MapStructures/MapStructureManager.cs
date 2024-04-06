using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map.MapSpaces;

namespace VillageProject.Core.Map.MapStructures;

public class MapStructCompArgs
{
    public MapSpot Spot { get; }
    public RotationFlag Rotation { get; }

    public MapStructCompArgs(MapSpot spot, RotationFlag rotation)
    {
        Spot = spot;
        Rotation = rotation;
    }
}

public class MapStructureManager : BaseManager, IMapPlacementValidator
{
    public const string DEFAULT_MAP_LAYER = "Default";

    public void Init()
    {
        
    }

    public Result CouldPlaceDefOnMapSpace(IMapSpace mapSpace, IDef def, MapSpot anchorSpot, RotationFlag rotation, object args)
    {
        var mapStructCompDef = def.GetComponentDefOfType<MapStructCompDef>();
        if (mapStructCompDef == null)
            return new Result(true);
        
        // Console.WriteLine($"CouldPlaceDefOnMapSpace: Checking Occupation");
        var occupation = new OccupationData(mapStructCompDef.OccupationData.OccupationDict, anchorSpot, rotation);
        foreach (var occSpot in occupation.ListOccupiedSpots())
        {
            if (!mapSpace.InBounds(occSpot))
                return new Result(false, "Out of bounds.");
            
            foreach (var existingOcc in AggregateOccupationAtSpot(mapSpace, occSpot))
            foreach (var newOcc in occupation.ListOccupationAtSpot(occSpot))
            {
                // Console.WriteLine($"CouldPlaceDefOnMapSpace: {occSpot} [{newOcc}] [{existingOcc}]");
                if (newOcc.OverlapsFlags(existingOcc))
                    return new Result(false, "Occupation Collision");
            }
        }
        return new Result(true);
    }

    Result IMapPlacementValidator.CanPlaceInstOnMapSpace(IMapSpace mapSpace, IInst inst, MapSpot anchorSpot, RotationFlag rotation, object args)
    {
        var mapStrutCompInst = inst.GetComponentOfType<MapStructCompInst>(activeOnly:false);
        if (mapStrutCompInst == null)
            return new Result(true);
        return CouldPlaceDefOnMapSpace(mapSpace, inst.Def, anchorSpot, rotation, args);
    }

    Result IMapPlacementValidator.TryPlaceInstOnMapSpace(IMapSpace mapSpace, IInst inst, MapSpot anchorSpot, RotationFlag rotation, object args)
    {
        var mapStrutCompInst = inst.GetComponentOfType<MapStructCompInst>(activeOnly:false);
        if (mapStrutCompInst == null)
            return new Result(true);

        var mapStructCompDef = (MapStructCompDef)mapStrutCompInst.CompDef;
        var occupation = mapStructCompDef.OccupationData.BuildNewOccupationData(anchorSpot, rotation);
        var occSpots = occupation.ListOccupiedSpots().ToList();
        var res = mapSpace.TryAddInstToSpots(inst, occSpots, mapStructCompDef.MapLayer);
        mapStrutCompInst.SetMapSpot(mapSpace, anchorSpot, rotation);
        return res;
    }
    
    public List<OccupationFlags> AggregateOccupationAtSpot(IMapSpace mapSpace, MapSpot spot)
    {
        var innerOccupation = OccupationFlags.Inner;
        var midOccupation = OccupationFlags.Middle;
        var outerOccupation = OccupationFlags.Outer;

        foreach (var inst in mapSpace.ListInstsAtSpot(spot))
        {
            var mapStructComp = inst.GetComponentOfType<MapStructCompInst>();
            if(mapStructComp == null)
                continue;
            foreach (var occ in mapStructComp.OccupationData.ListOccupationAtSpot(spot))
            {
                if(occ.ContainsFlag(OccupationFlags.Inner))
                    innerOccupation = innerOccupation | occ;
                if(occ.ContainsFlag(OccupationFlags.Middle))
                    midOccupation = midOccupation | occ;
                if(occ.ContainsFlag(OccupationFlags.Outer))
                    outerOccupation = outerOccupation | occ;
            }
        }

        innerOccupation = innerOccupation.RemoveFlag(OccupationFlags.Middle);
        innerOccupation = innerOccupation.RemoveFlag(OccupationFlags.Outer);
        
        midOccupation = midOccupation.RemoveFlag(OccupationFlags.Inner);
        midOccupation = midOccupation.RemoveFlag(OccupationFlags.Outer);
        
        outerOccupation = outerOccupation.RemoveFlag(OccupationFlags.Inner);
        outerOccupation = outerOccupation.RemoveFlag(OccupationFlags.Middle);

        return new List<OccupationFlags>() { innerOccupation, midOccupation, outerOccupation };
    }

    // public IInst CreateMapStructureFromDef(IDef def, MapSpot spot, RotationFlag rotation,
    //     Dictionary<string, object>? otherArgs = null)
    // {   
    //     var managedCompDefs = def.CompDefs
    //         .Where(x => x.ManagerClassName == this.GetType().FullName).ToList();
    //     
    //     if(!managedCompDefs.Any())
    //         throw new Exception($"No managed compdefs found on def '{def.DefName}'.");
    //     if (managedCompDefs.Count() > 1)
    //         throw new Exception($"Multiple managed compdefs found on def '{def.DefName}'.");
    //
    //     var compDef = managedCompDefs.Single();
    //     var args = otherArgs ?? new Dictionary<string, object>();
    //     var compArgs = new MapStructCompArgs( spot, rotation);
    //     if (args.ContainsKey(compDef.CompKey))
    //         throw new Exception($"Key conflict between args provided and generated for comp '{compDef.CompKey}'.");
    //     args.Add(compDef.CompKey, compArgs);
    //
    //     var inst = DimMaster.InstantiateDef(def, args);
    //     return inst;
    // }

    // public IEnumerable<IInst> ListInstsAtSpot(MapSpot spot, string? layer = null)
    // {
    //     // if (_cells.ContainsKey(spot))
    //     //     foreach (var inst in _cells[spot].ListInst(layer))
    //     //         yield return inst;
    //     if (false)
    //         yield return null;
    // }
    
    // public void RemoveMapStructure(IInst inst)
    // {
    //     // var mapStructComp = inst.GetComponentOfType<MapStructCompInst>(errorIfNull:true);
    //     // if (_cells.ContainsKey(mapStructComp.MapSpot))
    //     // {
    //     //     var cell = _cells[mapStructComp.MapSpot];
    //     //     cell.RemoveInst(mapStructComp.Layer, inst);
    //     // }
    // }

    // public bool CanPlaceMapStructure(IInst inst, MapSpot spot, RotationFlag rotation)
    // {
    //     return true;
    // }
    //
    // public bool TryPlaceMapStructure(IInst inst, MapSpot spot, RotationFlag rotation)
    // {
    //     // message = "";
    //     // var mapStructComp = inst.GetComponentOfType<MapStructCompInst>();
    //     // if (mapStructComp == null)
    //     // {
    //     //     // message = $"No {typeof(BaseMapStructCompInst).FullName} found.";
    //     //     return false;
    //     // }
    //     // var currentSpot = mapStructComp.MapSpot;
    //     // if (currentSpot == spot)
    //     //     return true;
    //     //
    //     // RemoveMapStructure(inst);
    //     //
    //     // // This method and MapStructComp.SetMapSpot(spot) both call each other
    //     // // So we don't want to call the other method unless something changed on our side
    //     //
    //     // // New spot never seen before
    //     // if (!_cells.ContainsKey(spot))
    //     // {
    //     //     _cells[spot] = new MapCell();
    //     //     var newCell = _cells[spot];
    //     //     newCell.AddInst(mapStructComp.Layer, inst);
    //     //     mapStructComp.SetMapSpot(spot, rotation);
    //     //     return true;
    //     // }
    //     // if(!_cells[spot].HasInst(mapStructComp.Layer, inst)) // Not seen in existing cell
    //     // {
    //     //     _cells[spot].AddInst(mapStructComp.Layer, inst);
    //     //     mapStructComp.SetMapSpot(spot, rotation);
    //     //     return true;
    //     // }
    //
    //     return true;
    //
    // }

    public CellSideFlags GetAdjacency(
        IInst inst,
        IMapSpace mapSpace,
        MapSpot spot,
        RotationFlag rotation = RotationFlag.North,
        AdjacencyHelper.InstAdjacencyMatch? matchDelegate = null)
    {
        var mapStructComp = inst.GetComponentOfType<MapStructCompInst>(errorIfNull:true);
        var mapStructCompDef = mapStructComp.CompDef as MapStructCompDef;
        
        var adjacency = CellSideFlags.None;
    
        foreach (var adjPair in spot.ListAdjacentSpots(rotation))
        {
            var direction = adjPair.Key;
            var adjSpot = adjPair.Value;
            if(!mapSpace.InBounds(adjSpot))
                continue;
            foreach (var thing in mapSpace.ListInstsAtSpot(adjSpot, mapStructCompDef.MapLayer))
            {
                var match = false;
                if (matchDelegate != null)
                    match = matchDelegate(inst, thing);
                else
                    match = AdjacencyHelper.MatchSameDefDelegate(inst, thing);
                if (match)
                {
                    adjacency = adjacency | direction.ToCellSide();
                    break;
                }
            }
        }
    
        return adjacency;
    }
}