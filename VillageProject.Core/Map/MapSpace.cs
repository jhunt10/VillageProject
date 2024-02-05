namespace VillageProject.Core.Map;

public class MapSpace
{
    public int MinX { get; private set; }
    public int MaxX { get; private set; }
    public int MinY { get; private set; }
    public int MaxY { get; private set; }
    public int MinZ { get; private set; }
    public int MaxZ { get; private set; }

    public bool InBounds(MapSpot spot)
    {
        return InBounds(spot.X, spot.Y, spot.Z);
    }

    public bool InBounds(int x, int y, int z)
    {
        return (x >= MinX && y >= MinY && z >= MinZ &&
                x <= MaxX && y <= MaxY && z <= MaxZ);
    }
}