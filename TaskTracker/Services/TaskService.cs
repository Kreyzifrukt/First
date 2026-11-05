using TaskTracker.Models;
using TaskTracker.Storage;

namespace TaskTracker.Services;

public sealed class TaskService
{
    private readonly JsonTaskRepository _repository;
    private readonly JsonCategoryRepository _categoryRepository;
    private readonly List<TaskItem> _tasks;
    private readonly List<TaskCategory> _categories;

    private TaskService(JsonTaskRepository repository, JsonCategoryRepository categoryRepository,
        List<TaskItem> tasks, List<TaskCategory> categories)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
        _tasks = tasks;
        _categories = categories;
    }

    public static async Task<TaskService> CreateAsync(JsonTaskRepository repository,
        JsonCategoryRepository? categoryRepository = null)
    {
        categoryRepository ??= new JsonCategoryRepository();
        var tasks = await repository.LoadAsync();
        var categories = await categoryRepository.LoadAsync();
        return new TaskService(repository, categoryRepository, tasks, categories);
    }

    public IReadOnlyList<TaskItem> GetAll() => _tasks.AsReadOnly();
    public IReadOnlyList<TaskCategory> GetCategories() => _categories.AsReadOnly();

    public IReadOnlyList<TaskItem> GetByCategory(Guid categoryId)
    {
        EnsureCategoryExists(categoryId);
        return _tasks.Where(task => task.CategoryId == categoryId).ToList().AsReadOnly();
    }

    public async Task<TaskItem> AddAsync(string title, string description)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Task title cannot be empty.", nameof(title));

        var task = new TaskItem { Title = title.Trim(), Description = description?.Trim() ?? string.Empty };
        _tasks.Add(task);
        try { await _repository.SaveAsync(_tasks); }
        catch { _tasks.Remove(task); throw; }
        return task;
    }

    public async Task<bool> CompleteAsync(Guid id)
    {
        var task = _tasks.FirstOrDefault(item => item.Id == id);
        if (task is null) return false;
        if (!task.IsCompleted)
        {
            task.IsCompleted = true;
            task.CompletedAt = DateTime.UtcNow;
            try { await _repository.SaveAsync(_tasks); }
            catch { task.IsCompleted = false; task.CompletedAt = null; throw; }
        }
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var task = _tasks.FirstOrDefault(item => item.Id == id);
        if (task is null) return false;
        _tasks.Remove(task);
        try { await _repository.SaveAsync(_tasks); }
        catch { _tasks.Add(task); throw; }
        return true;
    }

    public async Task<TaskCategory> AddCategoryAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty.", nameof(name));

        var normalized = name.Trim();
        if (_categories.Any(category => string.Equals(category.Name, normalized,
            StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("A category with this name already exists.");

        var category = new TaskCategory { Name = normalized };
        _categories.Add(category);
        try { await _categoryRepository.SaveAsync(_categories); }
        catch { _categories.Remove(category); throw; }
        return category;
    }

    public async Task<bool> DeleteCategoryAsync(Guid categoryId)
    {
        var category = _categories.FirstOrDefault(item => item.Id == categoryId);
        if (category is null) return false;
        if (_tasks.Any(task => task.CategoryId == categoryId))
            throw new InvalidOperationException("Category is assigned to tasks. Unassign or reassign those tasks first.");

        _categories.Remove(category);
        try { await _categoryRepository.SaveAsync(_categories); }
        catch { _categories.Add(category); throw; }
        return true;
    }

    public async Task<bool> AssignCategoryAsync(Guid taskId, Guid? categoryId)
    {
        var task = _tasks.FirstOrDefault(item => item.Id == taskId);
        if (task is null) return false;
        if (categoryId.HasValue) EnsureCategoryExists(categoryId.Value);

        var previous = task.CategoryId;
        task.CategoryId = categoryId;
        try { await _repository.SaveAsync(_tasks); }
        catch { task.CategoryId = previous; throw; }
        return true;
    }

    private void EnsureCategoryExists(Guid categoryId)
    {
        if (_categories.All(category => category.Id != categoryId))
            throw new ArgumentException("Category does not exist.", nameof(categoryId));
    }
}
