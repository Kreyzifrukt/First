using TaskTracker.Models;
using TaskTracker.Storage;

namespace TaskTracker.Services;

public sealed class TaskService
{
    private readonly JsonTaskRepository _repository;
    private readonly List<TaskItem> _tasks;

    private TaskService(JsonTaskRepository repository, List<TaskItem> tasks)
    {
        _repository = repository;
        _tasks = tasks;
    }

    public static async Task<TaskService> CreateAsync(JsonTaskRepository repository)
    {
        var tasks = await repository.LoadAsync();
        return new TaskService(repository, tasks);
    }

    public IReadOnlyList<TaskItem> GetAll() => _tasks;

    public async Task<TaskItem> AddAsync(string title, string description)
    {
        var task = new TaskItem
        {
            Title = title.Trim(),
            Description = description.Trim()
        };

        _tasks.Add(task);
        await _repository.SaveAsync(_tasks);
        return task;
    }

    public async Task<bool> CompleteAsync(Guid id)
    {
        var task = _tasks.FirstOrDefault(item => item.Id == id);
        if (task is null)
            return false;

        if (!task.IsCompleted)
        {
            task.IsCompleted = true;
            task.CompletedAt = DateTime.UtcNow;
            await _repository.SaveAsync(_tasks);
        }

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var task = _tasks.FirstOrDefault(item => item.Id == id);
        if (task is null)
            return false;

        _tasks.Remove(task);
        await _repository.SaveAsync(_tasks);
        return true;
    }
}
