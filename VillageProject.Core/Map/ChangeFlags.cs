namespace VillageProject.Core.Map;

public static class MapStructChangeFlags
{
    /// <summary>
    /// Change when this MapStruct is moved
    /// </summary>
    public const string MapPositionChanged = "MapPos";
    /// <summary>
    /// Change when this MapStruct is rotated
    /// </summary>
    public const string MapRotationChanged = "MapRot";
}

public static class MapSpaceChangeFlags
{
    /// <summary>
    /// Change when this MapSpace adds or removes an Inst
    /// </summary>
    public const string HeldInstsChanged = "MapSpaInst";
}