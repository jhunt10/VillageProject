namespace VillageProject.Core.DIM.Defs;

public interface ICompDef
{
    string CompDefClassName { get; }
    
    string CompInstClassName { get; }
    
    string ManagerClassName { get; }
    
    bool RegisterOnLoad { get; }
}