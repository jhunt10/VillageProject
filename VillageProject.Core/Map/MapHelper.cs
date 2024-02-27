using VillageProject.Core.Enums;

namespace VillageProject.Core.Map;

public static class MapHelper
{
    /// <summary>
    /// Get the MapSpot of the cell visible under the world position.
    /// Assumes the coordinates are centered on bottom back left corner of cell at (0,0,0).
    /// ZLayers 
    /// </summary>
    /// <returns></returns>
    public static MapSpot? WorldPositionToMapSpot(
        MapSpace mapSpace, float worldX, float worldY, 
        int? visibleZLayer = null,
        RotationFlag mapFaceing = RotationFlag.North, 
        int cellWidth = 32, int cellDepth = 32, int cellHight = 40)
    {
        var x = Convert.ToInt32(Math.Floor(worldX / cellWidth));;
        if (x > mapSpace.MaxX || x < mapSpace.MinX)
            return null;
        
        for (int z = visibleZLayer ?? mapSpace.MaxZ; z >= mapSpace.MinZ; z--)
        {
            var zOffset = z * cellHight;
            var y = Convert.ToInt32(Math.Floor((-worldY + zOffset) / cellDepth));
            MapSpot spot = default(MapSpot);
            switch (mapFaceing)
            {
                case RotationFlag.North:
                    spot = new MapSpot(x, y, z);
                    break;
                case RotationFlag.East:
                    spot = new MapSpot(y, -x, z);
                    break;
                case RotationFlag.South:
                    spot = new MapSpot(-x, -y, z);
                    break;
                case RotationFlag.West:
                    spot = new MapSpot(-y, x, z);
                    break;
            }

            return spot;

            /*// Check if top of cell
            if (TerrainManager.GetTerrainAtSpot(MapSpace, spot) != null)
            {
                // Console.WriteLine($"TopFound: Mouse: {relativePos} | Spot: {spot}");
                return spot;
            }
			
            // Check if front of cell
            var backSpot = spot.DirectionToSpot(DirectionFlags.Back, ViewRotation);
            if (TerrainManager.GetTerrainAtSpot(MapSpace,backSpot) != null)
            {
                // Console.WriteLine($"FrontFound: Mouse: {relativePos} | Spot: {backSpot}");
                return backSpot;
            }

            // There is a gap between the bottom of where a tile top would be and where the bottom of the front sprite really is
            // We need to correct for this with an extra check
            var diff = relativePos.Y - ((y * TILE_WIDTH) - (z * TILE_HIGHT));
            if(z ==0)
                // Console.WriteLine($"Diff: {diff}");
                if ( diff < (TILE_HIGHT - TILE_WIDTH))
                {
                    var doubleBackSpot = backSpot.DirectionToSpot(DirectionFlags.Back, ViewRotation);
                    // Console.WriteLine($"Edge Case: Mouse: {relativePos}Checking Spot: {spot} | FrontSpot: {backSpot} | DoubleCheck: {doubleBackSpot} | Diff: {diff}");
                    if (TerrainManager.GetTerrainAtSpot(MapSpace,doubleBackSpot) != null)
                    {
                        return doubleBackSpot;
                    }
                }*/
        }
        return new MapSpot();
    }

    /// <summary>
    /// Get the int[x,y] pixel position for a given MapSpot.
    /// For MapSpot coordinates X increases to the right and Y increases towards top of screen.
    /// For PixelPosition X increases to the right and Y increases towards bottom of screen. 
    /// </summary>
    /// <returns>int[x,y]</returns>
    public static int[] MapSpotToWorldPosition(
        MapSpace mapSpace, MapSpot spot,
        RotationFlag mapRotation = RotationFlag.North,
        int celleWidth = 32, int cellDepth = 32, int cellHight = 40,
        bool invertY = true)
    {
        var pos = new int[]{0,0};
        var xPos = spot.X * celleWidth;
        var yPos = spot.Y * cellDepth;
        var zOffset = spot.Z * cellHight;
        // if (invertY)
        // {
        //     yPos = -yPos;
        //     zOffset = -zOffset;
        // }
        
        switch (mapRotation)
        {
            case RotationFlag.North:
                if(invertY)
                    pos = new int[]{xPos, -yPos - zOffset};
                else
                    pos = new int[]{xPos, yPos + zOffset};
                break;
            case RotationFlag.East:
                if(invertY)
                    pos = new int[]{-yPos, -xPos + zOffset};
                else
                    pos = new int[]{-yPos, xPos - zOffset};
                break;
            case RotationFlag.South:
                if(invertY)
                    pos = new int[]{-xPos, yPos - zOffset};
                else
                    pos = new int[]{-xPos, -yPos + zOffset};
                break;
            case RotationFlag.West:
                if(invertY)
                    pos = new int[]{yPos, xPos + zOffset};
                else
                    pos = new int[]{yPos, -xPos - zOffset};
                break;
        }

        return pos;

    }
    
    
    public static AdjacencyFlags DirectionToAdjacency(DirectionFlags dir)
    {
        switch (dir)
        {
            case DirectionFlags.None:
                return AdjacencyFlags.None;
            case DirectionFlags.BackLeft:
                return AdjacencyFlags.BackLeft;
            case DirectionFlags.Back:
                return AdjacencyFlags.Back;
            case DirectionFlags.BackRight:
                return AdjacencyFlags.BackRight;
            case DirectionFlags.Left:
                return AdjacencyFlags.Left;
            case DirectionFlags.Right:
                return AdjacencyFlags.Right;
            case DirectionFlags.FrontLeft:
                return AdjacencyFlags.FrontLeft;
            case DirectionFlags.Front:
                return AdjacencyFlags.Front;
            case DirectionFlags.FrontRight:
                return AdjacencyFlags.FrontRight;
            case DirectionFlags.Top:
                return AdjacencyFlags.Top;
            case DirectionFlags.TopBackLeft:
                return AdjacencyFlags.TopBackLeft;
            case DirectionFlags.TopBack:
                return AdjacencyFlags.TopBack;
            case DirectionFlags.TopBackRight:
                return AdjacencyFlags.TopBackRight;
            case DirectionFlags.TopLeft:
                return AdjacencyFlags.TopLeft;
            case DirectionFlags.TopRight:
                return AdjacencyFlags.TopRight;
            case DirectionFlags.TopFrontLeft:
                return AdjacencyFlags.TopFrontLeft;
            case DirectionFlags.TopFront:
                return AdjacencyFlags.TopFront;
            case DirectionFlags.TopFrontRight:
                return AdjacencyFlags.TopFrontRight;
            case DirectionFlags.Bottom:
                return AdjacencyFlags.Bottom;
            case DirectionFlags.BottomBackLeft:
                return AdjacencyFlags.BottomBackLeft;
            case DirectionFlags.BottomBack:
                return AdjacencyFlags.BottomBack;
            case DirectionFlags.BottomBackRight:
                return AdjacencyFlags.BottomBackRight;
            case DirectionFlags.BottomLeft:
                return AdjacencyFlags.BottomLeft;
            case DirectionFlags.BottomRight:
                return AdjacencyFlags.BottomRight;
            case DirectionFlags.BottomFrontLeft:
                return AdjacencyFlags.BottomFrontLeft;
            case DirectionFlags.BottomFront:
                return AdjacencyFlags.BottomFront;
            case DirectionFlags.BottomFrontRight:
                return AdjacencyFlags.BottomFrontRight;

        }

        throw new Exception($"Unrecognized Direction Flag: {dir}");
    }
}