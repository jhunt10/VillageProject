using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.Map.MapSpaces;

public class MapSpaceCompInst : BaseCompInst, IMapSpace
{
    public string MapSpaceId => Instance.Id;
    public int MinX { get; private set; }
    public int MaxX { get; private set; }
    public int MinY { get; private set; }
    public int MaxY { get; private set; }
    public int MinZ { get; private set; }
    public int MaxZ { get; private set; }

    /// <summary>
    /// Matrix if cells ordered in [z][x][y]
    /// </summary>
    private MapCell[][][] _cellMatrix;
    
    private Dictionary<string, List<MapSpot>> _inst_to_spots = new Dictionary<string, List<MapSpot>>();

    public MapSpaceCompInst(ICompDef def, IInst inst) : base(def, inst)
    {
        var compDef = (MapSpaceCompDef)def;
        MinX = compDef.MinX;
        MaxX = compDef.MaxX;
        MinY = compDef.MinY;
        MaxY = compDef.MaxY;
        MinZ = compDef.MinZ;
        MaxZ = compDef.MaxZ;
        _buildCellMatrix(MaxX, MinX, MaxY, MinY, MaxZ, MinZ);
        Active = true;
    }
    
    public override DataDict? BuildSaveData()
    {
        var data = new DataDict(MapSpaceId);
        var cellData = new Dictionary<string, Dictionary<string, List<string>>>();
        foreach (var spot in EnumerateMapSpots())
        {
            var cellSave = _getCellAtSpot(spot).BuildSaveData();
            if(cellSave != null)
                cellData.Add(spot.ToString(),cellSave);
        }
        data.AddData("CellData", cellData);
        data.AddData("MaxX", MaxX);
        data.AddData("MinX", MinX);
        data.AddData("MaxY", MaxY);
        data.AddData("MinY", MinY);
        data.AddData("MaxZ", MaxZ);
        data.AddData("MinZ", MinZ);
        return data;
    }

    public override void LoadSavedData(DataDict dataDict)
    {
        MinX = dataDict.GetValueAs<int>("MinX");
        MaxX = dataDict.GetValueAs<int>("MaxX");
        MinY = dataDict.GetValueAs<int>("MinY");
        MaxY = dataDict.GetValueAs<int>("MaxY");
        MinZ = dataDict.GetValueAs<int>("MinZ");
        MaxZ = dataDict.GetValueAs<int>("MaxZ");
        _inst_to_spots = new Dictionary<string, List<MapSpot>>();
        _buildCellMatrix(MaxX, MinX, MaxY, MinY, MaxZ, MinZ);

        var cellData = dataDict.GetValueAs<Dictionary<string, Dictionary<string, List<string>>>>("CellData");
        foreach (var pair in cellData)
        {
            var spot = new MapSpot(pair.Key);
            var layerData = pair.Value;
            var cell = _getCellAtSpot(spot);
            foreach (var layer in layerData)
            foreach (var instId in layer.Value)
                cell.AddInstId(layer.Key, instId);
        }
        base.LoadSavedData(dataDict);
    }


    private void _buildCellMatrix(int maxX, int minX, int maxY, int minY, int maxZ, int minZ)
    {
        MaxX = maxX; 
        MinX = minX;
        MaxY = maxY; 
        MinY = minY;
        MaxZ = maxZ; 
        MinZ = minZ;
        var width = maxX - minX + 1;
        var depth = maxY - minY + 1;
        var hight = maxZ - minZ + 1;
        _cellMatrix = new MapCell[hight][][];
        for (int z = 0; z < hight; z++)
        {
            _cellMatrix[z] = new MapCell[width][];
            for (int x = 0; x < width; x++)
            {
                _cellMatrix[z][x] = new MapCell[depth];
                for (int y = 0; y < depth; y++)
                    _cellMatrix[z][x][y] = new MapCell();
            }
        }
    }

    private MapCell? _getCellAtSpot(MapSpot spot)
    {
        if (!InBounds(spot))
            return null;
        return _cellMatrix[spot.Z - MinZ][spot.X - MinX][spot.Y - MinY];
    }

    public bool InBounds(MapSpot spot)
    {
        return InBounds(spot.X, spot.Y, spot.Z);
    }

    public bool InBounds(int x, int y, int z)
    {
        return (x >= MinX && y >= MinY && z >= MinZ &&
                x <= MaxX && y <= MaxY && z <= MaxZ);
    }

    public IEnumerable<MapSpot> EnumerateMapSpots()
    {
        for(int z = MinZ; z <= MaxZ; z++)
        for(int x = MinX; x <= MaxX; x++)
        for (int y = MinY; y <= MaxY; y++)
            yield return new MapSpot(x, y, z);
    }
    
