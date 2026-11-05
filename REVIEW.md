# Review Standard

ChatGPT and the human owner use this checklist when reviewing a pull request.

## Required checks

- [ ] The implementation satisfies every acceptance criterion.
- [ ] The change is limited to the requested scope.
- [ ] Correctness: edge cases, null/invalid input, error paths, and state transitions are handled.
- [ ] Security: no secrets, unsafe deserialization, injection risks, or unnecessary permissions.
- [ ] Data safety: no accidental deletion, corruption, or irreversible migration.
- [ ] Maintainability: names, structure, abstractions, and comments are justified.
- [ ] Performance: no obvious unnecessary work or resource leaks.
- [ ] Tests: relevant tests were added or updated and actually run.
- [ ] Build/lint/CI results are reported accurately.
- [ ] Documentation and configuration are updated where needed.
- [ ] Breaking changes and residual risks are clearly described.

## Review outcome

Use one of these explicit outcomes:
- **Changes requested**: list each blocking issue, its location, why it matters, and the expected correction.
- **Approved**: state what was checked and list any non-blocking risks.

Approval from an AI reviewer is advisory. It does not authorize merging. The human owner makes the final decision.
