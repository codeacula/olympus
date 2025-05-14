---
applyTo: "**"
---
# Code Generation

## 1. Code Generation Methodology & Approach

- **Understand the Request & Context**:
  - Before generating, ensure you fully understand the goal of the requested code, its required inputs/outputs, and its specific location (project, layer, module) based on `plan.md` and existing code.
  - If the request is ambiguous or lacks critical details to proceed according to architectural and coding standards, ask clarifying questions.
- **Chain-of-Thought for Complexity**:
  - For requests involving non-trivial logic, new algorithms, or significant feature components, briefly outline your plan or reasoning steps (chain-of-thought) *before* writing the primary code block to ensure alignment.
- **Implementation Candidates (for New, Complex Functionality)**:
  - When asked to generate a substantial new piece of functionality (e.g., a new CQRS handler and its related classes, a new service, a complex domain method):
        1. If appropriate and multiple viable approaches exist that satisfy project principles, consider proposing two distinct implementation options.
        2. If proposing options, briefly list the pros and cons of each from the perspective of the project's established principles (e.g., maintainability, performance, adherence to patterns).
        3. Proceed with the implementation that best aligns with project standards or present the options for developer decision if trade-offs are significant.
  - For simpler, well-defined functions, boilerplate code, or minor additions, this multi-candidate approach is likely unnecessary; prioritize efficient and direct generation.
- **Completeness (Awareness of Related Components)**:
  - When generating a primary component of a new feature (e.g., a new CQRS command and handler):
    - Consider and, if appropriate, stub out or mention other necessary related components (e.g., DTOs, validators, event handlers, mappings) that would typically complete the feature slice according to `plan.md`.
    - Note where corresponding unit tests should be located and what key scenarios they might cover. (Actual test generation is guided by `.github/copilot-test-generation.md` and language-specific testing conventions).
- **Adherence to Existing Local Patterns**:
  - When adding code to an existing module, class, or layer, identify and closely follow the prevalent design patterns, coding style, and conventions already in use in that specific local context.

## 2. Key Directives for Generated Code Content & Structure

- **File Placement**:
  - New files MUST be placed in the correct architectural layer and specific feature slice/module folder.
  - **The authoritative guide for file and folder organization is `plan.md` (Section 4: Detailed Project & Folder Structure).** Adhere to it strictly.
- **Testability Focus**:
  - Generated code MUST be designed with testability as a primary concern, aligning with the "Architectural Quality: Testability" principle (from `.github/copilot-instructions.md`). This includes appropriate use of interfaces, dependency injection, and separation of concerns.
- **Adherence to "Critical Must Avoids"**:
  - You MUST pay extremely close attention to and strictly follow all rules listed in the "Critical Must Avoids" section of the main `.github/copilot-instructions.md`.

## 3. Output Expectations

- Generate clean, readable, efficient, and idiomatic code that fully aligns with all implicitly understood foundational instructions (architectural, general, language-specific) and the specific directives in this file.
- If a request cannot be fulfilled according to these collective guidelines, or if fulfilling it would violate a core principle, state this and explain why. Suggest alternatives if possible.
