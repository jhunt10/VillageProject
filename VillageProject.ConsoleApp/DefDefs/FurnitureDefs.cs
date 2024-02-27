using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapStructures;
using VillageProject.Core.Map.MapStructures.Constructables;

namespace VillageProject.ConsoleApp.DefDefs;

public class FurnitureDefs
{
    public static Dictionary<string, IDef> Defs = new Dictionary<string, IDef>
    {
        {
            "Bed",
            new Def
            {
                DefName = "Defs.MapStructures.Furniture.Bed",
                Label = "Bed",
                CompDefs = new List<ICompDef>
                {
                    new MapStructCompDef
                    {
                        MapLayer = MapStructureManager.DEFAULT_MAP_LAYER,
                        OccupationData = new OccupationData(new Dictionary<MapSpot, List<OccupationFlags>>
                        {
                            {
                                new MapSpot(0, 0, 0), new List<OccupationFlags>
                                {
                                    OccupationFlags.Center | OccupationFlags.Bottom | OccupationFlags.Front |
                                    OccupationFlags.BottomFront
                                }
                            },

                            {
                                new MapSpot(0, 1, 0), new List<OccupationFlags>
                                {
                                    OccupationFlags.Center | OccupationFlags.Bottom | OccupationFlags.Back |
                                    OccupationFlags.BottomBack
                                }
                            }
                        })

                    },
                    new ConstructableCompDef()
                    {
                        IconSprite = "BedIcon.png"
                    }
                }
            }

        }
    };
}