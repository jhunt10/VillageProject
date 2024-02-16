namespace VillageProject.Core.Map;


public enum AdjacencyType
{
    None = 0,
    Same = 1,
    Different = 2
}

/// <summary>
/// Interface for CompInst which are concerned with adjacency to other Insts.
/// Made for PatchCellSprites but may have other uses
/// </summary>
public interface IAdjacentable
{
    /// <summary>
    /// Check adjacency of cells on same z level.
    /// </summary>
    /// <returns>AdjacencyType[9]</returns>
    AdjacencyType[] GetHorizontalAdjacency();
    
    
    /// <summary>
    /// Check adjacency of cells to the left, right, above, or bellow.
    /// </summary>
    /// <returns>AdjacencyType[9]</returns>
    AdjacencyType[] GetVerticalAdjacency();
}