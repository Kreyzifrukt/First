# TaskTracker

A simple console-based task manager built with C# and .NET 10.

## Features

- Add tasks with a title and optional description
- View all tasks and their completion status
- Mark tasks as completed
- Delete tasks
- Persist tasks locally in a JSON file

## Requirements

- .NET 10 SDK

## Run

From the repository root:

```bash
cd TaskTracker
dotnet run
```

Tasks are stored in `tasks.json` in the application's working directory.

## Project structure

```text
TaskTracker/
├── Models/
│   └── TaskItem.cs
├── Services/
│   └── TaskService.cs
├── Storage/
│   └── JsonTaskRepository.cs
├── Program.cs
└── TaskTracker.csproj
```
