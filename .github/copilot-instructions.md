---
applyTo: "**"
---

## About the Olympus Project

Olympus is an AI-Powered Tabletop RPG Platform. Its core mission is to provide a flexible, immersive, and deeply interactive environment for playing various tabletop roleplaying games.

**Key Architectural Pillars & Principles:**
* **Language:** C# (targeting .NET 9+)
* **Primary Architecture:** Clean Architecture (Domain, Application, Infrastructure, Presentation/Clients)
* **Core Patterns:**
    * CQRS (Command Query Responsibility Segregation) using MediatR for in-process dispatch.
    * Event Sourcing (ES) with Marten on PostgreSQL for key domain entities.
    * Data-Oriented Programming (DOP) influences & Entity Component System (ECS) for dynamic game world state (generic NPCs, environment) primarily in Redis.
* **AI Integration:** Semantic Kernel with OpenAI as the initial LLM.
* **Clients:** Starting with Discord bots, with a Vue.js web portal planned.

**Developer Persona:**
Assume you are assisting a **Senior Software Engineer** experienced with these patterns and C#/.NET. Focus on providing idiomatic, performant, and maintainable code. Avoid overly simplistic explanations unless specifically asked.

**Key Technologies & Libraries:**
* .NET 9+
* ASP.NET Core
* MediatR
* Marten (for PostgreSQL Event Sourcing)
* Redis (for ECS and caching)
* Semantic Kernel (for AI orchestration)
* xUnit (for testing)
* Discord.Net (for Discord bot client)
* Potentially Vue.js for the web portal.

**General Coding Style & Expectations:**
* Adhere to Clean Architecture principles: strict separation of concerns, dependency rule (flow inwards).
* Embrace modern C# features (records, primary constructors, pattern matching, `required` members, static abstract members in interfaces, file-local types, source-generated logging, `IAsyncEnumerable<T>`, `ValueTask<T>`).
* Prioritize immutability for DTOs, Commands, Queries, Events, and Value Objects (use `record class`, `readonly record struct`).
* Implement explicit error handling using `Result<TSuccess, TError>` types (Railway Oriented Programming).
* Use `Option<T>` for values that may legitimately be absent.
* Ensure high cohesion and low coupling.
* Write testable code. Unit tests are expected for new logic.
* Follow existing patterns within the codebase (e.g., Value Objects, Aggregates, MediatR command/query handlers, feature slices).
* Keep AI prompts and plugin configurations declarative.
* Structure new features like vertical slices where appropriate.

Refer to the `plan.md` document in the repository for a more detailed architectural blueprint.
When generating code, assume we are within a feature slice or appropriate layer of the Clean Architecture.