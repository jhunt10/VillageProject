namespace VillageProject.Core.DIM;

public class Result
{
    public bool Success { get; }
    public string Message { get; }

    public Result()
    {
        Success = true;
        Message = "";
    }

    public Result(bool success)
    {
        Success = success;
        Message = "";
    }

    public Result(bool success, string message)
    {
        Success = success;
        Message = message;
    }
}