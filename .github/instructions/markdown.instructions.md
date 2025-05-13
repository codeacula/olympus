---
applyTo: "**/*.md"
---

## Markdown Writing Guidelines for Olympus

All Markdown files in the Olympus project should adhere to the following rules, consistent
with common markdown linting practices (e.g., markdownlint).

**General Formatting:**

* **Line Length:** Keep lines to a reasonable length, ideally under 120 characters, for
    better readability in various editors and diff tools. Wrap long lines if necessary.
* **Encoding:** Use UTF-8 encoding.
* **Line Endings:** Use LF line endings (Unix-style).
* **Trailing Spaces:** Do not use trailing spaces at the end of lines.
* **Blank Lines:**
    * Use a single blank line to separate block elements (e.g., paragraphs, headings,
        lists, code blocks).
    * Do not use multiple consecutive blank lines.
    * End files with a single newline character.

**Headings:**

* Use ATX style headings (`# Heading 1`, `## Heading 2`, etc.).
* Start headings with a single hash (`#`) for H1.
* Do not skip heading levels (e.g., H1 directly to H3).
* Ensure a single space exists between the hash(es) and the heading text.
* Headings should be surrounded by a single blank line (above and below), unless at
    the beginning or end of the document.

**Emphasis:**

* For italics, use a single asterisk: `*italic*`.
* For bold, use double asterisks: `**bold**`.

**Lists:**

* **Unordered Lists:** Use hyphens (`-`) consistently.
    * Indent sub-items by two spaces.
    * Ensure a single space after the list marker (e.g., `- `).
* **Ordered Lists:** Use `1.` for all items in an ordered list.
    * Ensure a single space after the list marker (e.g., `1. `).
* List items can contain multiple paragraphs or other block elements; these should be
    indented to the level of the list item marker plus one space.

**Code Blocks:**

* For fenced code blocks, use three backticks (```).
* Specify the language identifier immediately after the opening backticks
    (e.g., ````csharp`, ````markdown`, ````json`).
    ```csharp
    public class Example
    {
        public string Message { get; set; } = "Hello";
    }
    ```
* Do not indent fenced code blocks themselves.
* For inline code, use single backticks: `` `code` ``.

**Links and Images:**

* **Links:** Use inline links `[Link Text]\(url "Optional Title"\)`.
* Link URLs should generally be absolute unless linking to other files within the
    repository, in which case relative paths are preferred.
* Do not use angle brackets around auto-links unless necessary.

**Blockquotes:**

* Use `>` for blockquotes.
    ```markdown
    > This is a blockquote.
    > It can span multiple lines.
    ```

**Horizontal Rules:**

* Use three or more hyphens (`---`) on a line by themselves.
    ```markdown
    ---
    ```

**HTML:**

* Avoid using raw HTML unless absolutely necessary.

**Tables:**

* Format tables clearly with pipes (`|`) and hyphens (`-`) for the header separator.
* Align column content using colons in the header separator line.
    ```markdown
    | Header 1 | Header 2 | Header 3 |
    | :------- | :------: | -------: |
    | Left     | Center   | Right    |
    | Cell     | Cell     | Cell     |
    ```

**File Specifics:**

* For `.github/copilot-*.md` and `.github/instructions/*.md` files, include a YAML
    Front Matter block at the beginning if `applyTo` or other metadata is needed.
    ```yaml
    ---
    applyTo: "**/*.cs"
    ---
    ```

Always strive for clarity, consistency, and semantic correctness in your Markdown.