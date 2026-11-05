using TaskTracker.Models;
using TaskTracker.Services;
using TaskTracker.Storage;

var repository = new JsonTaskRepository();
var service = await TaskService.CreateAsync(repository);

while (true)
{
    Console.Clear();
    Console.WriteLine("=== TaskTracker ===");
    Console.WriteLine("1. Show tasks");
    Console.WriteLine("2. Add task");
    Console.WriteLine("3. Complete task");
    Console.WriteLine("4. Delete task");
    Console.WriteLine("0. Exit");
    Console.Write("\nSelect an option: ");

    switch (Console.ReadLine()?.Trim())
    {
        case "1":
            ShowTasks(service.GetAll());
            break;
        case "2":
            await AddTaskAsync(service);
            break;
        case "3":
            await CompleteTaskAsync(service);
            break;
        case "4":
            await DeleteTaskAsync(service);
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Unknown option.");
            break;
    }

    Console.WriteLine("\nPress any key to continue...");
    Console.ReadKey();
}

static void ShowTasks(IReadOnlyList<TaskItem> tasks)
{
    Console.WriteLine("\n--- Tasks ---");

    if (tasks.Count == 0)
    {
        Console.WriteLine("No tasks yet.");
        return;
    }

    foreach (var task in tasks.OrderBy(task => task.IsCompleted).ThenBy(task => task.CreatedAt))
    {
        var status = task.IsCompleted ? "[Done]" : "[Open]";
        Console.WriteLine($"{status} {task.Id}");
        Console.WriteLine($"  {task.Title}");

        if (!string.IsNullOrWhiteSpace(task.Description))
            Console.WriteLine($"  {task.Description}");
    }
}

static async Task AddTaskAsync(TaskService service)
{
    Console.Write("Title: ");
    var title = Console.ReadLine()?.Trim();

    if (string.IsNullOrWhiteSpace(title))
    {
        Console.WriteLine("Title cannot be empty.");
        return;
    }

    Console.Write("Description (optional): ");
    var description = Console.ReadLine() ?? string.Empty;

    var task = await service.AddAsync(title, description);
    Console.WriteLine($"Task added: {task.Id}");
}

static async Task CompleteTaskAsync(TaskService service)
{
    Console.Write("Task ID: ");
    if (!Guid.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("Invalid task ID.");
        return;
    }

    Console.WriteLine(await service.CompleteAsync(id)
        ? "Task marked as completed."
        : "Task not found.");
}

static async Task DeleteTaskAsync(TaskService service)
{
    Console.Write("Task ID: ");
    if (!Guid.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("Invalid task ID.");
        return;
    }

    Console.WriteLine(await service.DeleteAsync(id)
        ? "Task deleted."
        : "Task not found.");
}
