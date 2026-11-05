# AI Collaboration Architecture

## Overview

GitHub is the shared coordination layer between ChatGPT and Claude. It stores task requests, implementation proposals, code changes, review feedback, and decisions in one auditable place.

```text
Human owner
   | creates task / approves scope / final merge decision
   v
GitHub Issue (task queue + acceptance criteria)
   |
   v
Claude (implementation) ----> feature branch ----> Pull Request
                                                    |
                                                    v
                                      ChatGPT (technical review)
                                                    |
                                  Changes requested / Approved
                                                    |
                                                    v
                                          Human final review
                                                    |
                                                    v
                                              Merge to main
```

## Repository layout

- `TaskTracker/`: application source code.
- `.ai/CLAUDE.md`: operating instructions for Claude.
- `.ai/CHATGPT.md`: review and technical-lead instructions for ChatGPT.
- `PROTOCOL.md`: shared workflow, status transitions, and safety rules.
- `ARCHITECTURE.md`: this architecture.
- `TASKS.md`: active queue and task index.
- `REVIEW.md`: review checklist and review log conventions.
- `.github/ISSUE_TEMPLATE/ai-task.md`: structured task intake.
- `.github/PULL_REQUEST_TEMPLATE.md`: implementation handoff format.

## Integration boundaries

GitHub Issues and PRs are the source of truth. An agent must have its own authenticated GitHub integration to read or write there. A ChatGPT GitHub connection does not grant Claude access, and a Claude connection does not grant ChatGPT access.

For Claude to implement changes, use Claude Code with the repository checked out and authenticated GitHub access, or another explicitly configured Claude runtime. A read-only Claude connector is not sufficient for pushing branches or opening PRs. Do not put access tokens in repository files, prompts, Issues, or chat messages.

This setup provides a human-supervised workflow, not a continuously running autonomous process. Agents act when invoked by the user or by separately configured automation. No background polling or agent-to-agent messaging is enabled by these documentation files alone.

## Permissions and branch protection

Recommended GitHub settings:
- Protect `main`; require pull requests before merging.
- Require CI checks to pass before merge.
- Require at least one human approval where the GitHub plan/settings support it.
- Disable force pushes and branch deletion on `main`.
- Give agents the minimum permissions needed to work on branches and PRs. Do not grant administration or bypass permissions.
- Keep merge permission with the human owner.

These settings must be enabled in GitHub repository settings; documentation cannot enforce them.
