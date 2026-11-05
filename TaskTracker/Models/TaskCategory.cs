namespace TaskTracker.Models;

public sealed class TaskCategory
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
}
