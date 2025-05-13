
## 1. Big‑Picture Context
- Olympus is a modular monolith that **must** respect Clean Architecture boundaries (`Domain`, `Application`, `Infrastructure`, `Presentation/Clients`).
- Commands and Queries flow through MediatR; key aggregates use Event Sourcing (Marten) and project read models to Redis.
- The AI layer wraps Semantic Kernel; external events will eventually emit to RabbitMQ.

## 2. What Copilot Should Propose
1. **Layer‑appropriate constructs only**  
   - Domain: aggregates, VOs, domain services.  
   - Application: CQRS handlers, orchestration, Result/Option types.  
   - Infrastructure: concrete data access, ECS, SK plugins.
1. **Modern C# idioms**  
   - Records (`record class`, `readonly record struct`), required members, pattern matching, primary constructors.
1. **Error handling**  
   - Use `Result<TSuccess,TError>` and `Option<T>` rather than exceptions in core flows.
1. **Logging**  
   - Prefer `LoggerMessage` source‑generation; surface errors with structured logging.
1. **Testing first**  
   - Generate xUnit tests per [copilot-test-generation.md](copilot-test-generation.md).

## 3. What Copilot Must Avoid
- Cross‑layer references (e.g., Domain → Infrastructure).
- God objects or static state.
- Hard‑coding configuration (use `IOptions<T>`).
- Generating code without XML docs when a public API.

## 4. File References
- Commits: [copilot-commit-message-generation.md](copilot-commit-message-generation.md)
- PRs: [copilot-pr-title-and-description.md](copilot-pr-title-and-description.md)
- Reviews: [copilot-code-review.md](copilot-code-review.md)