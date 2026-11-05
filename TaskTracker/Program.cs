using TaskTracker.Models;
using TaskTracker.Services;
using TaskTracker.Storage;

var service = await TaskService.CreateAsync(new JsonTaskRepository(), new JsonCategoryRepository());

while (true)
{
    Console.Clear();
    Console.WriteLine("=== TaskTracker ===");
    Console.WriteLine("1. Show tasks");
    Console.WriteLine("2. Add task");
    Console.WriteLine("3. Complete task");
    Console.WriteLine("4. Delete task");
    Console.WriteLine("5. Show categories");
    Console.WriteLine("6. Add category");
    Console.WriteLine("7. Delete category");
    Console.WriteLine("8. Assign/remove task category");
    Console.WriteLine("9. Show tasks by category");
    Console.WriteLine("0. Exit");
    Console.Write("\nSelect an option: ");

    try
    {
        switch (Console.ReadLine()?.Trim())
        {
            case "1": ShowTasks(service.GetAll(), service.GetCategories()); break;
            case "2": await AddTaskAsync(service); break;
            case "3": await CompleteTaskAsync(service); break;
            case "4": await DeleteTaskAsync(service); break;
            case "5": ShowCategories(service.GetCategories()); break;
            case "6": await AddCategoryAsync(service); break;
            case "7": await DeleteCategoryAsync(service); break;
            case "8": await AssignCategoryAsync(service); break;
            case "9": ShowTasks(service.GetByCategory(ReadGuid("Category ID:")), service.GetCategories()); break;
            case "0": return;
            default: Console.WriteLine("Unknown option."); break;
        }
    }
    catch (Exception exception) when (exception is ArgumentException or InvalidOperationException or IOException)
    {
        Console.WriteLine($"Operation failed: {exception.Message}");
    }

    Console.WriteLine("\nPress any key to continue...");
    Console.ReadKey();
}

static Guid ReadGuid(string prompt)
{
    Console.Write(prompt);
    if (!Guid.TryParse(Console.ReadLine(), out var id))
        throw new ArgumentException("Invalid ID.");
    return id;
}

static void ShowTasks(IReadOnlyList<TaskItem> tasks, IReadOnlyList<TaskCategory> categories)
{
    Console.WriteLine("\n--- Tasks ---");
    if (tasks.Count == 0) { Console.WriteLine("No tasks yet."); return; }

    foreach (var task in tasks.OrderBy(task => task.IsCompleted).ThenBy(task => task.CreatedAt))
    {
        var status = task.IsCompleted ? "[Done]" : "[Open]";
        var category = categories.FirstOrDefault(item => item.Id == task.CategoryId)?.Name ?? "Uncategorized";
        Console.WriteLine($"{status} {task.Id}");
        Console.WriteLine($"  {task.Title}");
        Console.WriteLine($"  Category: {category}");
        if (!string.IsNullOrWhiteSpace(task.Description)) Console.WriteLine($"  {task.Description}");
    }
}

static void ShowCategories(IReadOnlyList<TaskCategory> categories)
{
    Console.WriteLine("\n--- Categories ---");
    if (categories.Count == 0) { Console.WriteLine("No categories yet."); return; }
    foreach (var category in categories.OrderBy(item => item.Name))
        Console.WriteLine($"{category.Id}  {category.Name}");
}

static async Task AddTaskAsync(TaskService service)
{
    Console.Write("Title: ");
    var title = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(title)) { Console.WriteLine("Title cannot be empty."); return; }
    Console.Write("Description (optional): ");
    var task = await service.AddAsync(title, Console.ReadLine() ?? string.Empty);
    Console.WriteLine($"Task added: {task.Id}");
}

static async Task CompleteTaskAsync(TaskService service)
{
    var id = ReadGuid("Task ID: ");
    Console.WriteLine(await service.CompleteAsync(id) ? "Task marked as completed." : "Task not found.");
}

static async Task DeleteTaskAsync(TaskService service)
{
    var id = ReadGuid("Task ID: ");
    Console.WriteLine(await service.DeleteAsync(id) ? "Task deleted." : "Task not found.");
}

static async Task AddCategoryAsync(TaskService service)
{
    Console.Write("Category name: ");
    var category = await service.AddCategoryAsync(Console.ReadLine() ?? string.Empty);
    Console.WriteLine($"Category created: {category.Id}");
}

static async Task DeleteCategoryAsync(TaskService service)
{
    ShowCategories(service.GetCategories());
    var id = ReadGuid("Category ID: ");
    Console.WriteLine(await service.DeleteCategoryAsync(id) ? "Category deleted." : "Category not found.");
}

static async Task AssignCategoryAsync(TaskService service)
{
    ShowTasks(service.GetAll(), service.GetCategories());
    ShowCategories(service.GetCategories());
    var taskId = ReadGuid("Task ID: ");
    Console.Write("Category ID (blank to remove category): ");
    var input = Console.ReadLine();
    Guid? categoryId = string.IsNullOrWhiteSpace(input)
        ? null
        : Guid.TryParse(input, out var parsed) ? parsed : throw new ArgumentException("Invalid category ID.");
    Console.WriteLine(await service.AssignCategoryAsync(taskId, categoryId)
        ? "Category assignment updated." : "Task not found.");
}
