namespace VillageProject.Core.Map;

public static class MapHelper
{
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