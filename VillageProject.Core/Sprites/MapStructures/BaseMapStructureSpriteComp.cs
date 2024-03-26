using VillageProject.Core.DIM;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapSpaces;

namespace VillageProject.Core.Sprites.MapStructures;

public abstract class BaseMapStructureSpriteComp : BaseSpriteComp, IMapPlacementWatcherComp
{
    public BaseMapStructureSpriteComp(ICompDef def, IInst inst) : base(def, inst)
    {
        
    }

    public void MapPositionSet(IMapSpace mapSpaceCompInst, MapSpot mapSpot, RotationFlag rotation)
    {
        DirtySprite();
    }
    
    // /// <summary>
    // /// Lower value means closer to front (SouthTop)
    // /// </summary>
    // public static float OccupationToZOffset(MapOccupationData occupationData, float scale)
    // {
    //     float outVal = 0f;
    //     var occupation = occupationData.OccupationAt(occupationData.Anchor);
    //     var checkSpot = CellOccupation.FullCell;
    //
    //     #region Floor and Back Wall
    //
    //     // Floor
    //     checkSpot = (CellOccupation.Bottom | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // West Floor Wall
    //     checkSpot = (CellOccupation.WestBottom | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // East Floor Wall
    //     checkSpot = (CellOccupation.EastBottom | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //
    //     outVal += scale;
    //
    //     // North Wall
    //     checkSpot = (CellOccupation.North | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // North West Wall
    //     checkSpot = (CellOccupation.NorthWest | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // North East Wall
    //     checkSpot = (CellOccupation.NorthEast | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //
    //     outVal += scale;
    //
    //     // North Bottom Wall
    //     checkSpot = (CellOccupation.NorthBottom | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // North West Bottom Wall
    //     checkSpot = (CellOccupation.NorthWestBottom | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // North East Bottom Wall
    //     checkSpot = (CellOccupation.NorthEastBottom | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //
    //     outVal += scale;
    //
    //     #endregion
    //
    //     #region Mid Floor and Back Mid
    //
    //     // Floor
    //     checkSpot = (CellOccupation.Bottom | CellOccupation.MidSpace);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // West Floor Wall
    //     checkSpot = (CellOccupation.WestBottom | CellOccupation.MidSpace);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // East Floor Wall
    //     checkSpot = (CellOccupation.EastBottom | CellOccupation.MidSpace);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //
    //     outVal += scale;
    //
    //     // North Wall
    //     checkSpot = (CellOccupation.North | CellOccupation.MidSpace);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // North West Wall
    //     checkSpot = (CellOccupation.NorthWest | CellOccupation.MidSpace);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // North East Wall
    //     checkSpot = (CellOccupation.NorthEast | CellOccupation.MidSpace);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //
    //     outVal += scale;
    //
    //     // North Bottom Wall
    //     checkSpot = (CellOccupation.NorthBottom | CellOccupation.MidSpace);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // North West Bottom Wall
    //     checkSpot = (CellOccupation.NorthWestBottom | CellOccupation.MidSpace);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // North East Bottom Wall
    //     checkSpot = (CellOccupation.NorthEastBottom | CellOccupation.MidSpace);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //
    //     outVal += scale;
    //
    //     #endregion
    //
    //     #region Inner Floor and Inner Mid
    //
    //     // Floor
    //     checkSpot = (CellOccupation.Bottom | CellOccupation.InnerSpace);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // West Floor Wall
    //     checkSpot = (CellOccupation.WestBottom | CellOccupation.InnerSpace);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // East Floor Wall
    //     checkSpot = (CellOccupation.EastBottom | CellOccupation.InnerSpace);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //
    //     outVal += scale;
    //
    //     // North Wall
    //     checkSpot = (CellOccupation.North | CellOccupation.InnerSpace);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // North West Wall
    //     checkSpot = (CellOccupation.NorthWest | CellOccupation.InnerSpace);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // North East Wall
    //     checkSpot = (CellOccupation.NorthEast | CellOccupation.InnerSpace);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //
    //     outVal += scale;
    //
    //     // North Bottom Wall
    //     checkSpot = (CellOccupation.NorthBottom | CellOccupation.InnerSpace);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // North West Bottom Wall
    //     checkSpot = (CellOccupation.NorthWestBottom | CellOccupation.InnerSpace);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // North East Bottom Wall
    //     checkSpot = (CellOccupation.NorthEastBottom | CellOccupation.InnerSpace);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //
    //     outVal += scale;
    //
    //     #endregion
    //
    //
    //     #region East West and Center Walls
    //
    //     // West Wall
    //     checkSpot = (CellOccupation.East | CellOccupation.InnerSpace);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // East Wall
    //     checkSpot = (CellOccupation.West | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //
    //     outVal += scale;
    //
    //     #endregion
    //
    //
    //     #region South and Top Wall
    //
    //     // North Top Wall
    //     checkSpot = (CellOccupation.NorthTop | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // North Top West Wall
    //     checkSpot = (CellOccupation.NorthWestTop | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // North Top East Wall
    //     checkSpot = (CellOccupation.NorthEastTop | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //
    //     outVal += scale;
    //
    //     // South Bottom Wall
    //     checkSpot = (CellOccupation.SouthBottom | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // South Bottom West Wall
    //     checkSpot = (CellOccupation.SouthWestBottom | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // South Bottom East Wall
    //     checkSpot = (CellOccupation.SouthEastBottom | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //
    //     outVal += scale;
    //
    //     // South Wall
    //     checkSpot = (CellOccupation.South | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // South West Wall
    //     checkSpot = (CellOccupation.SouthWest | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // South East Wall
    //     checkSpot = (CellOccupation.SouthEast | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //
    //     outVal += scale;
    //
    //     // Top Wall
    //     checkSpot = (CellOccupation.Top | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // Top West Wall
    //     checkSpot = (CellOccupation.WestTop | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // Top East Wall
    //     checkSpot = (CellOccupation.EastTop | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //
    //     outVal += scale;
    //
    //     // South Top Wall
    //     checkSpot = (CellOccupation.SouthTop | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // South Top West Wall
    //     checkSpot = (CellOccupation.SouthWestTop | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // South Top East Wall
    //     checkSpot = (CellOccupation.SouthEastTop | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //
    //     outVal += scale;
    //
    //
    //     // North Bottom Wall
    //     checkSpot = (CellOccupation.NorthBottom | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // North West Bottom Wall
    //     checkSpot = (CellOccupation.NorthWestBottom | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //     // North East Bottom Wall
    //     checkSpot = (CellOccupation.NorthEastBottom | CellOccupation.Walls);
    //     if (occupation.Any(x => (x & checkSpot) == checkSpot)) return outVal;
    //
    //     outVal += scale;
    //
    //     #endregion
    //
    //
    //
    //     return 0f;
    //
    //
    // }
    //
    // public static int DumbShit(CellOccupation[] cellOccupations)
    // {
    //     // Sides Top > South > East/West > North > Bottom
    //     // Layers Walls > Mid > Inner
    //
    //     //TODO: this is dumb
    //
    //     int outVal = 0;
    //
    //
    //     // Top South Edges
    //     if (cellOccupations.Contains(CellOccupation.SouthTop | CellOccupation.Walls))
    //         return outVal; else outVal++;
    //     
    //     // Top South East/West Cornors
    //     if (cellOccupations.Contains(CellOccupation.SouthEastTop | CellOccupation.Walls) ||
    //         cellOccupations.Contains(CellOccupation.SouthWestTop | CellOccupation.Walls))
    //         return outVal; else outVal++;
    //
    //     // Top Face
    //     if (cellOccupations.Contains(CellOccupation.Top | CellOccupation.Walls))
    //         return outVal; else outVal++;
    //     
    //     // Top East/West Edges
    //     if (cellOccupations.Contains(CellOccupation.EastTop | CellOccupation.Walls) ||
    //         cellOccupations.Contains(CellOccupation.SouthWestTop | CellOccupation.Walls))
    //         return outVal; else outVal++;
    //
    //     // South Face
    //     if (cellOccupations.Contains(CellOccupation.South | CellOccupation.Walls))
    //         return outVal; else outVal++;
    //
    //
    //     // South East/West Edge
    //     if (cellOccupations.Contains(CellOccupation.SouthEast | CellOccupation.Walls) ||
    //         cellOccupations.Contains(CellOccupation.SouthWest | CellOccupation.Walls))
    //         return outVal; else outVal++;
    //
    //     // South East/West Bottom Corners
    //     if (cellOccupations.Contains(CellOccupation.SouthEastBottom | CellOccupation.Walls) ||
    //         cellOccupations.Contains(CellOccupation.SouthWestBottom | CellOccupation.Walls))
    //         return outVal; else outVal++;
    //     
    //     // South Bottom Edge
    //     if (cellOccupations.Contains(CellOccupation.South | CellOccupation.Walls))
    //         return outVal; else outVal++;
    //
    //     // East/West Faces
    //     if (cellOccupations.Contains(CellOccupation.East | CellOccupation.Walls) ||
    //         cellOccupations.Contains(CellOccupation.West| CellOccupation.Walls))
    //         return outVal; else outVal++;
    //
    //     // Top East/West North Corners
    //     if (cellOccupations.Contains(CellOccupation.NorthEastTop | CellOccupation.Walls) ||
    //         cellOccupations.Contains(CellOccupation.NorthWestTop | CellOccupation.Walls))
    //         return outVal; else outVal++;
    //     
    //     // Top North Edge
    //     if (cellOccupations.Contains(CellOccupation.NorthTop | CellOccupation.Walls))
    //         return outVal; else outVal++;
    //
    //     // Top East/West North Corners
    //     if (cellOccupations.Contains(CellOccupation.NorthEastTop | CellOccupation.Walls) ||
    //         cellOccupations.Contains(CellOccupation.NorthWestTop | CellOccupation.Walls))
    //         return outVal;
    //     else outVal++;
    //     throw new NotImplementedException();
    //
    // }
}