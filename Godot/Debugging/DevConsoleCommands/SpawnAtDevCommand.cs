using VillageProject.Core.Behavior;
using VillageProject.Core.DIM;
using VillageProject.Core.Enums;
using VillageProject.Core.Items;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapStructures;

namespace VillageProject.Godot.Debugging.DevConsoleCommands;

public class SpawnAtDevCommand : IDevConsoleCommand
{
    public string Tag => "spawnat";
    public Result RunCommand(string commandText)
    {
        try
        {
            var tokens = commandText.Split(" ");
            if (tokens.Length != 5)
                return new Result(false, "Invalid Args. Expected: spawnat DEF_NAME X Y Z");

            var def = DimMaster.GetDefByPartialName(tokens[1], false);
            if (def == null)
                return new Result(false, $"Failed to find def with name '{tokens[1]}'");

            var newInst = DimMaster.InstantiateDef(def);

            var mapController = GameMaster.MapControllerNode.GetMainMapNode();

            var x = int.Parse(tokens[2]);
            var y = int.Parse(tokens[3]);
            var z = int.Parse(tokens[4]);

            var mapspot = new MapSpot(x, y, z);

            var mapComp = newInst.GetComponentOfType<IMapPositionComp>(activeOnly:false);
            if (mapComp != null)
            {
                mapComp.TrySetMapPosition(new MapPositionData(mapController.MapSpace,
                        mapspot, RotationFlag.South));

                return new Result(true, $"Spawned '{tokens[1]}' at {mapspot}.");
            }

            var itemComp = newInst.GetComponentOfType<ItemCompInst>(activeOnly: false);
            if (itemComp != null)
            {
                ItemHelper.DropItemOnMap(itemComp, mapspot, mapController.MapSpace);
                return new Result(true, $"Spawned Item '{tokens[1]}' at {mapspot}.");
            }
            
            return new Result(true, $"Spawned '{tokens[1]}' No POS.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new Result(true, e.Message);
        }
        
    }
}