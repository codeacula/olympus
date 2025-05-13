---
applyTo: "**/*" # Applies broadly when code generation tasks are invoked
---

## Olympus Code Generation Guidelines for Copilot

When you are generating code for the Olympus project, please adhere to the following principles:

**1. Understand the Context:**

* Pay close attention to the current file, open files, selected code, and any specific
    instructions or comments provided in the prompt.
* Refer to `#file:./path/to/relevant/file.cs` or `#symbol:Namespace.TypeName.MethodName`
    if specific context from other parts of the codebase is crucial.
* Consider the architectural layer (`Domain`, `Application`, `Infrastructure`) implied by
    the current file or prompt.

**2. Follow Project Conventions and Patterns:**

* **Architecture:** Strictly adhere to Clean Architecture principles. Ensure code generated
    belongs in the correct layer and respects dependency rules.
* **C# Standards:** Follow the guidelines in `.github/instructions/csharp.instructions.md`.
    Use modern C# features (.NET 9+) appropriately.
* **Error Handling:** Implement error handling using the `Result<TSuccess, TError>` pattern
    for operations that can fail. Define clear error types.
* **Immutability:** Favor immutable types (records, readonly structs) for DTOs, events,
    and value objects.
* **Async:** Use `async/await` correctly for I/O-bound operations. Use `ValueTask<T>`
    and `IAsyncEnumerable<T>` where appropriate.
* **Testing:** If generating business logic or complex components, also suggest or be
    prepared to generate corresponding xUnit tests following the AAA pattern.
* **Existing Code:** Mimic the style and patterns found in existing, related code within
    the Olympus project.

**3. Quality of Generated Code:**

* **Clarity and Readability:** Generate code that is easy to understand and maintain.
    Use meaningful names and clear logic.
* **Correctness:** Strive for functionally correct code. While the developer is responsible
    for final validation, aim for high accuracy.
* **Performance:** Be mindful of performance, especially in loops, data processing, and
    critical paths. Avoid unnecessary allocations.
* **Security:** Generate code that is secure by default. Be aware of common vulnerabilities
    (e.g., SQL injection if generating database interaction code, though Marten helps
    mitigate this). Sanitize inputs if dealing with external data directly.
* **Conciseness:** Generate code that is concise but not at the expense of readability.
    Avoid overly verbose or boilerplate code where modern C# features can simplify it.

**4. Iterative Generation and Self-Correction:**

* **Multiple Suggestions:** If unsure, provide a few distinct, high-quality suggestions
    if the hosting environment supports it.
* **Self-Review (Conceptual):** Before finalizing a suggestion, conceptually review it
    against these guidelines:
  * "Does this code fit the Clean Architecture layer it's intended for?"
  * "Does it use modern C# features appropriately for Olympus?"
  * "Is error handling robust (e.g., using `Result<T, E>`)?"
  * "Is it testable?"
  * "Are there any obvious performance or security concerns?"
* **Alternative Approaches:** If a prompt is complex or could have multiple valid
    implementations, you might briefly mention an alternative approach or ask for
    clarification if the prompt is ambiguous.

**5. Specific Scenarios:**

* **New Classes/Methods:** Ensure they have appropriate access modifiers, clear names,
    and XML documentation comments for public APIs.
* **Implementing Interfaces:** Fully implement all members of the interface.
* **CQRS Handlers (MediatR):**
  * Commands should be records.
  * Handlers should receive dependencies via constructor injection.
  * Handlers should return `Task<Result<TSuccess, TError>>` or `Task<Result<Success, TError>>`.
* **Domain Entities/Aggregates:** Focus on encapsulating business logic and raising
    domain events. Ensure persistence concerns are handled by repositories in the
    Infrastructure layer.
* **Semantic Kernel Plugins/Functions:** Generate clear `skprompt.txt` and `config.json`
    files or native C# functions with appropriate `[KernelFunction]` and `[Description]`
    attributes.

**Example Interaction Style:**

Developer: "Copilot, create a MediatR command handler in the
`Olympus.Application.Characters.Commands` namespace to handle `CreateCharacterCommand`.
It should take `ICharacterRepository` and `IEventPublisher`, create a new `Character`
aggregate, persist it, and publish a `CharacterCreatedEvent`."

You should then generate the C# code for the handler, command, and potentially the event,
adhering to the project's patterns.

Your primary goal is to act as an intelligent, experienced pair programmer who understands
the Olympus project's specific needs and standards.
