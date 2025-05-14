# GitHub Copilot - Core Architectural Instructions for Olympus

**Project Vision:** Olympus is an AI-Powered Tabletop RPG Platform, designed to provide a flexible, immersive, and deeply interactive TTRPG environment by leveraging AI.

**Core Guiding Principle for Copilot:** Adhere to the architectural vision. This document provides core architectural directives. More detailed general coding principles and language-specific conventions are provided in other instruction files that the IDE makes available to you based on the current file context or task.

---

## 1. Olympus Core Architecture & Key Technologies

- **Clean Architecture Mandate**:
  - Strict layer separation: `Domain` -> `Application` -> `Infrastructure` / `Presentation (API, Clients)`.
  - Dependency Rule: Dependencies flow inwards ONLY.
  - Boundaries are defined with interfaces in inner layers, implemented by outer layers.
- **CQRS & Event Sourcing (ES) - Architectural Blueprint**:
  - **CQRS Philosophy**: Operations that change state (Commands) are segregated from operations that read state (Queries).
  - **Dispatcher Abstraction**: Interaction with the command/query dispatching mechanism from Application or Presentation layers MUST use the `IOlympusDispatcher` abstraction.
    - (The underlying implementation is an Infrastructure concern).
  - **Event Sourcing Approach**: Core domain entities employ event sourcing; domain events are the source of truth for their state changes.
  - **Persistence Strategy**: Marten (on PostgreSQL) is the designated technology for event storage and read model projections.
- **Key Technologies & Strategic Roles**:
  - **.NET (C#)**: Primary backend technology stack.
  - **ASP.NET Core**: Framework for `Olympus.Api`.
  - **PostgreSQL (via Marten)**: Primary data persistence solution.
  - **Redis**: Strategic caching solution.
  - **Semantic Kernel**: Core of the AI layer (`Olympus.Infrastructure.Ai`).
- **Data-Oriented Programming (DOP) - Architectural Stance**:
  - The architecture favors clear, immutable data structures and transformations.
- **Error Handling & Optionality - Architectural Stance**:
  - The architecture mandates explicit signaling for fallible operations and optional data.
- **Architectural Quality: Testability**:
  - All system components MUST be designed with testability as a primary concern, influencing choices towards modularity, clear interfaces, and dependency injection.
- **Architectural Process: Decision Records**:
  - Significant architectural decisions impacting the overall system design or choices of core technologies are formally documented. (The practice of writing these records is detailed in general coding guidelines).

## 2. Critical "Must Avoids" (Global Rules for Copilot)

- **Clean Architecture Violations**: No direct references from inner to outer layers.
- **Service Locator Pattern / Direct Instantiation of Dependencies**: Use constructor-based Dependency Injection with abstractions.
- **Hard-Coding Configuration**: All configuration MUST be sourced externally.
- **Implicit Error Handling / Nulls for Optionality**: Adhere to the explicit error handling and optionality patterns established architecturally.
- **Mutable Static State**: Avoid due to testability and concurrency issues.
- **Unmanaged `DateTime.Now` / `DateTime.UtcNow`**: Use an `IDateTimeProvider` abstraction for testable time-dependent logic.

## 3. Files & Folders for Copilot to Generally Ignore

- `.git/`, `**/bin/`, `**/obj/`, `.vs/`, `*.user`, `*.suo`, `node_modules/`, `secrets.json`, `**/Class1.cs` (default templates).

## 4. Understanding the Hierarchy & Context of Instructions

- **This file (`copilot-instructions.md`) provides foundational architectural directives for Olympus.** It is always in context for you.
- **Contextual Instructions**: You will automatically receive more detailed, context-relevant guidance from other files based on the file you are working on (e.g., from `.github/instructions/general-coding.instructions.md` for general principles, or `.github/instructions/csharp.instructions.md` for C# specifics).
- **Project Plan Awareness**: For tasks involving new feature placement, understanding module responsibilities, or designing interactions between components, be aware that `plan.md` in the repository root contains detailed project structure, module responsibilities, and illustrative workflows. Strive to align your suggestions with the information in `plan.md` when contextually appropriate for such tasks.
- **Task-Specific Instructions**: When a specific Copilot feature is invoked (e.g., via IDE context menus for code generation, test generation, commit message generation, etc.), the IDE settings link that feature to a dedicated `copilot-[task].md` file. This file provides focused instructions for that operation, drawing upon the architectural and contextual guidelines.
- **Precedence**: More specific instructions (e.g., in `csharp.instructions.md` for C# code, or in a `copilot-[task].md` file for that task) take precedence over more general instructions in this file if an apparent conflict arises for that specific context or task.
- **Primary Goal**: Generate clear, maintainable, robust code and content that strictly adheres to the established patterns and architectural vision. If guidance seems incomplete or conflicting for a specific, nuanced scenario, state this and ask for developer clarification.
