using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Map.MapSpaces;

namespace VillageProject.Godot.DefDefs.DefPrefabs;

public static class MapDefs
{
    public static IDef SingleCellMapSpace = new ObjectDef
    {
        DefName = "Defs.MapSpaces.Testing.SingleCell",
        Label = "TinyTest",
        CompDefs = new System.Collections.Generic.List<ICompDef>
        {
            new MapSpaceCompDef
            {
                MaxX = 0,
                MinX = 0,
                MaxY = 0,
                MinY = 0,
                MaxZ = 0,
                MinZ = 0
            }
        }
    };
    
    public static IDef TinyTestMapSpace = new ObjectDef
    {
        DefName = "Defs.MapSpaces.Testing.TinyTest",
        Label = "TinyTest",
        CompDefs = new System.Collections.Generic.List<ICompDef>
        {
            new MapSpaceCompDef
            {
                MaxX = 3,
                MinX = -3,
                MaxY = 3,
                MinY = -3,
                MaxZ = 1,
                MinZ = -1
            }
        }
    };
    
    public static IDef LargerTestMapSpace = new ObjectDef
    {
        DefName = "Defs.MapSpaces.Testing.LargerTest",
        Label = "LargerTest",
        CompDefs = new System.Collections.Generic.List<ICompDef>
        {
            new MapSpaceCompDef
            {
                
                MaxX = 8,
                MinX = -8,
                MaxY = 8,
                MinY = -8,
                MaxZ = 1,
                MinZ = -1
            }
        }
    };
    
    public static Dictionary<string, IDef> Defs = new Dictionary<string, IDef>
    {
        {"SingleCell", SingleCellMapSpace},
        { "TinyTest", TinyTestMapSpace },
        { "LargerTest", LargerTestMapSpace}
    };
}