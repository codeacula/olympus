---
applyTo: "**/*" # Applies generally when PR generation is requested
---

## Olympus Pull Request Title & Description Generation Guidelines

Generate PR titles and descriptions for Olympus that are clear, complete, and adhere
to project conventions.

**PR Title:**

* **Format:** `type(scope): Concise imperative description`
  * Type: `feat`, `fix`, `refactor`, `docs`, `chore`, etc.
  * Scope (optional): `domain`, `app`, `infra-ai`, etc.
* **Clarity:** Immediately tell what the PR is about.
* **Brevity:** Aim for 50-70 characters.

**Examples of PR Titles:**

* `feat: Implement character creation endpoint and service`
* `fix(infra-ai): Correct prompt for NPC dialogue generation`
* `refactor(domain): Introduce Value Objects for character stats`

**PR Description (Markdown):**

**1. Summary / Purpose:**

* Brief (1-3 sentences) overview of what the PR achieves and why.
* State the problem solved or feature implemented.
    *Example:* "This PR introduces the initial character creation flow, including API,
    application service, domain aggregate, and Marten persistence. Addresses user story #XYZ."

**2. Related Issue(s):**

* Link to GitHub issues: `Closes #<issue_number>`, `Fixes #<issue_number>`,
    `Relates to #<issue_number>`.
    *Example:*

    ```markdown
    Closes #42
    Relates to #35
    ```

**3. Technical Explanation of Changes:**

* Detailed technical breakdown of changes. Approach and key decisions.
* Significant architectural changes or new patterns introduced.
* Explain *how* changes address the problem/feature.
    *Example:* "`CharactersController` has a new `POST /api/characters`. `CreateCharacterCommand`
    is handled by `CreateCharacterCommandHandler`, using `Character` aggregate factory and
    `ICharacterRepository`. Publishes `CharacterCreatedEvent`."

**4. List of Key Files Changed and General Changes:**

* List important files/modules affected and a high-level summary of changes in each.
    *Example:*

    ```markdown
    * `src/Olympus.Api/Controllers/CharactersController.cs`: Added `POST /api/characters`.
    * `src/Olympus.Application/Characters/Commands/CreateCharacterCommand.cs`: New command & handler.
    * `src/Olympus.Domain/Aggregates/Character.cs`: Added `CreateNew(...)` factory.
    * `src/Olympus.Infrastructure.Persistence.Marten/Repositories/MartenCharacterRepository.cs`: Implemented `AddAsync`.
    ```

**5. How to Test / Validation Steps:**

* Manual testing steps, API requests (cURL, Postman), or scenarios.
* Mention new automated tests.
    *Example:* "POST to `/api/characters` with `{ "name": "Test", "class": "Mage" }`.
    Verify 201 and DB entry. Unit tests for `CreateCharacterCommandHandler` added."

**6. Screenshots / GIFs (If Applicable).**

**7. Checklist (Reference `PULL_REQUEST_TEMPLATE.md` or include relevant items):**

  ```markdown
  - [ ] Code follows architecture and patterns
  - [ ] No breaking changes (or documented)
  - [ ] Manual test/validation performed
  - [ ] New unit tests added
  - [ ] Docs updated (if applicable)
  ```

Populate these sections based on commit history, diff, and issue information.
Prioritize a clear technical explanation and summary of file changes.
