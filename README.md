# TaskTracker

A console-based task manager built with C# and .NET 10.

## Features

- Add tasks with a title and optional description
- View all tasks and their completion status
- Mark tasks as completed and delete tasks
- Create categories with unique names (case-insensitive)
- Assign or remove a category from a task
- Filter tasks by category
- Prevent deletion of categories that are still assigned to tasks
- Persist tasks and categories locally as JSON

## Requirements

- .NET 10 SDK

## Run

From the repository root:

```bash
dotnet run --project TaskTracker/TaskTracker.csproj
```

Tasks are stored in `tasks.json` and categories in `categories.json`, both in the application's working directory. Existing task JSON files remain compatible because category assignment is an optional property.

## Test

```bash
dotnet test TaskTracker.Tests/TaskTracker.Tests.csproj --configuration Release
```

## Project structure

```text
TaskTracker/
├── Models/
│   ├── TaskItem.cs
│   └── TaskCategory.cs
├── Services/
│   └── TaskService.cs
├── Storage/
│   ├── JsonTaskRepository.cs
│   └── JsonCategoryRepository.cs
├── Program.cs
└── TaskTracker.csproj

TaskTracker.Tests/
├── CategoryServiceTests.cs
└── TaskTracker.Tests.csproj
```

## Continuous integration

GitHub Actions restores the test project, runs the test suite, and builds the application using the .NET 10 SDK in Release configuration.

## AI-assisted development

This repository uses a human-supervised Claude and ChatGPT workflow. See [PROTOCOL.md](PROTOCOL.md), [ARCHITECTURE.md](ARCHITECTURE.md), and [REVIEW.md](REVIEW.md). All changes remain in feature branches and pull requests until the human owner decides whether to merge.
