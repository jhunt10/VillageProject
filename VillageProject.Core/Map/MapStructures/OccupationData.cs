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

    public Dictionary<MapSpot, List<OccupationFlags>> OccupationDict { get; }

    public OccupationData(
        Dictionary<MapSpot, List<OccupationFlags>> occupiedSpots,
        MapSpot? anchorSpot = null, RotationFlag rotationFlag = RotationFlag.North)
    {
        AnchorSpot = anchorSpot;
        Rotation = rotationFlag;
        OccupationDict = _RotateOccupationDict(anchorSpot ?? new MapSpot(), rotationFlag, occupiedSpots);
    }

    public IEnumerable<MapSpot> ListOccupiedSpots()
    {
        foreach (var spot in OccupationDict.Keys)
        {
            yield return spot;
        }
    }

    public IEnumerable<OccupationFlags> ListOccupationAtSpot(MapSpot spot)
    {
        if(OccupationDict.ContainsKey(spot))
            foreach (var occ in OccupationDict[spot])
            {
                yield return occ;
            }
    }

    public OccupationData BuildNewOccupationData(MapSpot anchor, RotationFlag rotationFlag)
    {
        return new OccupationData(this.OccupationDict, anchor, Rotation);
    }

    private Dictionary<MapSpot, List<OccupationFlags>> _RotateOccupationDict(
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
            var newSpot = spot.RotateSpot(rotDir) + anchor;
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