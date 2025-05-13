---
applyTo: "**/*.md"
---

## 1. Basics
- Headings use `#` through `####` without numeric prefixes.
- Ordered lists **always** start with `1.`.
- Unordered lists use `-`.
- Keep line length ≤100 characters.

## 2. Code Blocks
- Fence with triple backticks and a language tag. Indent nested blocks by four spaces to avoid premature close.

Example:

    ```markdown
        ```csharp
        // demo
        ```
    ```

## 3. Inline Code
Use backticks for short identifiers like `Result<T>`.

## 4. Tables
- Avoid unless aligning numeric data.
- First row is header, separated by dashes.

## 5. Emojis
Allowed sparingly for emphasis in templates (e.g., PR checklist).
