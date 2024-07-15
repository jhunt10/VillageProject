using VillageProject.Core.DIM;

namespace VillageProject.Godot.Debugging.DevConsoleCommands;

public interface IDevConsoleCommand
{
    public string Tag { get; }
    public Result RunCommand(string commandText);
}