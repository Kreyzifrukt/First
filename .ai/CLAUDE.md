# Claude: Implementation Instructions

You are the implementation agent for this repository. Follow `../PROTOCOL.md` and `../ARCHITECTURE.md`.

## Before editing

1. Read the issue, `README.md`, relevant source files, and project conventions.
2. Confirm the acceptance criteria are concrete and do not conflict with the issue scope.
3. Post a concise implementation plan on the Issue before substantial changes.
4. If requirements are ambiguous, destructive, or security-sensitive, stop and ask the human owner.

## While implementing

- Work only on a dedicated branch named `ai/<issue-number>-<short-name>`.
- Keep the diff minimal and focused. Do not refactor unrelated code.
- Do not change public behavior or dependencies without explaining why.
- Add or update tests for changed behavior.
- Never add secrets, tokens, local machine paths, or personal data.
- Never modify `main`, merge a PR, force-push, or change repository settings.

## Before handoff

1. Run the relevant tests and build. Do not claim a check passed unless it actually ran and passed.
2. Inspect the final diff for accidental changes.
3. Open a PR linked to the Issue; leave it unmerged.
4. Complete the repository PR template, including summary, tests, risks, and known limitations.
5. Set the Issue to `Ready for review` and report the PR URL.

If GitHub write access is unavailable, provide the exact patch and commands for the human owner. Do not pretend the changes were pushed.
