using VillageProject.Core.Behavior;

namespace VillageProject.Core.Map.Pathing;

public class MapPath
{ 
    public bool Successful { get; }
    public int Index { get; set; }

    private List<MapSpot> _spots;
    
    public MapPath(List<MapSpot>? mapSpots)
    {
        Index = 0;
        Successful = mapSpots?.Any() ?? false;
        _spots = mapSpots ?? new List<MapSpot>();
    }

    public MapSpot? CurrentSpot()
    {
        if (Index < _spots.Count)
        {
            return _spots[Index];
        }

        return null;
    }

    public MapSpot? NextSpot()
    {
        if (Index < _spots.Count)
        {
            Index++;
        }

        return CurrentSpot();

    }
    
    public IEnumerable<MapSpot> ListAllSpots()
    {
        foreach (var spot in _spots)
        {
            yield return spot;
        }
    }

}