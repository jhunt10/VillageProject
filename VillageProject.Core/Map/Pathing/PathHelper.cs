using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map.MapSpaces;

namespace VillageProject.Core.Map.Pathing;

public static class PathHelper
{
    //-----------------------------
    // Path finding
    //-----------------------------

    public static IEnumerable<MapSpot> GetPathAdjacentSpots(IMapSpace mapSpace, MapSpot mapSpot)
    {
        foreach (var adjPair in mapSpot.ListAdjacentSpots(includeVertical:false, includeCenter:false))
        {
            var adjSpot = adjPair.Value;
            if (!mapSpace.InBounds(adjSpot))
                continue;
            
            var exitDirection = adjPair.Key;
            var enterDirection = exitDirection.Rotate(RotationDirection.HalfTurn);

            // Check that exiting in this direction is pathable
            if(!_IsPathInDirectionIsSupportedAndUnblocked(mapSpace, mapSpot, exitDirection))
                continue;
            
            // Check the entering the next cell is pathable
            if(!_IsPathInDirectionIsSupportedAndUnblocked(mapSpace, adjSpot, enterDirection))
                continue;
            
            yield return adjSpot;
        }

        // TODO: Get Bridgeable spots
        // var mapObjs = space.GetPathHooksAtSpot(spot);
        // foreach(var obj in mapObjs)
        // {
        //     if(obj is IPathBridge)
        //     {
        //         var pathBrige = obj as IPathBridge;
        //         foreach (var attached in pathBrige.GetSpotsConnectedTo(spot))
        //             yield return attached;
        //     }
        // }
    }

    private static bool _IsPathInDirectionIsSupportedAndUnblocked(IMapSpace mapSpace, MapSpot mapSpot, DirectionFlag direction)
    {
        var pathingSide = direction.ToCellSide();
        var isSupported = false;
        // Check supported paths through this cell (and grab blocking while we're at it)
        foreach (var comp in mapSpace.ListCompInstsOfTypeAtSpot<PathingCompInst>(mapSpot))
        {
            if (comp.GetBlockedPathThrough(mapSpot).HasFlag(pathingSide))
                return false;
            if (comp.GetSupportedPathThrough(mapSpot).HasFlag(pathingSide))
                isSupported = true;
        }

        if (isSupported)
            return true;
        
        // Check supported paths over the cell bellow 
        var underSpot = mapSpot + new MapSpot(0, 0, -1);
        foreach (var underComp in mapSpace.ListCompInstsOfTypeAtSpot<PathingCompInst>(underSpot))
        {
            if (underComp.GetSupportedPathOver(underSpot).HasFlag(pathingSide))
                return true;
        }

        return false;
    }


    // ///
}