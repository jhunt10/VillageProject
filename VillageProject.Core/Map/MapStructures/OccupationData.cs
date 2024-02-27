using System.Text.Json.Serialization;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Enums;
using VillageProject.Core.Serialization;

namespace VillageProject.Core.Map.MapStructures;


[JsonConverter(typeof(OccupationDataJsonConverter))]
public class OccupationData
{
    public MapSpot? AnchorSpot { get; }
    public RotationFlag Rotation { get; }

    internal Dictionary<MapSpot, List<OccupationFlags>> _occupiedSpots;

    public OccupationData(
        Dictionary<MapSpot, List<OccupationFlags>> occupiedSpots,
        MapSpot? anchorSpot = null, RotationFlag rotationFlag = RotationFlag.North,
        bool applyRotation = true)
    {
        AnchorSpot = anchorSpot;
        Rotation = rotationFlag;
        if (applyRotation)
            _occupiedSpots = RotateOccupationDict(anchorSpot ?? new MapSpot(), rotationFlag, occupiedSpots);
        else
            _occupiedSpots = occupiedSpots;
    }

    public IEnumerable<MapSpot> ListOccupiedSpots()
    {
        foreach (var spot in _occupiedSpots.Keys)
        {
            yield return spot;
        }
    }

    public IEnumerable<OccupationFlags> ListOccupationAtSpot(MapSpot spot)
    {
        if(_occupiedSpots.ContainsKey(spot))
            foreach (var occ in _occupiedSpots[spot])
            {
                yield return occ;
            }
    }

    public Dictionary<MapSpot, List<OccupationFlags>> RotateOccupationDict(
        MapSpot anchor,
        RotationFlag rotation, 
        Dictionary<MapSpot,List<OccupationFlags>> occupiedSpots)
    {
        var rotDir = RotationFlag.North.GetRotationDirection(rotation);
        var newDict = new Dictionary<MapSpot, List<OccupationFlags>>();
        foreach (var pair in occupiedSpots)
        {
            var spot = pair.Key;
            var occs = pair.Value;
            var newSpot = spot.RotateSpot(rotDir);
            var newOccs = new List<OccupationFlags>();
            foreach (var occ in occs)
            {
                newOccs.Add(occ.Rotate(rotDir));
            }
            newDict.Add(newSpot, newOccs);
        }
        return newDict;
    }
    
    
    
}