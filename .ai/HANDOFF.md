# Handoff: Claude -> ChatGPT (technical lead / reviewer)

Status: informational note, not a task. It does not replace GitHub Issues (see `../PROTOCOL.md`).
Written against commit `aeaba67` on `main`.

## What was actually checked

- Read every source file in `TaskTracker/` (`Program.cs`, `Models/TaskItem.cs`, `Services/TaskService.cs`, `Storage/JsonTaskRepository.cs`, `TaskTracker.csproj`), the CI workflow, both templates, and all collaboration docs.
- Not checked: the project was **not built or run** (no .NET SDK in the environment used to write this note), no tests exist, and GitHub Issues/labels/branch protection could not be inspected (the available GitHub CLI token was invalid). Treat every statement below as a static reading of the code, not as a tested result.

## Project facts

- Console task manager, .NET 10, nullable enabled, no external dependencies.
- Layers: `Program.cs` (menu loop, top-level statements) -> `TaskService` (in-memory list, saves after every change) -> `JsonTaskRepository` (`tasks.json` in the working directory, git-ignored).
- `TaskService` depends on the concrete `JsonTaskRepository`; there is no repository interface.
- CI builds in Release only. There is no test project and no test step.
- `TASKS.md` lists no tasks; no Issue has been registered by this note.

## Observations for review (from static reading)

Facts are marked as such; severity is a suggestion, not a verdict.

1. **Non-atomic save** (`JsonTaskRepository.SaveAsync`). `File.Create` truncates `tasks.json` before writing. A crash or I/O error mid-write can leave an empty or partial file and lose all tasks. Data-safety risk, likely the most important item.
2. **Corrupt file crashes startup** (`LoadAsync`). `JsonSerializer.DeserializeAsync` throws `JsonException` on invalid JSON and nothing catches it in `Program.cs`. Combined with item 1, one bad write can make the app unable to start.
3. **In-memory state diverges from disk on save failure** (`TaskService.AddAsync`/`DeleteAsync`/`CompleteAsync`). The list is changed first, then saved. If `SaveAsync` throws, memory and file disagree.
4. **Hard to unit test.** Concrete repository dependency plus all UI logic in top-level statements. An `ITaskRepository` interface would be the minimum seam.
5. **Usability.** Complete/Delete require typing a full GUID. Selecting by list number would be simpler. Product decision for the owner, not a defect.
6. **Interactive-only console calls.** `Console.ReadKey()` (and possibly `Console.Clear()`) can throw when input/output is redirected. Low priority for a desktop console app.
7. **Minor.** `GetAll()` returns the internal list typed as `IReadOnlyList<T>` (a caller can cast it back); no length limits on title/description; two running instances would overwrite each other's `tasks.json`.

## Suggested task breakdown (drafts only; none are approved or registered)

Each would need an Issue from the AI Task template and human scope approval before any work starts.

1. Introduce `ITaskRepository` and add an xUnit test project covering `TaskService` (add/complete/delete, not-found paths). Add a `dotnet test` step to CI.
2. Make `SaveAsync` safe: write to a temp file, then replace. Handle invalid/corrupt JSON on load with a clear message instead of a crash.
3. Make `TaskService` consistent when saving fails (roll back the in-memory change or save before committing it).
4. Optional, owner's call: select tasks by list number instead of GUID.

Items 2 and 3 change data-handling behavior, so per `REVIEW.md` they warrant explicit review of data safety.

## Reminders from the protocol

- Claude implements only on `ai/<issue-number>-<short-name>` branches and never merges; only the human owner authorizes merges.
- This note was added on branch `ai/handoff-note`, which does not follow the issue-number convention because no Issue exists yet. Rename or recreate the branch if an Issue is opened for it.
- AI approval is advisory only.
