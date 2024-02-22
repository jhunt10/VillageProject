using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;

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

public class MapStructureManager : BaseManager, IMapStructureManager 
{
    private class MapCell
    {
        private Dictionary<string, List<IInst>> _layers = new Dictionary<string, List<IInst>>();

        public bool HasInst(string layer, IInst inst)
        {
            if (!_layers.ContainsKey(layer))
                return false;

            return _layers[layer].Contains(inst);
        }

        public void AddInst(string layer, IInst inst)
        {
            if(!_layers.ContainsKey(layer))
                _layers.Add(layer, new List<IInst>());
            if(!_layers[layer].Contains(inst))
                _layers[layer].Add(inst);
        }

        public void RemoveInst(string layer, IInst inst)
        {
            if(!_layers.ContainsKey(layer))
                return;
            if (_layers[layer].Contains(inst))
                _layers[layer].Remove(inst);
            if (!_layers[layer].Any())
                _layers.Remove(layer);

        }

        public IEnumerable<IInst> ListInst(string? layer)
        {
            if (!string.IsNullOrEmpty(layer))
            {
                if(_layers.ContainsKey(layer))
                    foreach (var inst in _layers[layer])
                        yield return inst;
            }
            else
            {
                foreach (var insts in _layers.Values)
                foreach (var inst in insts)
                    yield return inst;
            }
        }
    }

    private static Dictionary<MapSpot, MapCell> _cells;
    
    public void Init()
    {
        _cells = new Dictionary<MapSpot, MapCell>();
    }

    public ICompInst CreateCompInst(ICompDef compDef, IInst newInst, object? args)
    {
        var mapStructArgs = args as MapStructCompArgs;
        if (mapStructArgs == null)
            throw new Exception($"Failed to cast CompArgs to type '{typeof(MapStructCompArgs).FullName}'.");
        
        var compInst = base.CreateCompInst(compDef, newInst, args);
        var mapStructCompInst = compInst as MapStructCompInst;
        if (mapStructCompInst == null)
            throw new Exception($"Failed to cast new CompInst to type '{typeof(MapStructCompInst).FullName}'.");
        newInst.AddComponent(compInst);
        if(!TryPlaceMapStructure(newInst, mapStructArgs.Spot, mapStructArgs.Rotation))
            throw new Exception("Failed to place new MapStructure.");
        return compInst;
    }

    public IInst CreateMapStructureFromDef(IDef def, MapSpot spot, RotationFlag rotation,
        Dictionary<string, object>? otherArgs = null)
    {   
        var managedCompDefs = def.CompDefs
            .Where(x => x.ManagerClassName == this.GetType().FullName).ToList();
        
        if(!managedCompDefs.Any())
            throw new Exception($"No managed compdefs found on def '{def.DefName}'.");
        if (managedCompDefs.Count() > 1)
            throw new Exception($"Multiple managed compdefs found on def '{def.DefName}'.");

        var compDef = managedCompDefs.Single();
        var args = otherArgs ?? new Dictionary<string, object>();
        var compArgs = new MapStructCompArgs(spot, rotation);
        if (args.ContainsKey(compDef.CompKey))
            throw new Exception($"Key conflict between args provided and generated for comp '{compDef.CompKey}'.");
        args.Add(compDef.CompKey, compArgs);

        var inst = DimMaster.InstantiateDef(def, args);
        return inst;
    }

    public IEnumerable<IInst> ListInstsAtSpot(MapSpot spot, string? layer = null)
    {
        if (_cells.ContainsKey(spot))
            foreach (var inst in _cells[spot].ListInst(layer))
                yield return inst;
    }
    
    public void RemoveMapStructure(IInst inst)
    {
        var mapStructComp = inst.GetComponentOfType<MapStructCompInst>(errorIfNull:true);
        if (_cells.ContainsKey(mapStructComp.MapSpot))
        {
            var cell = _cells[mapStructComp.MapSpot];
            cell.RemoveInst(mapStructComp.Layer, inst);
        }
    }

    public bool CanPlaceMapStructure(IInst inst, MapSpot spot, RotationFlag rotation)
    {
        return true;
    }

    public bool TryPlaceMapStructure(IInst inst, MapSpot spot, RotationFlag rotation)
    {
        // message = "";
        var mapStructComp = inst.GetComponentOfType<MapStructCompInst>();
        if (mapStructComp == null)
        {
            // message = $"No {typeof(BaseMapStructCompInst).FullName} found.";
            return false;
        }
        var currentSpot = mapStructComp.MapSpot;
        if (currentSpot == spot)
            return true;
        
        RemoveMapStructure(inst);
        
        // This method and MapStructComp.SetMapSpot(spot) both call each other
        // So we don't want to call the other method unless something changed on our side
        
        // New spot never seen before
        if (!_cells.ContainsKey(spot))
        {
            _cells[spot] = new MapCell();
            var newCell = _cells[spot];
            newCell.AddInst(mapStructComp.Layer, inst);
            mapStructComp.SetMapSpot(spot, rotation);
            return true;
        }
        if(!_cells[spot].HasInst(mapStructComp.Layer, inst)) // Not seen in existing cell
        {
            _cells[spot].AddInst(mapStructComp.Layer, inst);
            mapStructComp.SetMapSpot(spot, rotation);
            return true;
        }

        return true;

    }

    public static IEnumerator<OccupationFlags> RotateOccupation(OccupationFlags[] arr)
    {
        throw new NotImplementedException();
    }

    public AdjacencyFlags GetAdjacency(
        IInst inst,
        IMapSpace mapSpace,
        MapSpot spot,
        RotationFlag rotation = RotationFlag.North,
        AdjacencyHelper.InstAdjacencyMatch? matchDelegate = null)
    {
        var mapStructComp = inst.GetComponentOfType<MapStructCompInst>(errorIfNull:true);
        var mapStructCompDef = mapStructComp.CompDef as MapStructCompDef;
        
        var adjacency = AdjacencyFlags.None;

        foreach (var adjPair in spot.ListAdjacentSpots(rotation))
        {
            var direction = adjPair.Key;
            var adjSpot = adjPair.Value;
            if(!_cells.ContainsKey(adjSpot))
                continue;
            foreach (var thing in _cells[adjSpot].ListInst(mapStructCompDef.MapLayer))
            {
                var match = false;
                if (matchDelegate != null)
                    match = matchDelegate(inst, thing);
                else
                    match = AdjacencyHelper.MatchSameDefDelegate(inst, thing);
                if (match)
                {
                    adjacency = adjacency | MapHelper.DirectionToAdjacency(direction);
                    break;
                }
            }
        }

        return adjacency;
    }
}