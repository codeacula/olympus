# Task 4: Create Domain Layer Core

## Task Info

### Summary

Implement the empty Domain project with prescribed folders.

### Estimated size

None

### Context / background

Domain is the heart of the system—aggregates, events, value objects, enums, services, repositories, and errors belong here.

### Implementation plan

* In `Olympus.Domain`, create folders: `Aggregates`, `Events`, `ValueObjects`, `Enums`, `Services`, `Repositories`, `Errors`.
* Add marker interface `IDomainEvent` in `Events`.
* Create base classes `AggregateRoot` and `Entity` under `Aggregates/Common`.
* Stub out `DomainError` under `Errors`.
* Define empty interfaces under `Repositories` (e.g., `ICharacterRepository`).

### Acceptance criteria

* `Olympus.Domain` compiles.
* All folders exist and contain at least one placeholder file so they're tracked in Git.

## Execution Plan
