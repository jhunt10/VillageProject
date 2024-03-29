﻿using VillageProject.Core.Enums;
using VillageProject.Core.Map.MapSpaces;

namespace VillageProject.Core.Map;

public static class MapHelper
{
    /// <summary>
    /// Get the MapSpot of the cell visible under the world position.
    /// Assumes the coordinates are centered on bottom back left corner of cell at (0,0,0).
    /// ZLayers 
    /// </summary>
    public static MapSpot? WorldPositionToMapSpot(
        IMapSpace mapSpace, float worldX, float worldY, 
        int? visibleZLayer = null,
        RotationFlag mapFaceing = RotationFlag.North, 
        int cellWidth = 32, int cellDepth = 32, int cellHight = 40)
    {
        // Console.WriteLine($"MapHelper.WorldPositionToMapSpot: worldX:{worldX} worldY:{worldY} ");
        if (mapSpace == null)
            return null;
        var x = Convert.ToInt32(Math.Floor(worldX / cellWidth));;
        if (x > mapSpace.MaxX || x < mapSpace.MinX)
            return null;
        
        for (int z = visibleZLayer ?? mapSpace.MaxZ; z >= mapSpace.MinZ; z--)
        {
            var zOffset = z * cellHight;
            
            // Fist check if we're over the top of a cell
            var y = Convert.ToInt32(Math.Floor((-worldY - zOffset - cellHight) / cellDepth));
            MapSpot spot = _getRotatedSpot(x, y, z, mapFaceing);
            if (mapSpace.ListInstsAtSpot(spot).Any())
                return spot;
            
            // Next, check we're on the front face of the cell behind us
            spot = spot.DirectionToSpot(DirectionFlag.Back, mapFaceing);
            if (mapSpace.ListInstsAtSpot(spot).Any())
                return spot;
            
            // Finnaly, check if we're in the bottom tile of a cell
            y = Convert.ToInt32(Math.Floor((-worldY - zOffset) / cellDepth));
            spot = _getRotatedSpot(x, y, z, mapFaceing);
            if (mapSpace.ListInstsAtSpot(spot).Any())
                return spot;
        }
        return null;
    }

    private static MapSpot _getRotatedSpot(int x, int y, int z, RotationFlag rotation)
    {
        switch (rotation)
        {
            case RotationFlag.North:
                return new MapSpot(x, y, z);
            case RotationFlag.East:
                return new MapSpot(y, -x, z);
                break;
            case RotationFlag.South:
                return new MapSpot(-x, -y, z);
                break;
            case RotationFlag.West:
                return new MapSpot(-y, x, z);
                break;
        }

        throw new Exception($"Unknown RotationFlag: {rotation}");
    }

    /// <summary>
    /// Get the int[x,y] pixel position for a given MapSpot.
    /// For MapSpot coordinates X increases to the right and Y increases towards top of screen.
    /// For PixelPosition X increases to the right and Y increases towards bottom of screen. 
    /// </summary>
    /// <returns>int[x,y]</returns>
    public static int[] MapSpotToWorldPosition(
        IMapSpace mapSpace, MapSpot spot,
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
                    pos = new int[]{-yPos, -xPos - zOffset};
                else
                    pos = new int[]{-yPos, xPos + zOffset};
                break;
            case RotationFlag.South:
                if(invertY)
                    pos = new int[]{-xPos, yPos - zOffset};
                else
                    pos = new int[]{-xPos, -yPos + zOffset};
                break;
            case RotationFlag.West:
                if(invertY)
                    pos = new int[]{yPos, xPos - zOffset};
                else
                    pos = new int[]{yPos, -xPos + zOffset};
                break;
        }

        return pos;

    }
}