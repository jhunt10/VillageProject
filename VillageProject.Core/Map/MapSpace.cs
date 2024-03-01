﻿using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Map.Terrain;

namespace VillageProject.Core.Map;

public class MapSpace : IMapSpace
{
    public int MinX { get; private set; }
    public int MaxX { get; private set; }
    public int MinY { get; private set; }
    public int MaxY { get; private set; }
    public int MinZ { get; private set; }
    public int MaxZ { get; private set; }

    public MapSpace(int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
    {
        MinX = minX;
        MaxX = maxX;
        MinY = minY;
        MaxY = maxY;
        MinZ = minZ;
        MaxZ = maxZ;
        _buildCellMatrix(MaxX, MinX, MaxY, MinY, MaxZ, MinZ);
    }
    
    /// <summary>
    /// Matrix if cells ordered in [z][x][y]
    /// </summary>
    private MapCell[][][] _cellMatrix;
    
    private Dictionary<string, List<MapSpot>> _inst_to_spots = new Dictionary<string, List<MapSpot>>();

    private void _buildCellMatrix(int maxX, int minX, int maxY, int minY, int maxZ, int minZ)
    {
        MaxX = maxX; MinX = minX;
        MaxY = maxY; MinY = minY;
        MaxZ = maxZ; MinZ = minZ;
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

    public Result TryAddInstToSpots(IInst inst, List<MapSpot> spots, string layer)
    {
        if (_inst_to_spots.ContainsKey(inst.Id))
            return new Result(false, $"Inst {inst.Def.DefName}:{inst.Id} already added to MapSpace.");
        _inst_to_spots.Add(inst.Id, new List<MapSpot>());
        Result? failedRes = null;
        foreach (var spot in spots)
        {
            if (InBounds(spot))
            {
                var cell = _getCellAtSpot(spot);
                cell.AddInst(layer, inst);
                _inst_to_spots[inst.Id].Add(spot);
            }
            else
            {
                failedRes = new Result(false, $"Spot {spot} is out of bounds.");
                break;
            }
        }

        if (failedRes != null)
        {
            RemoveInst(inst);
            return failedRes;
        }

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
    }

    // public void RemoveInstFromSpots(MapSpot spot, IInst inst)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public IInst? GetTerrainAtSpot(MapSpot spot)
    // {
    //     var cell = _getCellAtSpot(spot);
    //     if (cell == null)
    //         return null;
    //     var terrainIndex = cell.TerrainIndex;
    //     if (!terrainIndex.HasValue)
    //         return null;
    //
    //     if (!_terrainLibrary.ContainsKey(terrainIndex.Value))
    //         throw new Exception($"Unknown terrain index {terrainIndex.Value} at spot {spot}.");
    //
    //     var terrainName = _terrainLibrary[terrainIndex.Value];
    //     var terrainManager = DimMaster.GetManager<TerrainManager>();
    //     var terrainInst = terrainManager.GetTerrainByName(terrainName);
    //     if(terrainInst == null)
    //         throw new Exception($"No terrain found with name {terrainName}.");
    //     return terrainInst;
    // }
    //
    // public void SetTerrainAtSpot(IInst terrain, MapSpot spot)
    // {
    //     var terrainName = terrain.Def.DefName;
    //     
    //     var cell = _getCellAtSpot(spot);
    //     if(cell == null)
    //         return;
    //     cell.AddInst(TerrainManager.TERRAIN_LAYER, terrain);
    // }
    
    private class MapCell
    {
        private Dictionary<string, List<string>> _layers = new Dictionary<string, List<string>>();

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
}