using System.Text.Json;
using TaskTracker.Models;

namespace TaskTracker.Storage;

public sealed class JsonCategoryRepository
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    public JsonCategoryRepository(string filePath = "categories.json")
    {
        _filePath = Path.GetFullPath(filePath);
    }

    public async Task<List<TaskCategory>> LoadAsync()
    {
        if (!File.Exists(_filePath))
            return [];

        await using var stream = File.OpenRead(_filePath);
        return await JsonSerializer.DeserializeAsync<List<TaskCategory>>(stream, _options) ?? [];
    }

    public async Task SaveAsync(IEnumerable<TaskCategory> categories)
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        var temporaryPath = _filePath + ".tmp";
        await using (var stream = new FileStream(
            temporaryPath, FileMode.Create, FileAccess.Write, FileShare.None,
            bufferSize: 4096, FileOptions.Asynchronous | FileOptions.WriteThrough))
        {
            await JsonSerializer.SerializeAsync(stream, categories, _options);
            await stream.FlushAsync();
        }

        File.Move(temporaryPath, _filePath, overwrite: true);
    }
}
