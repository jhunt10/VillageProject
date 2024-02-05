namespace VillageProject.Core.Map;

public class MapSpot
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public MapSpot(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}