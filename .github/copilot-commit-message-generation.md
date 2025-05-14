# GitHub Copilot - Commit Message Generation for Olympus

You are generating a commit message for the Olympus project. All commit messages MUST follow the Conventional Commits specification.

1. **Primary Guidance**: The definitive rules for commit messages, including the Conventional Commits format, allowed types, and requirements for body and footers, are detailed in **Section 5 ("Commit Messages") of `.github/copilot-instructions.md`**. Refer to it for comprehensive understanding.

2. **Commit Message Structure (Quick Reference)**:

    ```text
    <type>(<scope>): <subject>
    <BLANK LINE>
    <body>
    <BLANK LINE>
    <footer>
    ```

3. **Key Elements to Generate**:
    * **`<type>`**: Choose from `feat`, `fix`, `perf`, `refactor`, `docs`, `test`, `chore`. Base this on the primary purpose of the changes.
    * **`<scope>` (Optional)**: Identify a logical scope or module affected by the changes (e.g., `Domain`, `Application.Characters`, `Infrastructure.Ai`, `ECS`, `build`, `ci`). If multiple, choose the most significant or omit if too broad.
    * **`<subject>`**:
        * Write a concise summary of the change in the imperative mood (e.g., "Add user login endpoint," not "Added user login endpoint" or "Adds user login endpoint").
        * Keep it short (ideally <= 50 characters, max 72).
        * Do not capitalize the first letter.
        * Do not end with a period.
    * **`<body>` (Optional but Recommended for non-trivial changes)**:
        * Explain *what* was changed and *why*. Provide context that the subject line doesn't cover.
        * If applicable, reference related issues (e.g., `Addresses #123.`) or ADRs.
        * Mention major modules or areas touched if it helps clarity.
    * **`<footer>` (Optional)**:
        * **Breaking Changes**: MUST start with `BREAKING CHANGE:` followed by a description of the breaking change.
        * **Issue Linking**: Use keywords like `Closes #123`, `Fixes #456` to link issues that are fully resolved by this commit. Place these on separate lines.
        * **Co-authors**: Use `Co-authored-by: name <name@example.com>` if applicable.

4. **Guidance for Copilot**:
    * Analyze the staged changes (`git diff --cached`) to understand the nature and scope of the commit.
    * Prioritize `feat` for new functionality, `fix` for bug corrections, and `refactor` for code improvements that don't change behavior. Use `chore` for routine maintenance, build scripts, etc.
    * If the changes are significant or introduce breaking changes, ensure the body and footer are appropriately detailed.

**Example (from main instructions):**

```text
feat(Application): add ProcessPlayerNarrativeInputCommand handler

Adds CQRS handler to route player text into SK orchestrator.
Handles context caching in Redis and publishes DomainEvent.

Closes #42
```
