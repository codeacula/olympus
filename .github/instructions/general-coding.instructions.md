---
applyTo: "**/*"
---

## General Coding Guidelines for Olympus

As a Senior Software Engineer contributing to Olympus, ensure your code embodies the following principles:

**1. Readability and Maintainability:**

* Write clear, concise, and self-documenting code.
* Use meaningful names for variables, methods, classes, and other constructs. Avoid overly
    terse or ambiguous names.
* Employ consistent formatting. Refer to the `.editorconfig` for specific rules.
* Break down complex logic into smaller, manageable functions/methods.
* Comment code where the intent isn't immediately obvious or to explain complex
    algorithms/decisions. However, strive for code that explains itself.

**2. Robustness and Error Handling:**

* Anticipate potential failure points and handle errors gracefully.
* Utilize the `Result<TSuccess, TError>` pattern for operations that can fail, clearly
    defining error types.
* Use `Option<T>` for values that are legitimately optional, avoiding nulls where possible.
* Validate inputs and arguments, especially for public-facing APIs and service boundaries.

**3. Performance:**

* Be mindful of performance implications, especially in critical paths or hot code.
* Avoid premature optimization, but make sensible choices regarding data structures and algorithms.
* Leverage asynchronous programming (`async`/`await`) correctly to prevent blocking and
    improve scalability. Use `ValueTask<T>` where appropriate.

**4. Design Principles:**

* Adhere to SOLID principles.
* Strive for high cohesion within modules/classes and low coupling between them.
* Follow the "Don't Repeat Yourself" (DRY) principle.
* Favor composition over inheritance where appropriate.
* Ensure implementations are testable; write unit tests for new logic.

**5. Immutability:**

* Prefer immutable data structures where practical, especially for DTOs, events, and value objects.
* Utilize C# records (`record class`, `readonly record struct`) for this purpose.

**6. Asynchronous Programming:**

* Use `async` and `await` for I/O-bound operations and other long-running tasks.
* Propagate `CancellationToken`s where appropriate to support cancellation.
* Be aware of potential deadlocks and context-switching issues, especially in library code
    (`ConfigureAwait(false)`).

**7. Security:**

* Be aware of common security vulnerabilities (e.g., injection attacks, insecure direct
    object references) and write code to mitigate them.
* Validate and sanitize all external inputs.
* Follow the principle of least privilege.

**8. Testing:**

* Write unit tests for all new business logic and complex components.
* Aim for clear, concise tests that follow the AAA (Arrange, Act, Assert) pattern.
* Each test method should ideally test a single concern.
* Ensure tests are independent and can be run in any order.

When in doubt, refer to the project's `plan.md` for architectural guidance and existing code
for established patterns.