    public IEnumerable<IInst> EnumerateAllInsts()
    {
        foreach (var instsId in _inst_to_spots.Keys)
        {
            var inst = DimMaster.GetInstById(instsId, errorIfNull:true);
            yield return inst;
        }
    }

    public IEnumerable<IInst> ListInstsAtSpot(MapSpot spot, string? layer = null)
    {
        var cell = _getCellAtSpot(spot);
        if(cell != null)
            foreach (var instId in cell.ListInstIds(layer))
            {
                var inst = DimMaster.GetInstById(instId);
                if (inst == null)
                {
                    //TODO: handle lost insts
                    continue;
                }

                yield return inst;
            }
    }
    
    public IEnumerable<TComp> ListCompInstsOfTypeAtSpot<TComp>(MapSpot spot, string? layer = null)
        where TComp : ICompInst
    {
        var cell = _getCellAtSpot(spot);
        if(cell != null)
            foreach (var instId in cell.ListInstIds(layer))
            {
                var inst = DimMaster.GetInstById(instId);
                if (inst == null)
                {
                    //TODO: handle lost insts
                    continue;
                }

                foreach (var comp in inst.ListComponentsOfType<TComp>())
                {
                    yield return comp;
                }
            }
    }

    /// <summary>
    /// Try registering the given instance to the map cells defined by spots.
    /// Does not check for any mananged logic. Instead use MapManager.TryPlaceInstOnMapSpace()
    /// </summary>
    /// <param name="inst"></param>
    /// <param name="spots"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public Result TryAddInstToSpots(IInst inst, List<MapSpot> spots, string layer)
    {
        if (!_inst_to_spots.ContainsKey(inst.Id))
            _inst_to_spots.Add(inst.Id, new List<MapSpot>());
        
        Result? failedRes = null;
        foreach (var spot in spots)
        {
            if (!InBounds(spot))
            {
                RemoveInst(inst);
                return new Result(false, $"Spot {spot} is out of bounds.");
            }

            var cell = _getCellAtSpot(spot);
            cell.AddInst(layer, inst);
            _inst_to_spots[inst.Id].Add(spot);
        }

        // Console.WriteLine($"Added Inst {inst._DebugId} to spot [{string.Join(", ", spots.Select(x => x.ToString()))}].");
        Instance.FlagWatchedChange(MapSpaceChangeFlags.HeldInstsChanged);
        return new Result(true);
    }

    public void RemoveInst(IInst inst)
    {
        if(!_inst_to_spots.ContainsKey(inst.Id))
            return;
        foreach (var spot in _inst_to_spots[inst.Id])
        {
            var cell = _getCellAtSpot(spot);
            if(cell != null)
                cell.RemoveInst(inst);
        }
        _inst_to_spots[inst.Id].Clear();
        _inst_to_spots.Remove(inst.Id);
        Instance.FlagWatchedChange(MapSpaceChangeFlags.HeldInstsChanged);
    }

    private class MapCell
    {
        private Dictionary<string, List<string>> _layers = new Dictionary<string, List<string>>();

        
        public Dictionary<string, List<string>>? BuildSaveData()
        {
            if(_layers.Any())  
                return _layers;
            return null;

        }
        
        public bool HasInst(string layer, IInst inst)
        {
            if (!_layers.ContainsKey(layer))
                return false;

            return _layers[layer].Contains(inst.Id);
        }

        public void AddInst(string layer, IInst inst)
        {
            if(!_layers.ContainsKey(layer))
                _layers.Add(layer, new List<string>());
            if(!_layers[layer].Contains(inst.Id))
                _layers[layer].Add(inst.Id);
        }
        
        internal void AddInstId(string layer, string instId)
        {
            if(!_layers.ContainsKey(layer))
                _layers.Add(layer, new List<string>());
            if(!_layers[layer].Contains(instId))
                _layers[layer].Add(instId);
        }

        public void RemoveInst(IInst inst)
        {
            foreach (var pair in _layers)
            {
                var layer = pair.Value;
                if (layer.Contains(inst.Id))
                    layer.Remove(inst.Id);
                if (!layer.Any())
                    _layers.Remove(pair.Key);
            }

        }

        public IEnumerable<string> ListInstIds(string? layer)
        {
            if (!string.IsNullOrEmpty(layer))
            {
                if(_layers.ContainsKey(layer))
                    foreach (var instId in _layers[layer])
                        yield return instId;
            }
            else
            {
                foreach (var insts in _layers.Values)
                foreach (var instId in insts)
                    yield return instId;
            }
        }
    }
    private class MyClass
    {
            
    }
}