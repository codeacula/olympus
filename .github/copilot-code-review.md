# Code Review Guidelines

## 1. Preparation

1. Fetch the diff and run dotnet format + static analyzers.
1. Build and run unit tests.

## 2. Review Each Hunk

- **Explain the reason** for any suggestion first.
- Show a minimal **before / after** code snippet.
- Cross‑check with [copilot-instructions.md](copilot-instructions.md) rules:
  - Layer boundary respected?
  - CQRS handler names match `<Verb><Entity>`?
  - Logging via source‑generated helpers?
  - Tests cover new logic?

## 3. Output Template

```markdown

### Suggestion 1

**Why**: Explain.

  ```diff
  - old
  + new
  ```

### Suggestion 2

...

```

## 4. Finishing
- Ensure total filesize diff ≤500 lines unless justified.
- Re‑run tests and linters after changes.
