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

.ai/
├── CLAUDE.md
└── CHATGPT.md

.github/
├── ISSUE_TEMPLATE/
│   └── ai-task.md
├── workflows/
│   └── dotnet-ci.yml
└── PULL_REQUEST_TEMPLATE.md

PROTOCOL.md
ARCHITECTURE.md
TASKS.md
REVIEW.md
```

## Continuous integration

GitHub Actions runs a .NET 10 Release build for pull requests targeting `main` and pushes to `main`. The workflow restores dependencies and builds the application. It does not run automated tests yet because the repository currently has no test project.

## AI-assisted development

This repository includes a human-supervised collaboration workflow for Claude (implementation) and ChatGPT (technical lead/reviewer).

- Read [PROTOCOL.md](PROTOCOL.md) before starting AI-assisted work.
- See [ARCHITECTURE.md](ARCHITECTURE.md) for roles, integration boundaries, and recommended GitHub protections.
- Use the [AI Task issue template](.github/ISSUE_TEMPLATE/ai-task.md) to define scoped work.
- Use [REVIEW.md](REVIEW.md) as the review checklist.
- Agent-specific instructions are in `.ai/CLAUDE.md` and `.ai/CHATGPT.md`.

All changes go through a branch and pull request. AI review is advisory; only the human owner decides whether to merge. This repository setup does not create background agents or grant Claude GitHub write access by itself. Claude needs a separately authenticated write-capable runtime, such as Claude Code, to push branches and open pull requests.
