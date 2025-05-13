---
applyTo: "**/*.cs"
---

## C# Specific Coding Guidelines for Olympus (.NET 9+)

This project leverages modern C# features and targets .NET 9+. Adhere to the following
C#-specific guidelines:

**1. Language Version & Features:**

* Utilize features available in C# 12/13 (as supported by .NET 9+).
* **Records and Primary Constructors:** Use `record class` or `readonly record struct` for DTOs,
    events, value objects, and simple immutable types. Leverage primary constructors for conciseness.
    ```csharp
    // Example:
    public readonly record struct UserId(Guid Value);
    public record class CharacterCreatedEvent(CharacterId CharacterId, string Name, CharacterClass Class);
    ```
* **`required` Members:** Use `required` modifiers for properties that must be set during object
    initialization, especially in DTOs and command objects.
* **File-Local Types:** Use file-local types (`file class`, `file record`) for helper types
    that are only relevant within a single file to improve encapsulation.
* **Pattern Matching:** Extensively use pattern matching (e.g., in `switch` expressions,
    `is` expressions, property patterns) for clearer and more concise conditional logic.
* **Static Abstract Members in Interfaces:** Utilize for patterns like factories or parsable
    types (e.g., `IParseable<TSelf>`).
* **Collection Literals:** Use collection literals where appropriate for initializing collections.
* **Primary Constructors (for classes and structs):** Use where they simplify constructor logic.
* **Default Lambda Parameters:** Use for optional lambda parameters if it enhances clarity.
* **`using` Aliases for Any Type:** Leverage for improved readability with complex generic types.

**2. Nullability:**

* Enable nullable reference types (`<Nullable>enable</Nullable>`).
* Be explicit about nullability. Avoid `!` (null-forgiving operator) unless absolutely
    certain a value won't be null and the compiler cannot infer it.
* Prefer `Option<T>` or `Result<T, TError>` over returning `null` for operations that
    might not produce a value or might fail.

**3. Asynchronous Programming:**

* Use `async`/`await` for all I/O-bound operations.
* Return `ValueTask<T>` or `ValueTask` for methods that are expected to complete
    synchronously in the common case to avoid unnecessary allocations.
* Use `IAsyncEnumerable<T>` for asynchronous streaming of data.
* Properly use `CancellationToken` throughout async call chains.
* In library code (Application, Domain, Infrastructure layers), generally use
    `ConfigureAwait(false)` to avoid deadlocks, unless UI context synchronization is
    explicitly needed (rare in backend services).

**4. LINQ and Collections:**

* Use LINQ for querying collections where it enhances readability.
* Be mindful of deferred execution and potential multiple enumerations. Use `.ToList()`
    or `.ToArray()` when appropriate to materialize a query.
* Prefer immutable collections (`System.Collections.Immutable`) when immutability is desired
    and performance allows.
* Utilize `Span<T>` and `Memory<T>` for high-performance scenarios involving slices of arrays
    or memory, especially to avoid allocations.

**5. Error Handling & Exceptions:**

* For recoverable errors and expected alternative flows, prefer returning `Result<TSuccess, TError>`
    from application service methods and command handlers.
* Reserve exceptions for truly exceptional, unrecoverable situations or programmer errors
    (e.g., `ArgumentNullException`, `InvalidOperationException`).
* Define custom error types (often records) to be used with the `Result` type for clear,
    typed error information.

**6. Dependency Injection:**

* Follow constructor injection patterns.
* Services should generally be registered with appropriate lifetimes (singleton, scoped, transient)
    in the `DependencyInjection.cs` files of respective projects.

**7. Logging:**

* Use source-generated logging (`LoggerMessageAttribute`) for high-performance, structured logging.
* Log meaningful information at appropriate log levels (Information, Warning, Error, Debug).
* Include relevant context in log messages (e.g., IDs, operation names).

**8. Value Objects and Strongly-Typed IDs:**

* Extensively use `readonly record struct` for creating strongly-typed IDs (e.g., `CharacterId`,
    `CampaignId`) to enhance type safety.
* Implement value objects for domain concepts that are defined by their attributes rather than
    identity (e.g., `Money`, `PositionVO`). Ensure they implement value equality.

**9. Project Structure and Namespaces:**

* Follow the established project structure (e.g., `Olympus.Domain`, `Olympus.Application`,
    feature slices).
* Namespaces should mirror the folder structure.

**10. Code Style:**

* Adhere to the `.editorconfig` settings for consistent code style.
* Generally follow Microsoft's C# Coding Conventions.
* Prefer expression-bodied members for simple, single-line methods and properties.

When generating code, consider the architectural layer (Domain, Application, Infrastructure) and apply
patterns relevant to that layer. For example, Domain entities should encapsulate business logic,
while Application services orchestrate use cases.