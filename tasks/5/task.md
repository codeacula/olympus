# Task 5: Define Minimal Domain Value Objects for MVP

## Task Summary

- **Issue:** codeacula/olympus#5
- **Title:** Define minimal Domain value objects for MVP
- **Description:**
  - Introduce `SessionId` and `UserId` value objects (VOs) as `readonly record struct` for strong typing in Command handlers, avoiding plain strings for IDs.
  - Optionally implement parsing/factory methods (e.g., via `IParsable<T>`).
  - Write unit tests to validate equality and formatting.
- **Acceptance Criteria:**
  - Code compiles successfully.
  - Tests confirm VOs with the same GUID are equal and formatting round-trips correctly.

---

## Current Plan of Execution

1. **Create Strongly Typed ID Value Objects:**
   - `SessionId` (readonly record struct, wraps Guid)
   - `UserId` (readonly record struct, wraps Guid)
   - Both will implement parsing/factory methods and equality.
2. **Additional Value Objects to Create Now (per plan.md):**
   - `EntityId` (for ECS and general entity identification)
   - `GameDateTime` (for time tracking in the domain)
   - `PositionVO` (for logical position in the game world)
3. **Placement:**
   - All value objects will be placed in `src/Olympus.Domain/SharedKernel/ValueObjects/StronglyTypedIDs/` or appropriate subfolders as per plan.md.
4. **Testing:**
   - Unit tests for each value object will be added in `tests/Olympus.Tests.Domain/SharedKernel/ValueObjects/`.
5. **Rationale:**
   - These value objects are foundational, cross-cutting, and align with the MVP and Clean Architecture principles outlined in plan.md.

---

*This plan ensures the domain layer is ready for strong typing and future extensibility, following Olympus architectural standards.*

---

## Changelog

- Initial task and plan written, including SessionId, UserId, EntityId, GameDateTime, and PositionVO value objects. (Copilot, 2024-06-13)
