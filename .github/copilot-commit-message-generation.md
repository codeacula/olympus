# Commit Message Guidelines

## 1. Format

```text

<type>(<scope>): <subject> <BLANK>

<body>
<BLANK>
<footer>
```

- **type**: feat, fix, perf, refactor, docs, test, chore.
- **scope**: optional folder or bounded context (`Domain`, `ECS`, `Ai`, etc.).
- **subject**: imperative, ≤72 chars.

## 2. Body Checklist

- *What* changed and *why* (link to ADR or issue).
- Major modules touched (`git diff --name-only`).
- Migration guidance if any.

## 3. Footers

- Issue linkage: `Closes #123`.
- BREAKING CHANGE: write exactly this token then describe API impact.
- Co‑authors: `Co-Authored-By:` lines.

## 4. Examples

```text
feat(Application): add ProcessPlayerNarrativeInputCommand handler

Adds CQRS handler to route player text into SK orchestrator.
Handles context caching in Redis and publishes DomainEvent.

Closes #42
```

```text
BREAKING CHANGE: Result<T> now requires explicit Success/Failure states.
```
