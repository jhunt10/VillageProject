using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Map.MapSpaces;

namespace VillageProject.Core.Map.Pathing;

public static class PathFinder
{
    /// <summary>
    /// Cells that where searched and the number of times they were hit in the last path found. Good for debugging.
    /// </summary>
    public static Dictionary<MapSpot, PathSearchNode> CachedSearchedCells { get; private set; } 
        = new Dictionary<MapSpot, PathSearchNode>();

    public static MapPath FindPath(IMapSpace mapSpace, IInst actor, MapSpot start, MapSpot targetSpots,
        bool cacheSearchedCells = false)
    {
        return FindPath(mapSpace, actor, start, new List<MapSpot> { targetSpots }, cacheSearchedCells);
    }

    // Why Multiple Targets? An actor will often need to move near a target, adjacent but not on.
    // There may be a spot near the target and close to the actor, but that isn't a good place to stand.
    // A different spot could be higher quality, but slightly further away so it's found 2nd.
    // We could path to each spot separately and compare, but searching all spots at once means less processing.
    public static MapPath FindPath(IMapSpace mapSpace, IInst actor, MapSpot start, List<MapSpot> targetSpots, 
        bool cacheSearchedCells = false)
    {
        var willCheck = new List<PathSearchNode>();
        var beenChecked = new Dictionary<MapSpot, PathSearchNode>();
        CachedSearchedCells.Clear();

        beenChecked.Add(start, new PathSearchNode
        {
            CameFrom = start,
            Spot = start,
            ToHereCost = 0,
            ToTargetCost = 0
        });
        willCheck.Add(beenChecked[start]);

        // Process everything left in que to look for a better last node
        bool foundAcceptableEnd = false;

        while (willCheck.Any())
        {
            var checkingNode = willCheck.OrderBy(x => x.TotalCost).First();
            willCheck.Remove(checkingNode);

            // Finish out search que incase of better spot
            if (targetSpots.Contains(checkingNode.Spot))
                foundAcceptableEnd = true;

            var adjSpots = PathHelper.GetPathAdjacentSpots(mapSpace, checkingNode.Spot).ToList();
            foreach(var adjcentSpot in adjSpots)
            {
                // If already in que, skip
                if (willCheck.Any(x => x.Spot == adjcentSpot))
                    continue;

                var moveToCost = CalcMoveToCost(mapSpace, checkingNode, adjcentSpot);
                var toTargetCost = float.MaxValue;
                foreach(var target in targetSpots)
                {
                    var tempCost = CalcToTargetCost(mapSpace, adjcentSpot, target);
                    if (tempCost < toTargetCost)
                        toTargetCost = tempCost;
                }

                var spotQuality = CalcSpotQuality(mapSpace, adjcentSpot);

                var newNode = new PathSearchNode
                {
                    CameFrom = checkingNode.Spot,
                    Spot = adjcentSpot,
                    ToHereCost = moveToCost,
                    ToTargetCost = toTargetCost,
                    SpotQuality = spotQuality
                };

                // Add node if we haven't been to this spot before
                if (!beenChecked.ContainsKey(adjcentSpot))
                {
                    beenChecked.Add(adjcentSpot, newNode);
                    // Don't add to que if we've already found the end 
                    if (!foundAcceptableEnd)
                        willCheck.Add(newNode);
                    // Unless this is an alternate route to the end
                    else if(targetSpots.Contains(newNode.Spot))
                        willCheck.Add(newNode);
                }
                // Update node if this path to the spot is better
                else if(beenChecked[adjcentSpot].ToHereCost > newNode.ToHereCost)
                {
                    beenChecked[adjcentSpot] = newNode;
                    // Don't add to que if we've already found the end 
                    if (!foundAcceptableEnd)
                        willCheck.Add(newNode);
                    // Unless this is an alternate route to the end
                    else if(targetSpots.Contains(newNode.Spot))
                        willCheck.Add(newNode);
                }
            }
        }

        // Cache our search if asked to
        if(cacheSearchedCells)
            foreach (var checkedPair in beenChecked)
            {
                CachedSearchedCells.Add(checkedPair.Key, checkedPair.Value);
            }

        if (!beenChecked.Any(x => targetSpots.Contains(x.Key)))
            return null;

        var endNode = beenChecked.First(x => targetSpots.Contains(x.Key)).Value;
        foreach(var targSpot in targetSpots)
        {
            if (beenChecked.ContainsKey(targSpot) && 
                (beenChecked[targSpot].ToHereCost < endNode.ToHereCost ||
                beenChecked[targSpot].SpotQuality > endNode.SpotQuality))
            {
                endNode = beenChecked[targSpot];
            }
        }

        return BuildFinalPath(beenChecked, start, endNode.Spot);
    }

    private static float CalcMoveToCost(IMapSpace mapSpace, PathSearchNode oldNode, MapSpot newSpot)
    {
        float moveCost = 0f;
        foreach (var pathingComp in mapSpace.ListCompInstsOfTypeAtSpot<PathingCompInst>(oldNode.Spot))
        {
            moveCost += pathingComp.GetPathCost(oldNode.Spot, newSpot);
        }
        var distanceCost = MathF.Abs(oldNode.Spot.X - newSpot.X) +
                           MathF.Abs(oldNode.Spot.Y - newSpot.Y) +
                           MathF.Abs(oldNode.Spot.Z - newSpot.Z);
        return oldNode.ToHereCost + distanceCost;
    }


    private static float CalcToTargetCost(IMapSpace mapSpace, MapSpot currentSpot, MapSpot target)
    {
        // For now just a distance calculation
        // return 0f;
        // MathF.Abs(currentSpot.X - target.X) +
            //MathF.Abs(currentSpot.Y - target.Y) +
            //MathF.Abs(currentSpot.Z - target.Z);
        return MathF.Sqrt(MathF.Pow(currentSpot.X - target.X, 2f)
                + MathF.Pow(currentSpot.Y - target.Y, 2f)
                + MathF.Pow(currentSpot.Z - target.Z, 2f));
    }

    private static float CalcSpotQuality(IMapSpace mapSpace, MapSpot newSpot)
    {
        // TODO: Terain and path costs
        // if (mapSpace.GetObjectsAtSpot(newSpot).Any())
        //     return -100f;
        return 0;
    }

    private static MapPath BuildFinalPath(Dictionary<MapSpot, PathSearchNode> beenFound, MapSpot start, MapSpot target)
    {
        var outList = new List<MapSpot>();
        var curNode = beenFound[target];
        while(curNode.Spot != start)
        {
            outList.Add(curNode.Spot);
            curNode = beenFound[curNode.CameFrom];
        }
        outList.Add(start);
        outList.Reverse();

        return new MapPath(outList);
    }
    
    public class PathSearchNode
    {
        public MapSpot Spot;
        public MapSpot CameFrom;
        public float ToHereCost;
        public float ToTargetCost;
        public float SpotQuality;

        public float TotalCost { get { return ToHereCost + ToTargetCost; } }
    }
}