---
applyTo: "**/*" # Applies generally when commit message generation is requested
---

# Olympus Commit Message Generation Guidelines

When generating commit messages for Olympus, follow **Conventional Commits** (v1.0.0).

**Format:**

```text

\<type\>[optional scope]: \<description\>

[optional body]

[optional footer(s)]

```

**1. Header (`<type>[optional scope]: <description>`):**

* **Type:** One of:
  * `feat`: A new feature or significant functionality.
  * `fix`: A bug fix.
  * `build`: Changes to build system or external dependencies.
  * `chore`: Non-src/test changes (e.g., .gitignore).
  * `ci`: CI configuration changes.
  * `docs`: Documentation only.
  * `perf`: Performance improvements.
  * `refactor`: Code change that neither fixes a bug nor adds a feature.
  * `style`: Formatting, white-space, etc. (no code meaning change).
  * `test`: Adding or correcting tests.
* **Scope (Optional):** Noun describing the codebase section (e.g., `domain`, `app`,
    `infra-ai`, `api`, `bot-discord`).
* **Description:**
  * Imperative, present tense (e.g., "add" not "added" or "adds").
  * Lowercase first letter. No period at the end.
  * Concise (under 50 chars ideally), technically descriptive of *what changed*.
  * Example: `feat(app): add CreateCharacterCommand and handler`
        (Not: `feat: implement character creation`)

**2. Body (Optional):**

* Imperative, present tense.
* Explain *what* and *why* vs. *how*.
* Motivation for change, contrast with previous behavior.
* Wrap lines at 72 characters.

**3. Footer (Optional):**

* **Breaking Changes:** Start with `BREAKING CHANGE:` followed by a summary.

    ```text
    BREAKING CHANGE: The `ProcessNarrativeInput` command now requires `SessionId`.
    ```

* **Issue Linking:** `Closes #123`, `Relates to #456`.

**Examples:**

**Simple Fix:**

```text

fix(app): ensure UserId passed to CampaignCreationService

Previously, a default Guid was used. This ensures the authenticated
user's ID is properly utilized.

```

**New Feature with Scope:**

```text

feat(domain): implement basic combat resolution in DamageResolutionService

Adds methods for calculating damage considering armor and resistances.
Does not yet include status effects or critical hits.

```

Analyze staged changes for type, scope, and a concise, technical description.
Suggest breaking large changes into smaller commits if necessary.
