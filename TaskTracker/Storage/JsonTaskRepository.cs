using System.Text.Json;
using TaskTracker.Models;

namespace TaskTracker.Storage;

public sealed class JsonTaskRepository
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    public JsonTaskRepository(string filePath = "tasks.json")
    {
        _filePath = Path.GetFullPath(filePath);
    }

    public async Task<List<TaskItem>> LoadAsync()
    {
        if (!File.Exists(_filePath))
            return [];

        await using var stream = File.OpenRead(_filePath);
        return await JsonSerializer.DeserializeAsync<List<TaskItem>>(stream, _options) ?? [];
    }

    public async Task SaveAsync(IEnumerable<TaskItem> tasks)
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, tasks, _options);
    }
}
