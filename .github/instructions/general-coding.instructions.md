---
applyTo: "**/*"
---

## 1. Principles
- **DRY**: extract shared logic to services or extension methods.
- **KISS**: avoid over‑engineering; start with the simplest working form.
- **YAGNI**: implement only what is required for current stories.
- **SOLID**: single responsibility, dependency inversion, etc.
- **DDD & CQRS/ES**: aggregates guard invariants, commands mutate, queries read.
- **Data‑Oriented Programming**: favor immutable records and focused data shapes.
- **Railway‑Oriented Error Handling**: propagate `Result<TSuccess,TError>`.

## 2. Patterns
- Factory/Builder for complex aggregates.
- Repository per aggregate root.
- Specification pattern for queries that may move to IL‑translatable form.

## 3. Performance
- Prefer async IO (`ValueTask<T>`).
- Cache hot read models in Redis.

## 4. Documentation
- XML docs for public APIs.
- Update ADRs for structural decisions.
