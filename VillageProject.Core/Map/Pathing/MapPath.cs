namespace VillageProject.Core.Map.Pathing;

public class MapPath
{ 
    public List<MapSpot> Spots { get; }
    public MapPath(List<MapSpot> mapSpots)
    {
        Spots = mapSpots ?? throw new ArgumentException();
    }
}