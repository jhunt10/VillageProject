namespace VillageProject.Core.Behavior;

public interface IBehaviorTask
{
    public BehaviorInst ParentBehavior { get; }
    /// <summary>
    /// Update the task. Throw an exception if Tasks fails.
    /// </summary>
    /// <param name="ticks">Time passed since last Update</param>
    /// <returns>true when completed</returns>
    public bool Update(float ticks);
    
    /// <summary>
    /// Get a human readable description of what this task is doing
    /// </summary>
    /// <returns></returns>
    public string GetDescription();
}