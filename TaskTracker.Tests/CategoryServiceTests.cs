using Xunit;
using TaskTracker.Services;
using TaskTracker.Storage;

namespace TaskTracker.Tests;

public sealed class CategoryServiceTests : IAsyncLifetime
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "TaskTracker.Tests", Guid.NewGuid().ToString());

    public Task InitializeAsync()
    {
        Directory.CreateDirectory(_directory);
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        if (Directory.Exists(_directory))
            Directory.Delete(_directory, recursive: true);
        return Task.CompletedTask;
    }

    private TaskService CreateService() =>
        TaskService.CreateAsync(
            new JsonTaskRepository(Path.Combine(_directory, "tasks.json")),
            new JsonCategoryRepository(Path.Combine(_directory, "categories.json"))).GetAwaiter().GetResult();

    [Fact]
    public async Task AddCategory_TrimsName_AndPersistsIt()
    {
        var service = CreateService();

        var category = await service.AddCategoryAsync("  Study  ");
        var reloaded = await TaskService.CreateAsync(
            new JsonTaskRepository(Path.Combine(_directory, "tasks.json")),
            new JsonCategoryRepository(Path.Combine(_directory, "categories.json")));

        Assert.Equal("Study", category.Name);
        Assert.Contains(reloaded.GetCategories(), item => item.Id == category.Id && item.Name == "Study");
    }

    [Fact]
    public async Task AddCategory_RejectsDuplicateNamesIgnoringCase()
    {
        var service = CreateService();
        await service.AddCategoryAsync("Study");

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddCategoryAsync(" study "));
    }

    [Fact]
    public async Task AssignCategory_FiltersTasksAndPersistsAssignment()
    {
        var service = CreateService();
        var category = await service.AddCategoryAsync("Study");
        var task = await service.AddAsync("Read chapter", "");
        var otherTask = await service.AddAsync("Wash dishes", "");

        Assert.True(await service.AssignCategoryAsync(task.Id, category.Id));
        Assert.Single(service.GetByCategory(category.Id));
        Assert.Equal(task.Id, service.GetByCategory(category.Id)[0].Id);
        Assert.DoesNotContain(service.GetByCategory(category.Id), item => item.Id == otherTask.Id);

        var reloaded = await TaskService.CreateAsync(
            new JsonTaskRepository(Path.Combine(_directory, "tasks.json")),
            new JsonCategoryRepository(Path.Combine(_directory, "categories.json")));
        Assert.Equal(category.Id, Assert.Single(reloaded.GetByCategory(category.Id)).CategoryId);
    }

    [Fact]
    public async Task DeleteCategory_RejectsCategoryThatIsStillAssigned()
    {
        var service = CreateService();
        var category = await service.AddCategoryAsync("Study");
        var task = await service.AddAsync("Read chapter", "");
        await service.AssignCategoryAsync(task.Id, category.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteCategoryAsync(category.Id));
        Assert.Contains(service.GetCategories(), item => item.Id == category.Id);
    }

    [Fact]
    public async Task DeleteCategory_SucceedsAfterTaskIsUnassigned()
    {
        var service = CreateService();
        var category = await service.AddCategoryAsync("Study");
        var task = await service.AddAsync("Read chapter", "");
        await service.AssignCategoryAsync(task.Id, category.Id);
        await service.AssignCategoryAsync(task.Id, null);

        Assert.True(await service.DeleteCategoryAsync(category.Id));
        Assert.DoesNotContain(service.GetCategories(), item => item.Id == category.Id);
    }

    [Fact]
    public async Task AddTask_RejectsBlankTitle()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() => service.AddAsync("  ", ""));
    }
}
